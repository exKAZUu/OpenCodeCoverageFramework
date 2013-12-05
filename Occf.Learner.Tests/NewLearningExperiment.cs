using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core;
using Occf.Learner.Core.Tests.LearningAlgorithms;
using Paraiba.Linq;
using Paraiba.Xml.Linq;

namespace Occf.Learner.Core.Tests {
	public abstract class NewLearningExperiment {
		public int WrongCount { get; set; }
		protected abstract Processor Processor { get; }
		private readonly ISet<string> _elementNames;
		private Dictionary<string, HashSet<XElement>> _acceptedElementDict;
		private Dictionary<string, HashSet<XElement>> _rejectedElementDict;
		private List<XElement> _seedAccepted;
		private List<XElement> _seedRejected;
		private int _predicateDepth = 10;
		private const int LearningCount = 10;

		protected NewLearningExperiment(params string[] elementNames) {
			_elementNames = elementNames.ToHashSet();
		}

		public void LearnUntilBeStable(
				ICollection<string> allPaths, ICollection<string> seedPaths, LearningAlgorithm learner,
				double threshold) {
			Console.WriteLine(learner);
			_acceptedElementDict = new Dictionary<string, HashSet<XElement>>();
			_rejectedElementDict = new Dictionary<string, HashSet<XElement>>();
			foreach (var path in allPaths) {
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				var allElements = GetAllElements(ast).ToHashSet();
				var acceptedElements = GetAcceptedElements(ast).ToHashSet();
				_acceptedElementDict.Add(path, acceptedElements);
				_rejectedElementDict.Add(path, new HashSet<XElement>());
				_rejectedElementDict[path].UnionWith(allElements);
				_rejectedElementDict[path].ExceptWith(acceptedElements);
			}

			var seedAccepted = new HashSet<XElement>();
			var seedRejected = new HashSet<XElement>();
			foreach (var path in seedPaths) {
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				seedRejected.UnionWith(GetAllElements(ast).ToHashSet());
				seedAccepted.UnionWith(GetAcceptedElements(ast));
			}
			seedRejected.ExceptWith(seedAccepted);

			_seedAccepted = new List<XElement>();
			_seedRejected = new List<XElement>();
			_seedAccepted.AddRange(seedAccepted);
			_seedRejected.AddRange(seedRejected);

			while (true) {
				var time = Environment.TickCount;
				if (LearnAndApply()) {
					break;
				}
				Console.WriteLine("Time: " + (Environment.TickCount - time));
			}
			Console.WriteLine("Required elements: " + (_seedAccepted.Count + _seedRejected.Count));
		}

		private HashSet<SurroundingElementsPredicate> CreatePredicates() {
			var preds = TryCreatePredicates(_seedAccepted);
			XElement lastElement = null;
			while (_seedRejected.Any(e => GetClassifierInput(e, preds).IsEmpty())) {
				if (lastElement == null) {
					Console.WriteLine("Failed to learn rules.");
				}
				if (_seedAccepted.Count == 0) {
					throw new Exception("You must distinguish between seeds.");
				}
				lastElement = _seedAccepted[_seedAccepted.Count - 1];
				_seedAccepted.RemoveAt(_seedAccepted.Count - 1);
				preds = TryCreatePredicates(_seedAccepted);
			}
			if (lastElement != null) {
				Console.WriteLine(lastElement.TokenText());
				var e = lastElement.SafeParent().SafeParent().SafeParent().SafeParent();
				if (e != null) {
					Console.WriteLine(e.TokenText());
				}
				throw new Exception("You must add more seeds.");
			}
			return preds;
		}

		private bool LearnAndApply() {
			var preds = CreatePredicates();
			Debug.Assert(_seedAccepted.All(e => GetClassifierInput(e, preds).IsEmpty()));

			var correctlyAccepted = 0;
			var correctlyRejected = 0;
			var wronglyAccepted = 0;
			var wronglyRejected = 0;
			var nextElements = new List<Tuple<int, XElement, string, bool, List<int>>>();

			foreach (var kv in _acceptedElementDict) {
				var path = kv.Key;
				var elements = kv.Value;
				foreach (var e in elements) {
					var diffs = GetClassifierInput(e, preds).ToList();
					var actual = diffs.Count == 0;
					if (actual) {
						correctlyAccepted++;
					} else {
						if (wronglyRejected == 0) {
							//Console.WriteLine("WR (" + prob + "): " + e.SafeParent().SafeParent().SafeParent());
						}
						wronglyRejected++;
					}
					UpdateNextElements(diffs, nextElements, e, path, true);
				}
				Console.Write(".");
			}

			foreach (var kv in _rejectedElementDict) {
				var path = kv.Key;
				var elements = kv.Value;
				foreach (var e in elements) {
					var diffs = GetClassifierInput(e, preds).ToList();
					var actual = diffs.Count == 0;
					if (actual) {
						if (wronglyAccepted == 0) {
							//Console.WriteLine("WA (" + prob + "): " + e.SafeParent().SafeParent().SafeParent());
						}
						wronglyAccepted++;
					} else {
						correctlyRejected++;
					}
					UpdateNextElements(diffs, nextElements, e, path, false);
				}
				Console.Write(".");
			}
			Console.WriteLine("done");
			Console.WriteLine("WA: " + wronglyAccepted + ", WR: " + wronglyRejected + ", CA: "
			                  + correctlyAccepted + ", CR: " + correctlyRejected + ", L: "
			                  + (_seedAccepted.Count + _seedRejected.Count) + ", P: " + preds.Count);
			Console.WriteLine("Accepted: " + _seedAccepted.Count
			                  + " / " + _acceptedElementDict.Sum(kv => kv.Value.Count));
			WrongCount = wronglyAccepted + wronglyRejected;
			if (nextElements.Count > 0) {
				Console.WriteLine("Diff: " + nextElements[0].Item1);
				foreach (var t in nextElements) {
					if (t.Item4) {
						_seedAccepted.Add(t.Item2);
					} else {
						_seedRejected.Add(t.Item2);
					}
				}
			}
			return nextElements.Count(t => t.Item4) == 0;
		}

		private static int Hash(IEnumerable<int> diffs) {
			return diffs.Aggregate(1, (current, diff) => current * diff);
		}

		private static void UpdateNextElements(List<int> diffs, List<Tuple<int, XElement, string, bool, List<int>>> nextElements, XElement e, string path, bool accepted) {
			var count = diffs.Count;
			if (count > 0) {
				if (!nextElements.Any(t => t.Item1 == count && Hash(t.Item5) == Hash(diffs))) {
					nextElements.Add(Tuple.Create(count, e, path, accepted, diffs));
					nextElements.Sort((t1, t2) => t1.Item1.CompareTo(t2.Item1));
					if (nextElements.Count > LearningCount) {
						nextElements.RemoveAt(LearningCount);
					}
				}
			}
		}

		private HashSet<SurroundingElementsPredicate> TryCreatePredicates(IEnumerable<XElement> elements) {
			var elementCount = elements.Count();
			var dict = SurroundingElementsPredicate.GetSurroundingElements(elements, _predicateDepth);
			foreach (var kv in dict) {
				kv.Value.ClearItemsIf((key, count) => count != elementCount);
			}
			var newPredicates = dict
					.SelectMany(kv => kv.Value
							.Select(item => new SurroundingElementsPredicate(kv.Key, item)))
					.ToHashSet();
			return newPredicates;
		}

		private IEnumerable<int> GetClassifierInput(
				XElement e, IEnumerable<SurroundingElementsPredicate> predicates) {
			var dict = SurroundingElementsPredicate.GetSurroundingElements(e, _predicateDepth);
			var index = 0;
			foreach (var predicate in predicates) {
				if (!predicate.Check(dict)) {
					yield return index;
				}
				index++;
			}
		}

		protected IEnumerable<XElement> GetAllElements(XElement ast) {
			return ast.DescendantsAndSelf().Where(e => _elementNames.Contains(e.Name()));
		}

		protected abstract IEnumerable<XElement> GetAcceptedElements(XElement ast);
	}
}