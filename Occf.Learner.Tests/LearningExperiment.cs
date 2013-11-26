using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Accord.MachineLearning.DecisionTrees;
using Code2Xml.Core;
using Code2Xml.Languages.ANTLRv3.Core;
using Paraiba.Collections.Generic;
using Paraiba.Linq;
using Paraiba.Xml.Linq;

namespace Occf.Learner.Core.Tests {
	public abstract class LearningExperiment {
		protected abstract Processor Processor { get; }
		private readonly IList<string> _allPaths;
		private readonly ISet<string> _elementNames;
		private const int PredicateDepth = 10;
		private const int DeniedThreshold = 300;

		protected LearningExperiment(IList<string> allPaths, params string[] elementNames) {
			_allPaths = allPaths;
			_elementNames = elementNames.ToHashSet();
		}

		public void LearnUntilBeStable(
				IList<string> seedPaths, LearningAlgorithm learner, double threshold) {
			var seedPathSet = seedPaths.ToHashSet();
			Console.WriteLine(learner.Description);
			while (true) {
				var time = Environment.TickCount;
				string nextPath;
				var ret = LearnAndApply(seedPathSet, learner, out nextPath);
				if (ret >= threshold) {
					break;
				}
				seedPathSet.add(nextPath);
				Console.WriteLine("Time: " + (Environment.TickCount - time));
			}
		}

		private double LearnAndApply(
				ISet<string> seedPaths, LearningAlgorithm algorithm, out string nextPath) {
			var learningData = GenerateLearning(seedPaths);
			var judge = algorithm.Learn(learningData);

			var count = 0;
			var failedIndicies = new List<int>();
			var minProb = Double.MaxValue;
			nextPath = "";
			foreach (var path in _allPaths) {
				Console.Write(".");
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				var all = GetAllElements(ast);
				var accepted = GetAcceptedElements(ast).ToHashSet();
				foreach (var e in all) {
					var input = GetClassifierInput(e, learningData);
					var value = judge(input);
					var expected = value <= 0;
					var actual = accepted.Contains(e);
					if (expected != actual) {
						failedIndicies.Add(count);
					}
					if (!seedPaths.Contains(path)) {
						var prob = Math.Abs(value);
						if (minProb > prob) {
							minProb = prob;
							nextPath = path;
						}
					}
					count++;
				}
			}
			Console.WriteLine("done");
			Console.WriteLine(failedIndicies.Count + ": " + minProb);
			Console.WriteLine(nextPath);
			return minProb;
		}

		private LearningData GenerateLearning(IEnumerable<string> paths) {
			var allDenied = new HashSet<XElement>();
			var allAccepted = new HashSet<XElement>();
			foreach (var path in paths) {
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				var denied = GetAllElements(ast).ToHashSet();
				var accepted = GetAcceptedElements(ast).ToHashSet();
				denied.ExceptWith(accepted);
				Console.WriteLine("Accepted: " + accepted.Count + ", Denied: " + denied.Count);
				if (denied.Count > accepted.Count * DeniedThreshold) {
					var newDenied = new HashSet<XElement>();
					var rand = new Random();
					foreach (var deniedElement in denied) {
						if (rand.Next(denied.Count) < accepted.Count * DeniedThreshold) {
							newDenied.Add(deniedElement);
						}
					}
					denied = newDenied;
				}
				allDenied.UnionWith(denied);
				allAccepted.UnionWith(accepted);
			}
			if (!NormalizeElementNames(allAccepted).SetEquals(_elementNames)) {
				Console.WriteLine("Failed to normalize element names!");
			}

			var depth2Predicates = new Dictionary<int, HashSet<Predicate>>();
			foreach (var elem in allAccepted) {
				foreach (var predicate in PredicateGenerator.GeneratePredicates(elem, PredicateDepth)) {
					HashSet<Predicate> predicates;
					if (!depth2Predicates.TryGetValue(predicate.Depth, out predicates)) {
						predicates = new HashSet<Predicate>();
						depth2Predicates[predicate.Depth] = predicates;
					}
					predicates.Add(predicate);
				}
			}

			var variables = new List<DecisionVariable>();
			var count = depth2Predicates.Values.Sum(ps => ps.Count);
			for (int i = 0; i < count; i++) {
				variables.Add(new DecisionVariable(i.ToString(), DecisionVariableKind.Discrete));
			}

			Console.WriteLine("Accepted: " + allAccepted.Count + ", Denied: " + allDenied.Count
			                  + ", Predicates: " + count);

			var learningRecords = Enumerable.Empty<double[]>();
			var learningResults = new List<int>();
			var learningData = new LearningData {
				Depth2Predicates = depth2Predicates,
				Variables = variables,
			};
			learningRecords =
					learningRecords.Concat(
							allAccepted.Concat(allDenied)
									.Select(e => GetClassifierInput(e, learningData)));
			learningResults.AddRange(Enumerable.Repeat(-1, allAccepted.Count));
			learningResults.AddRange(Enumerable.Repeat(1, allDenied.Count));
			learningData.Inputs = learningRecords.ToArray();
			learningData.Outputs = learningResults.ToArray();
			return learningData;
		}

		private static double[] GetClassifierInput(XElement e, LearningData learningData) {
			var input = new double[learningData.Variables.Count];
			var count = 0;
			foreach (var depthAndPredicates in learningData.Depth2Predicates) {
				var depthInfo = new DepthInfo(e, depthAndPredicates.Key);
				foreach (var predicate in depthAndPredicates.Value) {
					input[count++] = predicate.Check(depthInfo) ? 1 : 0;
				}
			}
			return input;
		}

		private ISet<string> NormalizeElementNames(ICollection<XElement> accepted) {
			var nameSet = new HashSet<string>();
			var name2Count = new Dictionary<string, int>();
			var elements = Enumerable.SelectMany(accepted,
					e => e.AncestorsAndDescendantsWithNoSiblingAndSelf());
			foreach (var element in elements) {
				int count = 0;
				name2Count.TryGetValue(element.Name(), out count);
				name2Count[element.Name()] = count + 1;
			}
			foreach (var element in accepted) {
				var newElement =
						Enumerable.First(Enumerable.OrderByDescending(element.DescendantsOfOnlyChildAndSelf(),
								e => name2Count[e.Name()]));
				nameSet.add(newElement.Name());
			}
			return nameSet;
		}

		protected IEnumerable<XElement> GetAllElements(XElement ast) {
			return Enumerable.Where(ast.DescendantsAndSelf(), e => _elementNames.Contains(e.Name()));
		}

		protected abstract IEnumerable<XElement> GetAcceptedElements(XElement ast);
	}

	public static class Extension {
		public static IEnumerable<XElement> AncestorsAndDescendantsWithNoSibling(this XElement element) {
			return Enumerable.Concat(element.DescendantsOfOnlyChild(), element.AncestorsWithNoSibling());
		}

		public static IEnumerable<XElement> AncestorsAndDescendantsWithNoSiblingAndSelf(
				this XElement element) {
			return Enumerable.Concat(element.DescendantsOfOnlyChildAndSelf(),
					element.AncestorsWithNoSibling());
		}

		public static IEnumerable<XElement> AncestorsWithNoSiblingAndSelf(this XElement element) {
			do {
				yield return element;
				element = element.Parent;
			} while (element != null && Enumerable.Count(element.Elements()) == 1);
		}

		public static IEnumerable<XElement> AncestorsWithNoSibling(this XElement element) {
			element = element.Parent;
			while (element != null && Enumerable.Count(element.Elements()) == 1) {
				yield return element;
				element = element.Parent;
			}
		}

		public static IEnumerable<XElement> DescendantsWithNoSiblingAndSelf(this XElement element) {
			yield return element;
			while (Enumerable.Count(element.Elements()) == 1) {
				element = element.FirstElement();
				yield return element;
			}
		}

		public static IEnumerable<XElement> DescendantsWithNoSibling(this XElement element) {
			while (Enumerable.Count(element.Elements()) == 1) {
				element = element.FirstElement();
				yield return element;
			}
		}
	}
}