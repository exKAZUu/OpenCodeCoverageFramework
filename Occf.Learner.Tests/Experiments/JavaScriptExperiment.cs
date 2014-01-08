using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core;
using NUnit.Framework;
using Paraiba.Xml.Linq;
using ParserTests;

namespace Occf.Learner.Core.Tests.Experiments {
	[TestFixture]
	public class JavaScriptExperiment {
		private static IEnumerable<TestCaseData> TestCases {
			get {
				var exps = new BitLearningExperimentWithGrouping[] {
					//new JavaScriptBranchExperiment(),
					//new JavaScriptIfExperiment(),
					//new JavaScriptWhileExperiment(),
					//new JavaScriptDoWhileExperiment(),
					//new JavaScriptForExperiment(),
					new JavaScriptConsoleLogExperiment(),
					//new JavaScriptBlockExperiment(),
					//new JavaScriptLabeledStatementExperiment(),
					//new JavaScriptEmptyStatementExperiment(),
				};
				const string langName = "JavaScript";
				var learningSets = new[] {
					Tuple.Create(Fixture.GetInputProjectPath(langName, "cheet.js"),
							new List<string> { Fixture.GetInputCodePath(langName, "seed.js"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "ionic"),
							new List<string> { Fixture.GetInputCodePath(langName, "seed.js"), }),
					//Tuple.Create(Fixture.GetInputProjectPath(langName, "reportr"),
					//		new List<string> { Fixture.GetInputCodePath(langName, "seed.js"), }),
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
			var allPaths = Directory.GetFiles(projectPath, "*.js", SearchOption.AllDirectories)
					.ToList();
			exp.LearnUntilBeStable(allPaths, seedPaths);
			Assert.That(exp.WrongCount, Is.EqualTo(0));
		}
	}

	public class JavaScriptBranchExperiment : BitLearningExperimentWithGrouping {
		public JavaScriptBranchExperiment() : base("expression") {}

		protected override Processor Processor {
			get { return ProcessorLoader.JavaScriptUsingAntlr3; }
		}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			var ifConds = ast.Descendants("ifStatement")
					.Select(e => e.Element("expression"))
					.Select(e => Tuple.Create(e, 0));
			var whileConds = ast.Descendants("whileStatement")
					.Select(e => e.Element("expression"))
					.Select(e => Tuple.Create(e, 1));
			var doWhileConds = ast.Descendants("doWhileStatement")
					.Select(e => e.Element("expression"))
					.Select(e => Tuple.Create(e, 2));
			var forConds = ast.Descendants("forStatement")
					.Select(e => e.Elements().First(e2 => e2.TokenText() == ";"))
					.Where(e => e.NextElement().Name() == "expression")
					.Select(e => e.NextElement())
					.Select(e => Tuple.Create(e, 3));
			return ifConds.Concat(whileConds).Concat(doWhileConds).Concat(forConds);
		}
	}

	public class JavaScriptIfExperiment : BitLearningExperimentWithGrouping {
		public JavaScriptIfExperiment() : base("expression") {}

		protected override Processor Processor {
			get { return ProcessorLoader.JavaScriptUsingAntlr3; }
		}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			return ast.Descendants("ifStatement")
					.Select(e => e.Element("expression"))
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class JavaScriptWhileExperiment : BitLearningExperimentWithGrouping {
		public JavaScriptWhileExperiment() : base("expression") {}

		protected override Processor Processor {
			get { return ProcessorLoader.JavaScriptUsingAntlr3; }
		}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			return ast.Descendants("whileStatement")
					.Select(e => e.Element("expression"))
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class JavaScriptDoWhileExperiment : BitLearningExperimentWithGrouping {
		public JavaScriptDoWhileExperiment() : base("expression") {}

		protected override Processor Processor {
			get { return ProcessorLoader.JavaScriptUsingAntlr3; }
		}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			return ast.Descendants("doWhileStatement")
					.Select(e => e.Element("expression"))
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class JavaScriptForExperiment : BitLearningExperimentWithGrouping {
		public JavaScriptForExperiment() : base("expression") {}

		protected override Processor Processor {
			get { return ProcessorLoader.JavaScriptUsingAntlr3; }
		}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			return ast.Descendants("forStatement")
					.Select(e => e.Elements().First(e2 => e2.TokenText() == ";"))
					.Where(e => e.NextElement().Name() == "expression")
					.Select(e => e.NextElement())
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class JavaScriptConsoleLogExperiment : BitLearningExperimentWithGrouping {
		public JavaScriptConsoleLogExperiment() : base("assignmentExpression") {}

		protected override Processor Processor {
			get { return ProcessorLoader.JavaScriptUsingAntlr3; }
		}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			var preConds = ast.Descendants("callExpression")
					.Where(e => e.FirstElement().Value == "console.log")
					.Select(e => e.Element("arguments").Element("assignmentExpression"))
					.Where(e => e != null)
					.Select(e => Tuple.Create(e, 0));
			return preConds;
		}
	}

	public class JavaScriptBlockExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaScriptUsingAntlr3; }
		}

		public JavaScriptBlockExperiment() : base("statement") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			return ast.Descendants("statement")
					.Where(e => e.FirstElement().Name() == "statementBlock")
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class JavaScriptLabeledStatementExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaScriptUsingAntlr3; }
		}

		public JavaScriptLabeledStatementExperiment() : base("statement") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			return ast.Descendants("statement")
					.Where(e => e.FirstElement().Name() == "labelledStatement")
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class JavaScriptEmptyStatementExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaScriptUsingAntlr3; }
		}

		public JavaScriptEmptyStatementExperiment() : base("statement") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			return ast.Descendants("statement")
					.Where(e => e.FirstElement().Name() == "emptyStatement")
					.Select(e => Tuple.Create(e, 0));
		}
	}
}