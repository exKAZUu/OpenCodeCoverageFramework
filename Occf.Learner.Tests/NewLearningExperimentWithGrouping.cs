using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;
using Code2Xml.Core;
using Occf.Learner.Core.Tests.LearningAlgorithms;
using Paraiba.Linq;
using Paraiba.Xml.Linq;

namespace Occf.Learner.Core.Tests {
	public abstract class NewLearningExperimentWithGrouping {
		public int WrongCount { get; set; }
		protected abstract Processor Processor { get; }
		private readonly ISet<string> _elementNames;
		private Dictionary<string, HashSet<XElement>> _acceptedElementDict;
		private Dictionary<string, HashSet<XElement>> _rejectedElementDict;
		private List<XElement> _seedAccepted;
		private List<XElement> _seedRejected;
		private HashSet<BigInteger> _learnedDiffs;
		private int _predicateDepth = 10;
		private List<HashSet<string>> _originalSeedAccepted;
		private const int LearningCount = 20;

		protected NewLearningExperimentWithGrouping(params string[] elementNames) {
			_elementNames = elementNames.ToHashSet();
		}

		public void LearnUntilBeStable(
				ICollection<string> allPaths, ICollection<string> seedPaths, LearningAlgorithm learner,
				double threshold) {
			_learnedDiffs = new HashSet<BigInteger>();
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

			_originalSeedAccepted = _seedAccepted
				.Select(e => e.GetSurroundingKeys(_predicateDepth))
				.ToList();
			var originalSeedRejected = _seedRejected.ToList();

			while (true) {
				var time = Environment.TickCount;
				if (LearnAndApply()) {
					break;
				}
				Console.WriteLine("Time: " + (Environment.TickCount - time));
			}
			Console.WriteLine("Required elements: " + (_seedAccepted.Count + _seedRejected.Count));
		}

		public static HashSet<string> GetCommonKeys(IEnumerable<HashSet<string>> targets) {
			HashSet<string> commonKeys = null;
			foreach (var target in targets) {
				var keys = target;
				if (commonKeys == null) {
					commonKeys = keys;
				} else {
					commonKeys.IntersectWith(keys);
				}
			}
			return commonKeys;
		}

		private bool CanReject(IEnumerable<HashSet<string>> predicates, IEnumerable<HashSet<string>> rejected) {
			return predicates.All(p => rejected.All(r => p.Any(p2 => !r.Contains(p2))));
		}

		private List<HashSet<string>> LearnCommonKeys() {
			var histories = new Stack<int>();
			var count = _seedAccepted.Count;
			var learningSet = _originalSeedAccepted
					.Select(s => new Stack<HashSet<string>>(Enumerable.Repeat(s, 1)))
					.ToList();
			var accepted = _seedAccepted
					.Select(e => e.GetSurroundingKeys(_predicateDepth))
					.ToList();
			var rejected = _seedRejected
					.Select(e => e.GetSurroundingKeys(_predicateDepth))
					.ToList();
			for (int i = 0; i < count; i++) {
				for (int j = 0;; j++) {
					learningSet[j].Push(accepted[i]);
					var predicates = learningSet.Select(GetCommonKeys).ToList();
					if (CanReject(predicates, rejected)) {
						histories.Push(j);
						break;
					}
					learningSet[j].Pop();
					while (j == learningSet.Count - 1) {
						if (histories.IsEmpty()) {
							throw new Exception("Failed to learn");
						}
						i--;
						j = histories.Pop();
						learningSet[j].Pop();
					}
				}
			}
			return learningSet.Select(GetCommonKeys).ToList();
			//var commonKeys = _seedAccepted.GetCommonKeys(_predicateDepth);
			//XElement lastElement = null;
			//BigInteger diffs;
			//while (_seedRejected.Any(e => GetClassifierInput(e, commonKeys, out diffs) == 0)) {
			//	if (lastElement == null) {
			//		Console.WriteLine("Failed to learn rules.");
			//	}
			//	if (_seedAccepted.Count == 0) {
			//		throw new Exception("You must distinguish between seeds.");
			//	}
			//	lastElement = _seedAccepted[_seedAccepted.Count - 1];
			//	_seedAccepted.RemoveAt(_seedAccepted.Count - 1);
			//	commonKeys = _seedAccepted.GetCommonKeys(_predicateDepth);
			//}
			//if (lastElement != null) {
			//	Console.WriteLine(lastElement.TokenText());
			//	var e = lastElement.SafeParent().SafeParent().SafeParent().SafeParent();
			//	if (e != null) {
			//		Console.WriteLine(e.TokenText());
			//	}
			//	throw new Exception("You must add more seeds.");
			//}
			//return commonKeys;
		}

		private bool LearnAndApply() {
			var commonKeys = LearnCommonKeys();
			BigInteger diffs;
			//Debug.Assert(_seedAccepted.All(e => GetClassifierInput(e, commonKeys, out diffs) == 0));

			var correctlyAccepted = 0;
			var correctlyRejected = 0;
			var wronglyAccepted = 0;
			var wronglyRejected = 0;
			var nextElements = new List<Tuple<int, XElement, string, bool, BigInteger>>();

			foreach (var kv in _acceptedElementDict) {
				var path = kv.Key;
				var elements = kv.Value;
				foreach (var e in elements) {
					var diffCount = GetClassifierInput(e, commonKeys, out diffs);
					var actual = diffCount == 0;
					if (actual) {
						correctlyAccepted++;
					} else {
						if (wronglyRejected == 0) {
							//Console.WriteLine("WR (" + prob + "): " + e.SafeParent().SafeParent().SafeParent());
						}
						wronglyRejected++;
					}
					UpdateNextElements(diffCount, diffs, nextElements, e, path, true);
				}
				Console.Write(".");
			}

			foreach (var kv in _rejectedElementDict) {
				var path = kv.Key;
				var elements = kv.Value;
				foreach (var e in elements) {
					var diffCount = GetClassifierInput(e, commonKeys, out diffs);
					var actual = diffCount == 0;
					if (actual) {
						if (wronglyAccepted == 0) {
							//Console.WriteLine("WA (" + prob + "): " + e.SafeParent().SafeParent().SafeParent());
						}
						wronglyAccepted++;
					} else {
						correctlyRejected++;
					}
					UpdateNextElements(diffCount, diffs, nextElements, e, path, false);
				}
				Console.Write(".");
			}
			Console.WriteLine("done");
			Console.WriteLine("WA: " + wronglyAccepted + ", WR: " + wronglyRejected + ", CA: "
			                  + correctlyAccepted + ", CR: " + correctlyRejected + ", L: "
			                  + (_seedAccepted.Count + _seedRejected.Count) + ", P: " + commonKeys.Count);
			Console.WriteLine("Accepted: " + _seedAccepted.Count
			                  + " / " + _acceptedElementDict.Sum(kv => kv.Value.Count));
			Console.WriteLine("Rejected: " + _seedRejected.Count
			                  + " / " + _rejectedElementDict.Sum(kv => kv.Value.Count));
			WrongCount = wronglyAccepted + wronglyRejected;
			if (nextElements.Count > 0) {
				Console.WriteLine("Diff: " + nextElements[0].Item1);
				foreach (var t in nextElements) {
					if (t.Item4) {
						_seedAccepted.Add(t.Item2);
						_learnedDiffs.Clear();
					} else {
						_seedRejected.Add(t.Item2);
					}
				}
			}
			return nextElements.Count(t => t.Item4) == 0;
		}

		private void UpdateNextElements(
				int count, BigInteger diffs, List<Tuple<int, XElement, string, bool, BigInteger>> nextElements,
				XElement e, string path, bool accepted) {
			if (count > 0) {
				if (!_learnedDiffs.Contains(diffs)) {
					_learnedDiffs.Add(diffs);
					nextElements.Add(Tuple.Create(count, e, path, accepted, diffs));
					nextElements.Sort((t1, t2) => t1.Item1.CompareTo(t2.Item1));
					if (nextElements.Count > LearningCount) {
						nextElements.RemoveAt(LearningCount);
					}
				}
			}
		}

		private int GetClassifierInput(XElement e, IEnumerable<HashSet<string>> commonKeysSet, out BigInteger diffs) {
			//commonKeysSet.Min()
		}

		private int GetClassifierInput(XElement e, IEnumerable<string> commonKeys, out BigInteger diffs) {
			var keys = e.GetSurroundingKeys(_predicateDepth);
			var count = 0;
			diffs = BigInteger.Zero;
			foreach (var commonKey in commonKeys) {
				if (!keys.Contains(commonKey)) {
					count++;
					diffs++;
				}
				diffs <<= 1;
			}
			return count;
		}

		private BigInteger GetBit(XElement e, IEnumerable<string> predicateKeys) {
			var keys = e.GetSurroundingKeys(_predicateDepth);
			var ret = BigInteger.Zero;
			foreach (var predicateKey in predicateKeys) {
				if (keys.Contains(predicateKey)) {
					ret++;
				}
				ret <<= 1;
			}
			return ret;
		}

		protected IEnumerable<XElement> GetAllElements(XElement ast) {
			return ast.DescendantsAndSelf().Where(e => _elementNames.Contains(e.Name()));
		}

		protected abstract IEnumerable<XElement> GetAcceptedElements(XElement ast);
	}
}