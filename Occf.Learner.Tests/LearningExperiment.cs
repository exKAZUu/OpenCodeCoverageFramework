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
		private int _predicateDepth = 10;
		private const int DeniedThreshold = 300;

		protected LearningExperiment(params string[] elementNames) {
			_elementNames = elementNames.ToHashSet();
		}

		public void LearnUntilBeStable(
				IEnumerable<string> allPaths, ICollection<string> seedPaths, LearningAlgorithm learner,
				double threshold) {
			var allPathList = allPaths.ToList();
			var learningPathSet = seedPaths.ToList();
			var seedPathSet = seedPaths.ToHashSet();
			Console.WriteLine(learner);
			while (true) {
				var time = Environment.TickCount;
				string nextPath;
				var ret = LearnAndApply(allPathList, seedPathSet, learningPathSet, learner, out nextPath);
				if (ret >= threshold) {
					break;
				}
				learningPathSet.Add(nextPath);
				Console.WriteLine("Time: " + (Environment.TickCount - time));
			}
			Console.WriteLine("Required files: " + learningPathSet.Count);
		}

		public class LearningItem {
			public string Path { get; set; }
			public long FileSize { get; set; }
			public int AcceptedCount { get; set; }
			public double Probability { get; set; }
		}

		private double LearnAndApply(
				IEnumerable<string> allPaths, ISet<string> seedPathSet, IList<string> learningPaths,
				LearningAlgorithm algorithm, out string nextPath) {
			LearningData learningData = null;
			Func<double[], Tuple<bool, double>> judge = null;
			for (int i = learningPaths.Count - 1; i >= 0; i--) {
				double error;
				learningData = CreatePredicatesAndLearningData(seedPathSet, learningPaths);
				judge = algorithm.Learn(learningData, out error);
				if (error == 0) {
					break;
				}
				seedPathSet.Add(learningPaths[i]);
				_predicateDepth++;
				Console.WriteLine("Error: " + error + ", " + learningPaths[i] + ", " + _predicateDepth);
			}
			var learningPathSet = learningPaths.ToHashSet();
			var items = new List<LearningItem>();

			var correctlyAccepted = 0;
			var correctlyDenied = 0;
			var wronglyAccepted = 0;
			var wronglyDenied = 0;
			var minProb = Double.MaxValue;
			foreach (var path in allPaths) {
				Console.Write(".");
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				var all = GetAllElements(ast);
				var accepted = GetAcceptedElements(ast).ToHashSet();

				var acceptedCount = 0;
				var prob = Double.MaxValue;
				foreach (var e in all) {
					var input = GetClassifierInput(e, learningData);
					var tuple = judge(input);
					prob = Math.Min(prob, Math.Abs(tuple.Item2));
					var expected = tuple.Item1;
					var actual = accepted.Contains(e);
					if (expected != actual) {
						if (actual) {
							wronglyDenied++;
						} else {
							wronglyAccepted++;
						}
					} else {
						if (actual) {
							correctlyAccepted++;
						} else {
							correctlyDenied++;
						}
					}
					if (expected) {
						acceptedCount++;
					}
				}
				if (!learningPathSet.Contains(path)) {
					minProb = Math.Min(minProb, prob);
					items.Add(new LearningItem {
						Path = path,
						FileSize = codeFile.Length,
						AcceptedCount = acceptedCount,
						Probability = minProb,
					});
				}
			}
			//var minItem = items.Where(i => i.AcceptedCount > 0)
			//		.OrderBy(i => i.Probability)
			//		.Take(10).MinElementOrDefault(i => i.FileSize);
			var minItem = items.Where(i => i.AcceptedCount > 0)
					.OrderBy(i => i.Probability)
					.FirstOrDefault();
			if (minItem != null) {
				nextPath = minItem.Path;
			} else {
				nextPath = items
						.MinElementOrDefault(i => i.Probability).Path;
			}
			Console.WriteLine("done");
			Console.WriteLine("WA: " + wronglyAccepted + ", WD: " + wronglyDenied + ", CA: "
			                  + correctlyAccepted + ", CD: " + correctlyDenied + ", " + minProb);
			Console.WriteLine(nextPath);
			WrongCount = wronglyAccepted + wronglyDenied;
			return minProb;
		}

		private LearningData CreatePredicatesAndLearningData(
				IEnumerable<string> seedPaths, IEnumerable<string> learningPaths) {
			var seedRejected = new HashSet<XElement>();
			var allRejected = new HashSet<XElement>();
			Console.Write("Seed data: ");
			var seedAccepted = GatherAcceptedAndRejected(seedPaths, seedRejected);
			Console.Write("Learning data: ");
			var allAccepted = GatherAcceptedAndRejected(learningPaths, allRejected);
			var acceptedDepth2Predicates = CreatePredicates(seedAccepted);
			var rejectedDepth2Predicates = CreatePredicates(seedRejected);
			var depth2Predicates = MergePredicates(acceptedDepth2Predicates.Item1,
					rejectedDepth2Predicates.Item1);
			acceptedDepth2Predicates.Item2.AddRange(rejectedDepth2Predicates.Item2);
			return CreateLearningData(depth2Predicates, acceptedDepth2Predicates.Item2, allAccepted,
					allRejected);
		}

		private static Dictionary<int, HashSet<Predicate>> MergePredicates(
				Dictionary<int, HashSet<Predicate>> depth2Predicates,
				Dictionary<int, HashSet<Predicate>> merged) {
			foreach (var depthAndPredicates in merged) {
				var predicates = depth2Predicates.GetValueOrDefault(depthAndPredicates.Key);
				if (predicates == null) {
					depth2Predicates.Add(depthAndPredicates.Key, depthAndPredicates.Value);
				} else {
					predicates.AddRange(depthAndPredicates.Value);
				}
			}
			return depth2Predicates;
		}

		private Tuple<Dictionary<int, HashSet<Predicate>>, HashSet<SurroundingElementsPredicate>> CreatePredicates(
				ICollection<XElement> elements) {
			var depth2Predicate2Count = new Dictionary<int, Dictionary<Predicate, int>>();
			foreach (var elem in elements) {
				foreach (var predicate in PredicateGenerator.InferDepthBasedPredicate(elem, _predicateDepth)) {
					Dictionary<Predicate, int> predicates;
					if (!depth2Predicate2Count.TryGetValue(predicate.Depth, out predicates)) {
						predicates = new Dictionary<Predicate, int>();
						depth2Predicate2Count[predicate.Depth] = predicates;
					}
					predicates[predicate] = predicates.GetValueOrDefault(predicate) + 1;
				}
			}
			var depth2Predicates = new Dictionary<int, HashSet<Predicate>>();
			foreach (var depthAndPredicate2Count in depth2Predicate2Count) {
				var predicates = new HashSet<Predicate>();
				depth2Predicates[depthAndPredicate2Count.Key] = predicates;
				foreach (var predicateAndCount in depthAndPredicate2Count.Value) {
					if (predicateAndCount.Value >= 2) {
						predicates.add(predicateAndCount.Key);
					}
				}
			}
			var dict = SurroundingElementsPredicate.GetSurroundingElements(elements, _predicateDepth);
			foreach (var kv in dict) {
				kv.Value.ClearItemsIf((key, count) => count < 3);
			}
			var newPredicates = dict
					.SelectMany(kv => kv.Value
							.Select(item => new SurroundingElementsPredicate(kv.Key, item)))
					.ToHashSet();
			return Tuple.Create(depth2Predicates, newPredicates);
		}

		private LearningData CreateLearningData(
				Dictionary<int, HashSet<Predicate>> depth2Predicates,
				HashSet<SurroundingElementsPredicate> newPredicates, ICollection<XElement> allAccepted,
				ICollection<XElement> allRejected) {
			// TODO
			newPredicates = newPredicates.Where(p => p.Value.EndsWith("console") || p.Value.EndsWith("log")).ToHashSet();

			var variables = new List<DecisionVariable>();
			var count = depth2Predicates.Values.Sum(ps => ps.Count) + newPredicates.Count;
			for (int i = 0; i < count; i++) {
				variables.Add(new DecisionVariable(i.ToString(), DecisionVariableKind.Discrete));
			}

			Console.WriteLine("Accepted: " + allAccepted.Count + ", Rejected: " + allRejected.Count
			                  + ", Predicates: " + count);

			var learningRecords = Enumerable.Empty<double[]>();
			var learningResults = new List<int>();
			var learningData = new LearningData {
				Depth2Predicates = depth2Predicates,
				Variables = variables,
				NewPredicates = newPredicates,
			};
			foreach (var predicate in newPredicates) {
				Console.WriteLine(predicate);
			}
			learningRecords =
					learningRecords.Concat(
							allAccepted.Concat(allRejected)
									.Select(e => GetClassifierInput(e, learningData)));
			learningResults.AddRange(Enumerable.Repeat(-1, allAccepted.Count));
			learningResults.AddRange(Enumerable.Repeat(1, allRejected.Count));
			learningData.Inputs = learningRecords.ToArray();
			learningData.Outputs = learningResults.ToArray();
			return learningData;
		}

		private HashSet<XElement> GatherAcceptedAndRejected(
				IEnumerable<string> paths, ISet<XElement> allRejected) {
			var allAccepted = new HashSet<XElement>();
			foreach (var path in paths) {
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				var rejected = GetAllElements(ast).ToHashSet(); // Can return duplicated elements
				var accepted = GetAcceptedElements(ast).ToList(); // Can return duplicated elements
				var accptedCount = allAccepted.Count;
				allAccepted.UnionWith(accepted);
				accptedCount = allAccepted.Count - accptedCount;
				rejected.ExceptWith(accepted);
				Console.WriteLine("Accepted: " + accptedCount + ", Rejected: " + rejected.Count);
				rejected = FilterRejected(accptedCount, rejected);
				allRejected.UnionWith(rejected);
			}
			if (!NormalizeElementNames(allAccepted).SetEquals(_elementNames)) {
				Console.WriteLine("Failed to normalize element names: "
				                  + string.Join(",", NormalizeElementNames(allAccepted)));
			}
			return allAccepted;
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

		private double[] GetClassifierInput(XElement e, LearningData learningData) {
			var input = new double[learningData.Variables.Count];
			var count = 0;
			var count2 = 0;
			foreach (var depthAndPredicates in learningData.Depth2Predicates) {
				var depthInfo = new DepthInfo(e, depthAndPredicates.Key);
				foreach (var predicate in depthAndPredicates.Value) {
					input[count++] = predicate.Check(depthInfo) ? 1 : 0;
					count2 += (predicate.Check(depthInfo) ? 1 : 0);
				}
			}
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