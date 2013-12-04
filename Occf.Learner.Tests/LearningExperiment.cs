﻿using System;
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
		private HashSet<XElement> _acceptedElements;
		private HashSet<XElement> _rejectedElements;
		private int _predicateDepth = 10;
		private const int LearningCount = 10;
		private const int DeniedThreshold = 300;

		protected LearningExperiment(params string[] elementNames) {
			_elementNames = elementNames.ToHashSet();
		}

		public void LearnUntilBeStable(
				ICollection<string> allPaths, ICollection<string> seedPaths, LearningAlgorithm learner,
				double threshold) {
			_allElements = new Dictionary<string, HashSet<XElement>>();
			_acceptedElements = new HashSet<XElement>();
			foreach (var path in allPaths) {
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				_allElements.Add(path, GetAllElements(ast).ToHashSet());
				_acceptedElements.AddRange(GetAcceptedElements(ast));
			}
			_rejectedElements = _allElements.Values.SelectMany(e => e).ToHashSet();
			_rejectedElements.ExceptWith(_acceptedElements);

			

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

		private double LearnAndApply(
			IDictionary<string, HashSet<XElement>> all, HashSet<XElement> accepted, HashSet<XElement> rejected,
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

			var positiveList = new SortedList<int, XElement>();
			var negativeList = new SortedList<int, XElement>();
			foreach (var e in accepted) {
					var input = GetClassifierInput(e, learningData);
					var tuple = judge(input);
					var actual = tuple.Item1;
					var prob = tuple.Item2;
				if (actual) {
					correctlyAccepted++;
				} else {
							wronglyDenied++;
				}

			}



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

		private LearningData CreatePredicatesAndLearningData(ISet<XElement> seed, ISet<XElement> learning) {
			var seedRejected = new HashSet<XElement>();
			var allRejected = new HashSet<XElement>();
			Console.Write("Seed data: ");
			var seedAccepted = GatherAcceptedAndRejected(seedPaths, seedRejected);
			Console.Write("Learning data: ");
			var allAccepted = GatherAcceptedAndRejected(learningPaths, allRejected);
			var acceptedDepth2Predicates = CreatePredicates(seedAccepted);
			var rejectedDepth2Predicates = CreatePredicates(seedRejected);
			acceptedDepth2Predicates.AddRange(rejectedDepth2Predicates);
			return CreateLearningData(acceptedDepth2Predicates, allAccepted, allRejected);
		}

		private HashSet<SurroundingElementsPredicate> CreatePredicates(ICollection<XElement> elements) {
			var dict = SurroundingElementsPredicate.GetSurroundingElements(elements, _predicateDepth);
			foreach (var kv in dict) {
				kv.Value.ClearItemsIf((key, count) => count < 3);
			}
			var newPredicates = dict
					.SelectMany(kv => kv.Value
							.Select(item => new SurroundingElementsPredicate(kv.Key, item)))
					.ToHashSet();
			return newPredicates;
		}

		private LearningData CreateLearningData(
				HashSet<SurroundingElementsPredicate> newPredicates, ICollection<XElement> allAccepted,
				ICollection<XElement> allRejected) {
			// TODO
			newPredicates = newPredicates.Where(p => p.Value.EndsWith("console") || p.Value.EndsWith("log")).ToHashSet();

			var variables = new List<DecisionVariable>();
			var count = newPredicates.Count;
			for (int i = 0; i < count; i++) {
				variables.Add(new DecisionVariable(i.ToString(), DecisionVariableKind.Discrete));
			}

			Console.WriteLine("Accepted: " + allAccepted.Count + ", Rejected: " + allRejected.Count
			                  + ", Predicates: " + count);

			var learningRecords = Enumerable.Empty<double[]>();
			var learningResults = new List<int>();
			var learningData = new LearningData {
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

		private HashSet<XElement> GatherAcceptedAndRejected(ISet<XElement> all, ISet<XElement> rejected) {
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