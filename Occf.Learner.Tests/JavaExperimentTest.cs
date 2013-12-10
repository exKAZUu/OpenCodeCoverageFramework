﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Accord.Statistics.Kernels;
using Code2Xml.Core;
using NUnit.Framework;
using Occf.Learner.Core.Tests.LearningAlgorithms;
using Paraiba.Xml.Linq;
using ParserTests;

namespace Occf.Learner.Core.Tests {
	[TestFixture]
	public class JavaExperimentTest {
		private static IEnumerable<TestCaseData> TestCases {
			get {
				var exps = new NewLearningExperiment[] {
					//new JavaBranchExperiment(),
					new JavaIfExperiment(),
					new JavaWhileExperiment(),
					new JavaDoWhileExperiment(),
					new JavaForExperiment(),
					//new JavaPreconditionsExperiment(),
					//new JavaStatementExperiment(), 
					//new JavaBlockExperiment(),
					//new JavaLabeledStatementExperiment(), 
					//new JavaEmptyStatementExperiment(),
				};
				var algorithms = new LearningAlgorithm[] {
					new SvmLearner(new Linear()),
					//new NaiveBayesLearner(), 
					//new C45Learner(new SvmLearner(new Linear())),
				};
				const string langName = "Java";
				var learningSets = new[] {
					Tuple.Create(Fixture.GetInputProjectPath(langName, "pageobjectgenerator"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.java"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "presto"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.java"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "storm"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.java"), }),
				};
				foreach (var exp in exps) {
					foreach (var algorithm in algorithms) {
						foreach (var learningSet in learningSets) {
							yield return new TestCaseData(exp, algorithm, learningSet.Item1, learningSet.Item2);
						}
					}
				}
			}
		}

		[Test, TestCaseSource("TestCases")]
		public void Test(
				NewLearningExperiment exp, LearningAlgorithm algorithm, string projectPath, IList<string> seedPaths) {
			var allPaths = Directory.GetFiles(projectPath, "*.java", SearchOption.AllDirectories)
					.ToList();
			exp.LearnUntilBeStable(allPaths, seedPaths, algorithm, 0.5);
			Assert.That(exp.WrongCount, Is.EqualTo(0));
		}
	}

	public class JavaBranchExperiment : NewLearningExperiment {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public JavaBranchExperiment() : base("expression") {}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			// ifトークンが存在するかどうか
			var ifConds = ast.Descendants("statement")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "if")
					.Select(e => e.Element("parExpression").NthElement(1));
			var whileConds = ast.Descendants("statement")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "while")
					.Select(e => e.Element("parExpression").NthElement(1));
			var doConds =  ast.Descendants("statement")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "do")
					.Select(e => e.Element("parExpression").NthElement(1));
			var forConds = ast.Descendants("forstatement")
					.Where(e => e.Elements().Count(e2 => e2.TokenText() == ";") >= 2)
					.Select(e => e.Element("expression"));
			return ifConds.Concat(whileConds).Concat(doConds).Concat(forConds);
		}
	}

	public class JavaIfExperiment : NewLearningExperiment {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public JavaIfExperiment() : base("expression") {}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			// ifトークンが存在するかどうか
			return ast.Descendants("statement")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "if")
					.Select(e => e.Element("parExpression").NthElement(1));
		}
	}

	public class JavaWhileExperiment : NewLearningExperiment {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public JavaWhileExperiment() : base("expression") {}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			// whileトークンが存在するかどうか
			return ast.Descendants("statement")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "while")
					.Select(e => e.Element("parExpression").NthElement(1));
		}
	}

	public class JavaDoWhileExperiment : NewLearningExperiment {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public JavaDoWhileExperiment() : base("expression") {}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			// doトークンが存在するかどうか
			return ast.Descendants("statement")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "do")
					.Select(e => e.Element("parExpression").NthElement(1));
		}
	}

	public class JavaForExperiment : NewLearningExperiment {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public JavaForExperiment() : base("expression") {}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			// expressionの位置
			return ast.Descendants("forstatement")
					.Where(e => e.Elements().Count(e2 => e2.TokenText() == ";") >= 2)
					.Select(e => e.Element("expression"));
		}
	}

	public class JavaPreconditionsExperiment : NewLearningExperiment {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public JavaPreconditionsExperiment() : base("expression") {}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			// ある深さから見てトークンが存在するか？
			return ast.Descendants("expression")
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
								if (e.ElementsBeforeSelf().Any()) {
									return false;
								}
								return true;
							});
		}
	}

	public class JavaStatementExperiment : NewLearningExperiment {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public JavaStatementExperiment() : base("statement") {}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			return ast.Descendants("statement")
					.Where(e => {
						// ブロック自身は意味を持たないステートメントで、中身だけが必要なので除外
						if (e.FirstElement().Name() == "block") {
							return false;
						}
						// ラベルはループ文に付くため，ラベルの中身は除外
						var second = e.Parent.NthElementOrDefault(1);
						if (second != null && second.Value == ":"
						    && e.Parent.Name() == "statement") {
							return false;
						}
						if (e.FirstElement().Value == ";") {
							return false;
						}
						return true;
					});
		}
	}

	public class JavaBlockExperiment : NewLearningExperiment {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public JavaBlockExperiment() : base("statement") {}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			return ast.Descendants("statement")
					.Where(e => e.FirstElement().Name() == "block");
		}
	}

	public class JavaLabeledStatementExperiment : NewLearningExperiment {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public JavaLabeledStatementExperiment() : base("statement") {}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			return ast.Descendants("statement")
					.Where(e => {
						// ラベルはループ文に付くため，ラベルの中身は除外
						var second = e.Parent.NthElementOrDefault(1);
						if (second != null && second.Value == ":"
						    && e.Parent.Name() == "statement") {
							return true;
						}
						return false;
					});
		}
	}

	public class JavaEmptyStatementExperiment : NewLearningExperiment {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public JavaEmptyStatementExperiment() : base("statement") {}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			return ast.Descendants("statement")
					.Where(e => e.FirstElement().Value == ";");
		}
	}
}