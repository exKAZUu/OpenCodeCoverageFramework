using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core;
using Code2Xml.Languages.ANTLRv3.Processors.CSharp;
using NUnit.Framework;
using Paraiba.Xml.Linq;
using ParserTests;

namespace Occf.Learner.Core.Tests.Experiments {
	[TestFixture]
	public class CSharpExperiment {
		private static IEnumerable<TestCaseData> TestCases {
			get {
				var exps = new BitLearningExperimentWithGrouping[] {
					new CSharpStatementExperiment(),
					new CSharpBranchExperiment(),
					new CSharpIfExperiment(),
					new CSharpWhileExperiment(),
					new CSharpDoWhileExperiment(),
					new CSharpForExperiment(),
					new CSharpPreconditionsExperiment(),
					new CSharpBlockExperiment(),
					new CSharpLabeledStatementExperiment(),
				};
				const string langName = "CSharp";
				var learningSets = new[] {
					Tuple.Create(Fixture.GetInputProjectPath(langName, "SignalR"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.cs"), }),
				};
				foreach (var exp in exps) {
					foreach (var learningSet in learningSets) {
						yield return new TestCaseData(exp, learningSet.Item1, learningSet.Item2);
					}
				}
			}
		}

		[Test, TestCaseSource("TestCases")]
		public void Test(
				BitLearningExperimentWithGrouping exp, string projectPath, IList<string> seedPaths) {
			var code = @"
			var a = from method in new[] { 123.ToString() }
				group method by method.Length
				into overloads
				select oload;
			 ";
			//var processor = new TestProcessorUsingAntlr3();
			//var xml1 = processor.GenerateXml("class A { void main() { new A() { }; } }");

			Console.WriteLine("-----------------");

			var processor2 = new CSharpProcessorUsingAntlr3();
			var xml2 = processor2.GenerateXml("class A { void main() { " + code + "} }", true);

			var allPaths = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories)
					.ToList();
			exp.LearnUntilBeStable(allPaths, seedPaths);
			Assert.That(exp.WrongCount, Is.EqualTo(0));
		}

		[Test, TestCaseSource("TestCases")]
		public void CheckLearnable(
				BitLearningExperimentWithGrouping exp, string projectPath, IList<string> seedPaths) {
			var allPaths = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories)
					.ToList();
			exp.CheckLearnable(allPaths, seedPaths);
		}
	}

	public class CSharpBranchExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return ProcessorLoader.CSharpUsingAntlr3; }
		}

		public CSharpBranchExperiment() : base("boolean_expression") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			// ifトークンが存在するかどうか
			var ifConds = ast.Descendants("if_statement")
					.Select(e => e.Element("boolean_expression"))
					.Select(e => Tuple.Create(e, 0));
			var whileConds = ast.Descendants("while_statement")
					.Select(e => e.Element("boolean_expression"))
					.Select(e => Tuple.Create(e, 1));
			var doConds = ast.Descendants("do_statement")
					.Select(e => e.Element("boolean_expression"))
					.Select(e => Tuple.Create(e, 2));
			var forConds = ast.Descendants("for_statement")
					.Select(e => e.Element("for_condition"))
					.Where(e => e != null)
					.Select(e => e.Element("boolean_expression"))
					.Select(e => Tuple.Create(e, 3));
			return ifConds.Concat(whileConds).Concat(doConds).Concat(forConds);
		}
	}

	public class CSharpIfExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return ProcessorLoader.CSharpUsingAntlr3; }
		}

		public CSharpIfExperiment() : base("boolean_expression") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			// ifトークンが存在するかどうか
			return ast.Descendants("if_statement")
					.Select(e => e.Element("boolean_expression"))
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class CSharpWhileExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return ProcessorLoader.CSharpUsingAntlr3; }
		}

		public CSharpWhileExperiment() : base("boolean_expression") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			// whileトークンが存在するかどうか
			return ast.Descendants("while_statement")
					.Select(e => e.Element("boolean_expression"))
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class CSharpDoWhileExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return ProcessorLoader.CSharpUsingAntlr3; }
		}

		public CSharpDoWhileExperiment() : base("boolean_expression") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			// doトークンが存在するかどうか
			return ast.Descendants("do_statement")
					.Select(e => e.Element("boolean_expression"))
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class CSharpForExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return ProcessorLoader.CSharpUsingAntlr3; }
		}

		public CSharpForExperiment() : base("boolean_expression") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			// expressionの位置
			return ast.Descendants("for_statement")
					.Select(e => e.Element("for_condition"))
					.Where(e => e != null)
					.Select(e => e.Element("boolean_expression"))
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class CSharpPreconditionsExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return ProcessorLoader.CSharpUsingAntlr3; }
		}

		public CSharpPreconditionsExperiment() : base("expression") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
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
							})
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class CSharpStatementExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return ProcessorLoader.CSharpUsingAntlr3; }
		}

		public CSharpStatementExperiment() : base("statement") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			return ast.Descendants("statement")
					.Where(e => {
						// ラベルはループ文に付くため，ラベルの中身は除外
						if (e.Element("labeled_statement") != null) {
							return false;
						}

						// ブロック自身は意味を持たないステートメントで、中身だけが必要なので除外
						var e2 = e.Element("embedded_statement");
						if (e2 != null && e2.Element("block") != null) {
							return false;
						}
						return true;
					})
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class CSharpBlockExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return ProcessorLoader.CSharpUsingAntlr3; }
		}

		public CSharpBlockExperiment() : base("statement") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			return ast.Descendants("statement")
					.Where(e => {
						var e2 = e.Element("embedded_statement");
						if (e2 != null && e2.Element("block") != null) {
							return true;
						}
						return false;
					})
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class CSharpLabeledStatementExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return ProcessorLoader.CSharpUsingAntlr3; }
		}

		public CSharpLabeledStatementExperiment() : base("statement") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			return ast.Descendants("statement")
					.Where(e => e.Element("labeled_statement") != null)
					.Select(e => Tuple.Create(e, 0));
		}
	}
	class A {
		void main() {
			var a = from method in new[] { 123.ToString() }
				let oload = (from overload in new[] { 123.ToString() }
					orderby overload.Length
					select overload).FirstOrDefault()
				orderby oload.Length
				select oload;
		}
	}
}