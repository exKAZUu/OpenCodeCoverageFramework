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
using AForge;
using NUnit.Framework;
using Occf.Core.Manipulators;
using Occf.Core.Tests;
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

		[Test, TestCaseSource("TestCases")]
		public void LearnC(string fileName) {
			var profile = LanguageSupports.GetCoverageModeByClassName("C");
			var inPath = Path.Combine(Fixture.GetCoverageInputPath(), fileName);
			var codeFile = new FileInfo(inPath);
			var ast = profile.Processor.GenerateXml(codeFile);
			var statements = profile.AstAnalyzer.FindStatements(ast).ToList();
			RuleLearner.Learn(new[] { new LearningRecord(ast, statements) });
			//var statements2 = rule.Find(ast).ToList();
			//Assert.That(statements2.Count, Is.EqualTo(statements.Count));
			//Assert.That(statements2, Is.SubsetOf(statements));
		}

		[Test]
		public void TestLearningJava() {
			var names = new[] {
				"GenerateCommand.java",
				"TestCodeGenerator.java",
				"Block1.java",
				"Block2.java",
				"Block3.java",
				"Condition.java",
				"Simple.java",
			};
			var rules1 = LearnJava(names[0]);
			Console.WriteLine("------------------------------");
			var rules2 = LearnJava(names[1]);
			Console.WriteLine("------------------------------");
		}

		public IEnumerable<IFilter> LearnJava(string fileName) {
			var profile = LanguageSupports.GetCoverageModeByClassName("Java");
			var inPath = Path.Combine(Fixture.GetCoverageInputPath(), fileName);
			var codeFile = new FileInfo(inPath);
			var ast = profile.Processor.GenerateXml(codeFile);
			var accepted = profile.AstAnalyzer.FindStatements(ast).ToList();
			var rules = RuleLearner.Learn(new[] { new LearningRecord(ast, accepted) });
			return rules;
		}

		[Test]
		public void TestMachineLearning() {
			var all = new HashSet<XElement>();
			var accepted = new HashSet<XElement>();
			var inPaths = new[] {
				@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\main\java\com\google\testing\pogen\GenerateCommand.java",
				//@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\main\java\com\google\testing\pogen\generator\test\java\NameConverter.java",
				//@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\main\java\com\google\testing\pogen\Command.java",
				//@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\main\java\com\google\testing\pogen\generator\template\TemplateUpdaterWithClassAttribute.java",
				//@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\main\java\com\google\testing\pogen\parser\template\TemplateParser.java",
				//@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\main\java\com\google\testing\pogen\generator\template\TemplateUpdaterWithoutClassAttribute.java",
				//@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\main\java\com\google\testing\pogen\MeasureCommand.java",
				//@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\PageObjectGenerator - コピー.java",
				//@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\main\java\com\google\testing\pogen\parser\template\RegexVariableExtractor.java",
				//@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\main\java\com\google\testing\pogen\generator\template\TemplateUpdater.java",
				//@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\main\java\com\google\testing\pogen\PageObjectGenerator.java",
				//@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\main\java\com\google\testing\pogen\generator\template\TemplateUpdaterWithClassAttribute.java",
				//@"C:\Users\exKAZUu\Projects\pageobjectgenerator\src\main\java\com\google\testing\pogen\measurer\VariableCoverage.java",
				//@"C:\Users\exKAZUu\Projects\pageobjectgenerator\src\main\java\com\google\testing\pogen\FileProcessException.java",
				//@"C:\Users\exKAZUu\Projects\pageobjectgenerator\src\main\java\com\google\testing\pogen\PageObjectGenerator.java",
				//@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\main\java\com\google\testing\pogen\parser\template\HtmlTagInfo.java",
			};
			foreach (var inPath in inPaths) {
				var allAndAccepted = CalculateAllAndAccepted(inPath);
				all.UnionWith(allAndAccepted.Item1);
				accepted.UnionWith(allAndAccepted.Item2);
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
			learningRecords =
					learningRecords.Concat(
							accepted.Concat(denied)
									.Select(e => GetLearningRecord(e, variables, extractors, prop2Index)));
			learningResults.AddRange(Enumerable.Repeat(-1, accepted.Count));
			learningResults.AddRange(Enumerable.Repeat(1, denied.Count));
			var inputs = learningRecords.ToArray();
			var outputs = learningResults.ToArray();

			{
				var tree = new DecisionTree(variables, 2);
				var learning = new C45Learning(tree);
				learning.Run(inputs, outputs);
				Func<double[], double> judge = record => tree.Compute(record);
				VerifyLearning(judge, variables, extractors, prop2Index);
			}

			//{
			//	var tree = new DecisionTree(variables, 2);
			//	var learning = new ID3Learning(tree);
			//	learning.Run(inputs.Select(ds => ds.Select(d => (int)d).ToArray()).ToArray(), outputs);
			//	Func<double[], double> judge = record => tree.Compute(record);
			//	VerifyLearning(judge, variables, extractors, prop2Index);
			//}

			{
				DoubleRange range; // valid range will be returned as an out parameter
				var kernel = new Linear();
				var svm = new KernelSupportVectorMachine(kernel, variables.Count);
				var smo = new SequentialMinimalOptimization(svm, inputs, outputs);
				smo.Run();
				VerifyLearning(svm.Compute, variables, extractors, prop2Index);
			}

			//{
			//	DoubleRange range; // valid range will be returned as an out parameter
			//	var kernel = new Gaussian();
			//	var svm = new KernelSupportVectorMachine(kernel, variables.Count);
			//	var smo = new SequentialMinimalOptimization(svm, inputs, outputs);
			//	smo.Run();
			//	VerifyLearning(svm.Compute, variables, extractors, prop2Index);
			//}
		}

		private void VerifyLearning(
				Func<double[], double> judge, List<DecisionVariable> variables,
				IEnumerable<PropertyExtractor<string>> extractors, Dictionary<string, int> prop2Index) {
			var profile = LanguageSupports.GetCoverageModeByClassName("Java");
			var files = Directory.GetFiles(@"C:\Users\exKAZUu\Projects\PageObjectGenerator", "*.java",
					SearchOption.AllDirectories)
					//.Concat(Directory.GetFiles(@"C:\Users\exKAZUu\Projects\jenkins", "*.java",
					//		SearchOption.AllDirectories))
					//.Concat(Directory.GetFiles(@"C:\Users\exKAZUu\Projects\storm", "*.java",
					//		SearchOption.AllDirectories))
					//.Concat(Directory.GetFiles(@"C:\Users\exKAZUu\Projects\xUtils", "*.java",
					//		SearchOption.AllDirectories))
					//.Concat(Directory.GetFiles(@"C:\Users\exKAZUu\Projects\presto", "*.java",
					//		SearchOption.AllDirectories))
					;
			var count = 0;
			var failedIndicies = new List<int>();
			var minProb = Double.MaxValue;
			var minProbPath = "";
			foreach (var inPath in files) {
				//Console.WriteLine(inPath);
				var codeFile = new FileInfo(inPath);
				var ast = profile.Processor.GenerateXml(codeFile);
				var accepted = GetAcceptedElements(ast);
				var all = ast.Descendants("expression").ToHashSet();
				foreach (var e in all) {
					var record = GetLearningRecord(e, variables, extractors, prop2Index);
					var value = judge(record);
					var expected = value <= 0;
					var actual = accepted.Contains(e);
					if (expected != actual) {
						var props = extractors.Select(ext => ext.ExtractProperty(e)).ToList();
						//Console.WriteLine(count + ": expected: " + expected + "(" + value + ")" + ", actual: "
						//                  + actual);
						failedIndicies.Add(count);
					}
					var prob = Math.Abs(value);
					if (minProb > prob) {
						minProb = prob;
						minProbPath = inPath;
					}
					count++;
				}
			}
			Console.WriteLine(failedIndicies.Count + ": " + string.Join(",", failedIndicies));
			Console.WriteLine(minProb + ": " + minProbPath);
			Console.WriteLine("-------------------------------------------");
		}

		private static double[] GetLearningRecord(
				XElement e, List<DecisionVariable> variables, IEnumerable<PropertyExtractor<string>> extractors,
				Dictionary<string, int> prop2Index) {
			var record = new double[variables.Count];
			extractors.ForEach((extractor, i) => {
				var propWithIndex = i + extractor.ExtractProperty(e);
				int index;
				if (prop2Index.TryGetValue(propWithIndex, out index)) {
					record[index] = 1;
				}
			});
			return record;
		}

		private static Tuple<HashSet<XElement>, HashSet<XElement>> CalculateAllAndAccepted(string inPath) {
			var profile = LanguageSupports.GetCoverageModeByClassName("Java");
			var codeFile = new FileInfo(inPath);
			var ast = profile.Processor.GenerateXml(codeFile);
			var accepted = GetAcceptedElements(ast);
			var all = ast.Descendants("expression").ToHashSet();
			return Tuple.Create(all, accepted);
		}

		private static HashSet<XElement> GetAcceptedElements(XElement ast) {
			var profile = LanguageSupports.GetCoverageModeByClassName("Java");
			//return profile.AstAnalyzer.FindStatements(ast).ToHashSet();

			var ifConds = ast.Descendants("statement")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "if")
					.Select(e => e.NthElement(1).NthElement(1));
			var preConds = ast.Descendants("expression")
					.Where(e => {
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
			var accepted = ifConds.Concat(preConds).ToHashSet();
			return accepted;
		}
	}
}