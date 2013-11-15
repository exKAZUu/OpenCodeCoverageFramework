#region License

// Copyright (C) 2011-2013 Kazunori Sakamoto
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Code2Xml.Core;
using Code2Xml.Languages.ANTLRv3.Core;
using NUnit.Framework;
using Paraiba.Linq;
using Paraiba.Xml.Linq;

namespace Occf.Learner.Core.Tests {
	[TestFixture]
	public class FindLearnerTest {
		private static IEnumerable<TestCaseData> TestCases {
			get {
				var names = new[] {
					"mul_mv.c",
					"mul_mv2.c",
					"mersenne.c",
					"multi.h",
					"bubblesort.c",
					"quicksort.c",
					"block1.c",
					"block2.c",
					"block3.c",
					"get_sign.c",
					"uint4.c",
				};
				return names.Select(name => new TestCaseData(name));
			}
		}

		private static IEnumerable<TestCaseData> JavaTestCases {
			get {
				var names = new[] {
					"Block1.java",
					"Block2.java",
					"Block3.java",
					"Condition.java",
					"Simple.java",
				};
				return names.Select(name => new TestCaseData(name));
			}
		}

		//[Test, TestCaseSource("TestCases")]
		//public void LearnC(string fileName) {
		//	var profile = LanguageSupports.GetCoverageModeByClassName("C");
		//	var path = Path.Combine(Fixture.GetCoverageInputPath(), fileName);
		//	var codeFile = new FileInfo(path);
		//	var ast = profile.Processor.GenerateXml(codeFile);
		//	var statements = profile.AstAnalyzer.FindStatements(ast).ToList();
		//	RuleLearner.Learn(new[] { new LearningRecord(ast, statements) });
		//	//var statements2 = rule.Find(ast).ToList();
		//	//Assert.That(statements2.Count, Is.EqualTo(statements.Count));
		//	//Assert.That(statements2, Is.SubsetOf(statements));
		//}

		//[Test]
		//public void TestLearningJava() {
		//	var names = new[] {
		//		"GenerateCommand.java",
		//		"TestCodeGenerator.java",
		//		"Block1.java",
		//		"Block2.java",
		//		"Block3.java",
		//		"Condition.java",
		//		"Simple.java",
		//	};
		//	var rules1 = LearnJava(names[0]);
		//	Console.WriteLine("------------------------------");
		//	var rules2 = LearnJava(names[1]);
		//	Console.WriteLine("------------------------------");
		//}

		//public IEnumerable<IFilter> LearnJava(string fileName) {
		//	var profile = LanguageSupports.GetCoverageModeByClassName("Java");
		//	var path = Path.Combine(Fixture.GetCoverageInputPath(), fileName);
		//	var codeFile = new FileInfo(path);
		//	var ast = profile.Processor.GenerateXml(codeFile);
		//	var accepted = profile.AstAnalyzer.FindStatements(ast).ToList();
		//	var rules = RuleLearner.Learn(new[] { new LearningRecord(ast, accepted) });
		//	return rules;
		//}

		private class LearningData {
			public List<DecisionVariable> Variables { get; set; }
			public List<ElementSequenceExtractor> Extractors { get; set; }
			public double[][] Inputs { get; set; }
			public int[] Outputs { get; set; }
			public Dictionary<string, int> Prop2Index { get; set; }
		}

		[Test]
		public void TestMachineLearning() {
			var paths = Directory.GetFiles(
					@"C:\Users\exKAZUu\Projects\PageObjectGenerator", "*.java",
					SearchOption.AllDirectories)
					//.Concat(Directory.GetFiles(@"C:\Users\exKAZUu\Projects\jenkins", "*.java",
					//		SearchOption.AllDirectories))
					.Concat(
							Directory.GetFiles(
									@"C:\Users\exKAZUu\Projects\storm", "*.java",
									SearchOption.AllDirectories))
					//.Concat(Directory.GetFiles(@"C:\Users\exKAZUu\Projects\xUtils", "*.java",
					//		SearchOption.AllDirectories))
					//.Concat(Directory.GetFiles(@"C:\Users\exKAZUu\Projects\presto", "*.java",
					//		SearchOption.AllDirectories))
					.ToList();
			var learningPaths = new[] {
				@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\main\java\com\google\testing\pogen\GenerateCommand.java",
				@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\main\java\com\google\testing\pogen\generator\test\java\TestCodeGenerator.java",
			};

			var learningPathSet = learningPaths.ToHashSet();
			Console.WriteLine("SVM with Linerar");
			const double threshold = 0.5;
			while (true) {
				var learningData = GenerateLearning(learningPathSet);
				var ret = TrySvm(paths, learningPathSet, learningData, new Linear());
				if (ret.Item1 >= threshold) {
					break;
				}
				learningPathSet.add(ret.Item2);
			}

			learningPathSet = learningPaths.ToHashSet();
			Console.WriteLine("SVM with Polynomial 1");
			while (true) {
				var learningData = GenerateLearning(learningPathSet);
				var ret = TrySvm(paths, learningPathSet, learningData, new Polynomial(1));
				if (ret.Item1 >= threshold) {
					break;
				}
				learningPathSet.add(ret.Item2);
			}

			//learningPathSet = learningPaths.ToHashSet();
			//Console.WriteLine("SVM with Polynomial 2");
			//while (true) {
			//	var learningData = GenerateLearning(learningPathSet);
			//	var ret = TrySvm(paths, learningPathSet, learningData, new Polynomial(2));
			//	if (ret.Item1 >= threshold) {
			//		break;
			//	}
			//	learningPathSet.add(ret.Item2);
			//}

			//learningPathSet = learningPaths.ToHashSet();
			//Console.WriteLine("SVM with Gaussian");
			//while (true) {
			//	var learningData = GenerateLearning(learningPathSet);
			//	var ret = TrySvm(paths, learningPathSet, learningData, new Gaussian());
			//	if (ret.Item1 >= threshold) {
			//		break;
			//	}
			//	learningPathSet.add(ret.Item2);
			//}

			//learningPathSet = learningPaths.ToHashSet();
			//Console.WriteLine("SVM with Gaussian estimated");
			//while (true) {
			//	var learningData = GenerateLearning(learningPathSet);
			//	var ret = TrySvm(paths, learningPathSet, learningData, Gaussian.Estimate(learningData.Inputs));
			//	if (ret.Item1 >= threshold) {
			//		break;
			//	}
			//	learningPathSet.add(ret.Item2);
			//}

			learningPathSet = learningPaths.ToHashSet();
			Console.WriteLine("SVM with C45");
			while (true) {
				var learningData = GenerateLearning(learningPathSet);
				TryC45Learning(paths, learningPathSet, learningData);
				var ret = TrySvm(paths, learningPathSet, learningData, new Linear());
				if (ret.Item1 >= threshold) {
					break;
				}
				learningPathSet.add(ret.Item2);
			}

			//learningPathSet = learningPaths.ToHashSet();
			//Console.WriteLine("SVM with ID3");
			//while (true) {
			//	var learningData = GenerateLearning(learningPathSet);
			//	TryID3Learning(paths, learningPathSet, learningData);
			//	var ret = TrySvm(paths, learningPathSet, learningData, new Linear());

			//	if (ret.Item1 >= threshold) {
			//		break;
			//	}
			//	learningPathSet.add(ret.Item2);
			//}
		}

		private Tuple<double, string> TryID3Learning(
				IEnumerable<string> paths, ICollection<string> learningPaths, LearningData learningData) {
			var tree = new DecisionTree(learningData.Variables, 2);
			var learning = new ID3Learning(tree);
			learning.Run(
					learningData.Inputs.Select(ds => ds.Select(d => (int)d).ToArray()).ToArray(),
					learningData.Outputs);
			Func<double[], double> judge = record => tree.Compute(record);
			var ret = VerifyLearning(paths, learningPaths, learningData, judge);
			return ret;
		}

		private Tuple<double, string> TryC45Learning(
				IEnumerable<string> paths, ICollection<string> learningPaths, LearningData learningData) {
			var tree = new DecisionTree(learningData.Variables, 2);
			var learning = new C45Learning(tree);
			learning.Run(learningData.Inputs, learningData.Outputs);
			Func<double[], double> judge = record => tree.Compute(record);
			var ret = VerifyLearning(paths, learningPaths, learningData, judge);
			return ret;
		}

		private Tuple<double, string> TrySvm(
				IEnumerable<string> paths, ICollection<string> learningPaths, LearningData learningData, IKernel kernel) {
			var svm = new KernelSupportVectorMachine(kernel, learningData.Variables.Count);
			var smo = new SequentialMinimalOptimization(svm, learningData.Inputs, learningData.Outputs);
			smo.Run();
			return VerifyLearning(paths, learningPaths, learningData, svm.Compute);
		}

		private static LearningData GenerateLearning(IEnumerable<string> paths) {
			var all = new HashSet<XElement>();
			var accepted = new HashSet<XElement>();
			foreach (var path in paths) {
				CalculateAllAndAccepted(path, all, accepted);
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

		private Tuple<double, string> VerifyLearning(
				IEnumerable<string> paths, ICollection<string> learningPaths, LearningData learningData,
				Func<double[], double> judge) {
			var count = 0;
			var failedIndicies = new List<int>();
			var minProb = Double.MaxValue;
			var minProbPath = "";
			foreach (var path in paths) {
				Console.Write(".");
				//Console.WriteLine(path);
				var codeFile = new FileInfo(path);
				var ast = ProcessorLoader.JavaUsingAntlr3.GenerateXml(codeFile);
				var accepted = GetAcceptedElements(ast).ToHashSet();
				var all = ast.Descendants("expression").ToHashSet();
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
					if (!learningPaths.Contains(path)) {
						var prob = Math.Abs(value);
						if (minProb > prob) {
							minProb = prob;
							minProbPath = path;
						}
					}
					count++;
				}
			}
			Console.WriteLine(failedIndicies.Count + ": " /*+ string.Join(",", failedIndicies)*/);
			Console.WriteLine(minProb + ": " + minProbPath);
			Console.WriteLine("-------------------------------------------");
			return Tuple.Create(minProb, minProbPath);
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

		private static void CalculateAllAndAccepted(
				string path, ISet<XElement> all, ISet<XElement> accepted) {
			var codeFile = new FileInfo(path);
			var ast = ProcessorLoader.JavaUsingAntlr3.GenerateXml(codeFile);
			all.UnionWith(ast.Descendants("expression"));
			accepted.UnionWith(GetAcceptedElements(ast));
		}

		private static IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			var ifConds = ast.Descendants("statement")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "if")
					.Select(e => e.NthElement(1).NthElement(1));
			var preConds = ast.Descendants("expression")
					.Where(
							e => {
								var primary = e.SafeParent().SafeParent().SafeParent().SafeParent();
								if (primary.SafeName() != "primary") {
									return false;
								}
								if (primary.NthElementOrDefault(0).SafeValue() != "Preconditions") {
									return false;
								}
								if (primary.NthElementOrDefault(2).SafeValue() != "checkArgument") {
									return false;
								}
								return true;
							});
			return ifConds.Concat(preConds);
		}
	}
}