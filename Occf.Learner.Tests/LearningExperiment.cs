using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Accord.MachineLearning.DecisionTrees;
using Code2Xml.Core;
using Code2Xml.Languages.ANTLRv3.Core;
using Occf.Learner.Core.Tests.LearningAlgorithms;
using Paraiba.Collections.Generic;
using Paraiba.Linq;
using Paraiba.Xml.Linq;

namespace Occf.Learner.Core.Tests {
	public abstract class LearningExperiment {
		public int WrongCount { get; set; }
		protected abstract Processor Processor { get; }
		private readonly ISet<string> _elementNames;
		private Dictionary<string, HashSet<XElement>> _allElements;
		private HashSet<XElement> _acceptedElementsWithSeed;
		private HashSet<XElement> _acceptedElements;
		private HashSet<XElement> _rejectedElements;
		private HashSet<XElement> _seedElements;
		private HashSet<XElement> _learningElements;
		private int _predicateDepth = 10;
		private const int LearningCount = 10;
		private const int DeniedThreshold = 300;

		protected LearningExperiment(params string[] elementNames) {
			_elementNames = elementNames.ToHashSet();
		}

		public void LearnUntilBeStable(
				ICollection<string> allPaths, ICollection<string> seedPaths, LearningAlgorithm learner,
				double threshold) {
			Console.WriteLine(learner);
			_allElements = new Dictionary<string, HashSet<XElement>>();
			_acceptedElements = new HashSet<XElement>();
			_seedElements = new HashSet<XElement>();
			_learningElements = new HashSet<XElement>();
			_acceptedElementsWithSeed = new HashSet<XElement>();
			foreach (var path in allPaths) {
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				_allElements.Add(path, GetAllElements(ast).ToHashSet());
				_acceptedElements.UnionWith(GetAcceptedElements(ast));
			}
			_acceptedElementsWithSeed.UnionWith(_acceptedElements);
			foreach (var path in seedPaths) {
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				_seedElements.UnionWith(GetAllElements(ast).ToHashSet());
				_acceptedElementsWithSeed.UnionWith(GetAcceptedElements(ast));
			}
			_rejectedElements = _allElements.Values.SelectMany(e => e).ToHashSet();
			_rejectedElements.ExceptWith(_acceptedElements);
			_learningElements.UnionWith(_seedElements);
			while (true) {
				var time = Environment.TickCount;
				var ret = LearnAndApply(learner);
				if (ret >= threshold) {
					break;
				}
				Console.WriteLine("Time: " + (Environment.TickCount - time));
			}
			Console.WriteLine("Required elements: " + _learningElements.Count);
		}

		public class ProbAndElement {
			public double Porb { get; set; }
			public XElement Element { get; set; }
		}

		public class LearningItem {
			public string Path { get; set; }
			public long FileSize { get; set; }
			public int AcceptedCount { get; set; }
			public double Probability { get; set; }
		}

		private double LearnAndApply(LearningAlgorithm algorithm) {
			LearningData learningData = null;
			Func<double[], Tuple<bool, double>> judge = null;
			do {
				double error;
				learningData = CreatePredicatesAndLearningData();
				judge = algorithm.Learn(learningData, out error);
				if (error == 0) {
					break;
				}
				_seedElements.UnionWith(_learningElements);
				_predicateDepth++;
				Console.WriteLine("Error: " + error + ", " + _seedElements.Count + ", " + _predicateDepth);
			} while (_seedElements.Count < _learningElements.Count);

			var correctlyAccepted = 0;
			var correctlyRejected = 0;
			var wronglyAccepted = 0;
			var wronglyRejected = 0;

			var positiveList = new List<Tuple<double, XElement, string>>
			{ Tuple.Create<double, XElement, string>(1, null, null) };
			var negativeList = new List<Tuple<double, XElement, string>>
			{ Tuple.Create<double, XElement, string>(1, null, null) };
			foreach (var kv in _allElements) {
				var path = kv.Key;
				var elements = kv.Value;
				foreach (var e in elements) {
					var input = GetClassifierInput(e, learningData);
					var tuple = judge(input);
					var actual = tuple.Item1;
					var expected = _acceptedElements.contains(e);
					var prob = tuple.Item2;
					if (actual) {
						if (expected) {
							correctlyAccepted++;
						} else {
							if (wronglyAccepted == 0) {
								Console.WriteLine("WA (" + prob + "): " + e.SafeParent().SafeParent().SafeParent());
							}
							wronglyAccepted++;
						}
					} else {
						if (expected) {
							if (wronglyRejected == 0) {
								Console.WriteLine("WR (" + prob + "): " + e.SafeParent().SafeParent().SafeParent());
							}
							wronglyRejected++;
						} else {
							correctlyRejected++;
						}
					}
					UpdateLists(prob, e, path, positiveList, negativeList);
				}
				Console.Write(".");
			}
			Console.WriteLine("done");
			Console.WriteLine("WA: " + wronglyAccepted + ", WD: " + wronglyRejected + ", CA: "
			                  + correctlyAccepted + ", CD: " + correctlyRejected + ", Count: "
			                  + _learningElements.Count);
			Console.WriteLine("Positive: " + positiveList[0].Item1 + ", Negative: "
			                  + negativeList[0].Item1);
			Console.WriteLine("Accepted: " + _learningElements.Count(e => _acceptedElements.Contains(e))
			                  + " / " + _acceptedElements.Count);
			WrongCount = wronglyAccepted + wronglyRejected;
			if (positiveList[0].Item1 < negativeList[0].Item1) {
				if (positiveList[0].Item3 != null) {
					_learningElements.UnionWith(_allElements[positiveList[0].Item3]);
					Console.WriteLine(positiveList[0].Item3);
				}
			} else {
				if (negativeList[0].Item3 != null) {
					_learningElements.UnionWith(_allElements[negativeList[0].Item3]);
					Console.WriteLine(negativeList[0].Item3);
				}
			}
			//_learningElements.UnionWith(positiveList.Where(e => e.Item2 != null).Select(e => e.Item2));
			//_learningElements.UnionWith(negativeList.Where(e => e.Item2 != null).Select(e => e.Item2));
			return Math.Min(positiveList[0].Item1, negativeList[0].Item1);
		}

		private void UpdateLists(
				double prob, XElement e, string path, List<Tuple<double, XElement, string>> positiveList,
				List<Tuple<double, XElement, string>> negativeList) {
			if (_learningElements.Contains(e)) {
				return;
			}
			if (prob > 0) {
				positiveList.Add(Tuple.Create(prob, e, path));
				positiveList.Sort((t1, t2) => t1.Item1.CompareTo(t2.Item1));
				if (positiveList.Count > LearningCount) {
					positiveList.RemoveAt(LearningCount);
				}
			} else {
				negativeList.Add(Tuple.Create(-prob, e, path));
				negativeList.Sort((t1, t2) => t1.Item1.CompareTo(t2.Item1));
				if (negativeList.Count > LearningCount) {
					negativeList.RemoveAt(LearningCount);
				}
			}
		}

		private LearningData CreatePredicatesAndLearningData() {
			var seedRejected = new HashSet<XElement>();
			var learningRejected = new HashSet<XElement>();
			Console.Write("Seed data: ");
			var seedAccepted = GatherAcceptedAndRejected(_seedElements, seedRejected);
			Console.Write("Learning data: ");
			var learningAccepted = GatherAcceptedAndRejected(_learningElements, learningRejected);
			var acceptedDepth2Predicates = CreatePredicates(seedAccepted);
			var rejectedDepth2Predicates = CreatePredicates(seedRejected);
			acceptedDepth2Predicates.UnionWith(rejectedDepth2Predicates);
			var np1 = TryCreatePredicates(learningAccepted);
			var np2 = TryCreatePredicates2(learningAccepted, learningRejected, np1);
			np1.UnionWith(np2);
			return CreateLearningData(np1, learningAccepted, learningRejected);
		}

		private HashSet<SurroundingElementsPredicate> TryCreatePredicates(IEnumerable<XElement> elements) {
			var dict = SurroundingElementsPredicate.GetSurroundingElements(elements, _predicateDepth);
			foreach (var kv in dict) {
				kv.Value.ClearItemsIf((key, count) => count != elements.Count());
			}
			var newPredicates = dict
					.SelectMany(kv => kv.Value
							.Select(item => new SurroundingElementsPredicate(kv.Key, item)))
					.ToHashSet();
			return newPredicates;
		}

		private HashSet<SurroundingElementsPredicate> TryCreatePredicates2(
				IEnumerable<XElement> accepted, IEnumerable<XElement> rejected,
				HashSet<SurroundingElementsPredicate> predicates) {
			var dict = SurroundingElementsPredicate.GetSurroundingElements(
					rejected.Where(e => CheckPredicate(e, predicates)), _predicateDepth);
			var dict2 = SurroundingElementsPredicate.GetSurroundingElements(accepted, _predicateDepth);
			foreach (var kv in dict2) {
				var values = dict.GetValueOrDefault(kv.Key);
				if (values != null) {
					foreach (var v in kv.Value) {
						values.ClearItem(v);
					}
				}
			}
			foreach (var kv in dict) {
				kv.Value.ClearItemsIf((key, count) => count < 2);
			}
			var newPredicates = dict
					.SelectMany(kv => kv.Value
							.Select(item => new SurroundingElementsPredicate(kv.Key, item)))
					.ToHashSet();
			return newPredicates;
		}

		private HashSet<SurroundingElementsPredicate> CreatePredicates(IEnumerable<XElement> elements) {
			var dict = SurroundingElementsPredicate.GetSurroundingElements(elements, _predicateDepth);
			foreach (var kv in dict) {
				kv.Value.ClearItemsIf((key, count) => count < 2);
			}
			var newPredicates = dict
					.SelectMany(kv => kv.Value
							.Select(item => new SurroundingElementsPredicate(kv.Key, item)))
					.ToHashSet();
			return newPredicates;
		}

		private LearningData CreateLearningData(
				HashSet<SurroundingElementsPredicate> newPredicates, ICollection<XElement> accepted,
				ICollection<XElement> rejected) {
			// TODO
			//newPredicates = newPredicates.Where(p => p.Value.EndsWith("console") || p.Value.EndsWith("log")).ToHashSet();

			var variables = new List<DecisionVariable>();
			var count = newPredicates.Count;
			for (int i = 0; i < count; i++) {
				variables.Add(new DecisionVariable(i.ToString(), DecisionVariableKind.Discrete));
			}

			Console.WriteLine("Accepted: " + accepted.Count + ", Rejected: " + rejected.Count
			                  + ", Predicates: " + count);

			var learningRecords = Enumerable.Empty<double[]>();
			var learningResults = new List<int>();
			var learningData = new LearningData {
				Variables = variables,
				NewPredicates = newPredicates,
			};
			//foreach (var predicate in newPredicates) {
			//	Console.WriteLine(predicate);
			//}
			learningRecords =
					learningRecords.Concat(
							accepted.Concat(rejected)
									.Select(e => GetClassifierInput(e, learningData)));
			learningResults.AddRange(Enumerable.Repeat(-1, accepted.Count));
			learningResults.AddRange(Enumerable.Repeat(1, rejected.Count));
			learningData.Inputs = learningRecords.ToArray();
			learningData.Outputs = learningResults.ToArray();
			return learningData;
		}

		private HashSet<XElement> GatherAcceptedAndRejected(
				IEnumerable<XElement> elements, HashSet<XElement> rejected) {
			var accepted = new HashSet<XElement>();
			foreach (var e in elements) {
				if (_acceptedElementsWithSeed.contains(e)) {
					accepted.Add(e);
				} else {
					rejected.Add(e);
				}
			}
			if (!NormalizeElementNames(accepted).SetEquals(_elementNames)) {
				Console.WriteLine("Failed to normalize element names: "
				                  + string.Join(",", NormalizeElementNames(accepted)));
			}
			Console.WriteLine("Accepted: " + accepted.Count + ", Rejected: " + rejected.Count);
			return accepted;
		}

		private static HashSet<XElement> FilterRejected(int accptedCount, HashSet<XElement> rejected) {
			var threshold = Math.Max(accptedCount * DeniedThreshold, 100);
			if (rejected.Count > threshold) {
				var newRejected = new HashSet<XElement>();
				var rand = new Random();
				foreach (var deniedElement in rejected) {
					if (rand.Next(rejected.Count) < threshold) {
						newRejected.Add(deniedElement);
					}
				}
				rejected = newRejected;
			}
			return rejected;
		}

		private bool CheckPredicate(XElement e, HashSet<SurroundingElementsPredicate> predicates) {
			var dict = SurroundingElementsPredicate.GetSurroundingElements(e, _predicateDepth);
			return predicates.All(predicate => predicate.Check(dict));
		}

		private double[] GetClassifierInput(XElement e, LearningData learningData) {
			var input = new double[learningData.NewPredicates.Count];
			var count = 0;
			var count2 = 0;
			var dict = SurroundingElementsPredicate.GetSurroundingElements(e, _predicateDepth);
			foreach (var predicate in learningData.NewPredicates) {
				input[count++] = predicate.Check(dict) ? 1 : 0;
				count2 += (predicate.Check(dict) ? 1 : 0);
			}
			//Console.WriteLine("Input: " + count2);
			return input;
		}

		private ISet<string> NormalizeElementNames(ICollection<XElement> accepted) {
			var nameSet = new HashSet<string>();
			var name2Count = new Dictionary<string, int>();
			var elements = accepted.SelectMany(e => e.AncestorsAndDescendantsWithNoSiblingAndSelf());
			foreach (var element in elements) {
				int count = 0;
				name2Count.TryGetValue(element.Name(), out count);
				name2Count[element.Name()] = count + 1;
			}
			foreach (var element in accepted) {
				var newElement = element
						.DescendantsOfOnlyChildAndSelf()
						.OrderByDescending(e => name2Count[e.Name()]).First();
				nameSet.add(newElement.Name());
			}
			return nameSet;
		}

		protected IEnumerable<XElement> GetAllElements(XElement ast) {
			return ast.DescendantsAndSelf().Where(e => _elementNames.Contains(e.Name()));
		}

		protected abstract IEnumerable<XElement> GetAcceptedElements(XElement ast);
	}

	public static class Extension {
		public static IEnumerable<XElement> AncestorsAndDescendantsWithNoSibling(this XElement element) {
			return element.DescendantsOfOnlyChild().Concat(element.AncestorsWithNoSibling());
		}

		public static IEnumerable<XElement> AncestorsAndDescendantsWithNoSiblingAndSelf(
				this XElement element) {
			return element.DescendantsOfOnlyChildAndSelf()
					.Concat(element.AncestorsWithNoSibling());
		}

		public static IEnumerable<XElement> AncestorsWithNoSiblingAndSelf(this XElement element) {
			do {
				yield return element;
				element = element.Parent;
			} while (element != null && element.Elements().Count() == 1);
		}

		public static IEnumerable<XElement> AncestorsWithNoSibling(this XElement element) {
			element = element.Parent;
			while (element != null && element.Elements().Count() == 1) {
				yield return element;
				element = element.Parent;
			}
		}

		public static IEnumerable<XElement> DescendantsWithNoSiblingAndSelf(this XElement element) {
			yield return element;
			while (element.Elements().Count() == 1) {
				element = element.FirstElement();
				yield return element;
			}
		}

		public static IEnumerable<XElement> DescendantsWithNoSibling(this XElement element) {
			while (element.Elements().Count() == 1) {
				element = element.FirstElement();
				yield return element;
			}
		}
	}
}