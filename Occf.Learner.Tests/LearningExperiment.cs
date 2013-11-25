using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Accord.MachineLearning.DecisionTrees;
using Code2Xml.Core;
using Code2Xml.Languages.ANTLRv3.Core;
using Paraiba.Linq;
using Paraiba.Xml.Linq;

namespace Occf.Learner.Core.Tests {
	public abstract class LearningExperiment {
		protected abstract Processor Processor { get; }
		private readonly IList<string> _allPaths;
		private readonly ISet<string> _elementNames;

		protected LearningExperiment(IList<string> allPaths, params string[] elementNames) {
			_allPaths = allPaths;
			_elementNames = elementNames.ToHashSet();
		}

		public void LearnUntilBeStable(
				IList<string> seedPaths, LearningAlgorithm learner, double threshold) {
			var seedPathSet = seedPaths.ToHashSet();
			Console.WriteLine(learner.Description);
			while (true) {
				string nextPath;
				var ret = LearnAndApply(seedPathSet, learner, out nextPath);
				if (ret >= threshold) {
					break;
				}
				seedPathSet.add(nextPath);
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
					var record = GetLearningRecord(e, learningData);
					var value = judge(record);
					var expected = value <= 0;
					var actual = accepted.Contains(e);
					if (expected != actual) {
						var props = learningData.Extractors.Select(ext => ext.ExtractProperty(e)).ToList();
						//Console.WriteLine(count + ": expected: " + expected + "(" + value + ")" + ", actual: "
						//                  + actual);
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
			var all = new HashSet<XElement>();
			var accepted = new HashSet<XElement>();
			foreach (var path in paths) {
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				all.UnionWith(GetAllElements(ast));
				accepted.UnionWith(GetAcceptedElements(ast));
			}
			if (!NormalizeElementNames(accepted).SetEquals(_elementNames)) {
				Console.WriteLine("Failed to normalize element names!");
			}

			var prop2Index = new Dictionary<string, int>();
			var variables = new List<DecisionVariable>();
			var extractors = Enumerable.Range(-10, 21)
					.Select(i => new ElementSequenceExtractor(i))
					.ToList();
			for (int i = 0; i < extractors.Count; i++) {
				foreach (var elem in accepted) {
					var propWithIndex = i + extractors[i].ExtractProperty(elem);
					if (!prop2Index.ContainsKey(propWithIndex)) {
						prop2Index[propWithIndex] = variables.Count;
						variables.Add(new DecisionVariable(propWithIndex, DecisionVariableKind.Discrete));
					}
				}
			}
			var denied = all; // Should not use all after this
			denied.ExceptWith(accepted);

			var learningRecords = Enumerable.Empty<double[]>();
			var learningResults = new List<int>();
			var learningData = new LearningData {
				Extractors = extractors,
				Variables = variables,
				Prop2Index = prop2Index,
			};
			learningRecords =
					learningRecords.Concat(
							accepted.Concat(denied)
									.Select(e => GetLearningRecord(e, learningData)));
			learningResults.AddRange(Enumerable.Repeat(-1, accepted.Count));
			learningResults.AddRange(Enumerable.Repeat(1, denied.Count));
			learningData.Inputs = learningRecords.ToArray();
			learningData.Outputs = learningResults.ToArray();
			return learningData;
		}

		private static double[] GetLearningRecord(XElement e, LearningData learningData) {
			var record = new double[learningData.Variables.Count];
			learningData.Extractors.ForEach(
					(extractor, i) => {
						var propWithIndex = i + extractor.ExtractProperty(e);
						int index;
						if (learningData.Prop2Index.TryGetValue(propWithIndex, out index)) {
							record[index] = 1;
						}
					});
			return record;
		}

		private ISet<string> NormalizeElementNames(ICollection<XElement> accepted) {
			var nameSet = new HashSet<string>();
			var name2Count = new Dictionary<string, int>();
			var elements = accepted
					.SelectMany(e => e.AncestorsAndDescendantsWithNoSiblingAndSelf());
			foreach (var element in elements) {
				int count = 0;
				name2Count.TryGetValue(element.Name(), out count);
				name2Count[element.Name()] = count + 1;
			}
			foreach (var element in accepted) {
				var newElement = element.DescendantsOfOnlyChildAndSelf()
						.OrderByDescending(e => name2Count[e.Name()])
						.First();
				nameSet.add(newElement.Name());
			}
			return nameSet;
		}

		protected IEnumerable<XElement> GetAllElements(XElement ast) {
			return ast.DescendantsAndSelf()
					.Where(e => _elementNames.Contains(e.Name()));
		}

		protected abstract IEnumerable<XElement> GetAcceptedElements(XElement ast);
	}

	public static class Extension {
		public static IEnumerable<XElement> AncestorsAndDescendantsWithNoSibling(this XElement element) {
			return element.DescendantsOfOnlyChild().Concat(element.AncestorsWithNoSibling());
		}

		public static IEnumerable<XElement> AncestorsAndDescendantsWithNoSiblingAndSelf(
				this XElement element) {
			return element.DescendantsOfOnlyChildAndSelf().Concat(element.AncestorsWithNoSibling());
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