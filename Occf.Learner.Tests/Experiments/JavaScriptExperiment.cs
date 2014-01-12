using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core;
using Code2Xml.Core.Processors;
using NUnit.Framework;
using Paraiba.Xml.Linq;
using ParserTests;

namespace Occf.Learner.Core.Tests.Experiments {
	[TestFixture]
	public class JavaScriptExperiment {
		private readonly StreamWriter _writer = File.CreateText(@"C:\Users\exKAZUu\Desktop\javascript.txt");

		public static Processor Processor =
				new MemoryCachchProcessor(new FileCacheProcessor(ProcessorLoader.JavaScriptUsingAntlr3));

		private static IEnumerable<TestCaseData> TestCases {
			get {
				var exps = new BitLearningExperimentGroupingWithId[] {
					new JavaScriptComplexStatementExperiment(),
					new JavaScriptSuperComplexBranchExperiment(),
					new JavaScriptComplexBranchExperiment(),
					new JavaScriptIfExperiment(),
					new JavaScriptWhileExperiment(),
					new JavaScriptDoWhileExperiment(),
					new JavaScriptForExperiment(),
					new JavaScriptConsoleLogExperiment(),
					new JavaScriptStatementExperiment(),
					new JavaScriptBlockExperiment(),
					new JavaScriptLabeledStatementExperiment(),
					new JavaScriptEmptyStatementExperiment(),
				};
				const string langName = "JavaScript";
				var learningSets = new[] {
					Tuple.Create(Fixture.GetInputProjectPath(langName, "cheet.js"),
							new List<string> { Fixture.GetInputCodePath(langName, "seed.js"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "ionic"),
							new List<string> { Fixture.GetInputCodePath(langName, "seed.js"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "clmtrackr"),
							new List<string> { Fixture.GetInputCodePath(langName, "seed.js"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "gulp"),
							new List<string> { Fixture.GetInputCodePath(langName, "seed.js"), }),
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
				BitLearningExperimentGroupingWithId exp, string projectPath, IList<string> seedPaths) {
			var allPaths = Directory.GetFiles(projectPath, "*.js", SearchOption.AllDirectories)
					.ToList();
			exp.LearnUntilBeStable(allPaths, seedPaths, _writer);
			Assert.That(exp.WrongCount, Is.EqualTo(0));
		}
	}

	public class JavaScriptSuperComplexBranchExperiment : BitLearningExperimentGroupingWithId {
		public JavaScriptSuperComplexBranchExperiment() : base("expression") {}

		protected override Processor Processor {
			get { return JavaScriptExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool IsAccepted(XElement e) {
			var parentName = e.Parent.SafeName();
			if (parentName == "ifStatement") {
				return true;
			}
			if (parentName == "whileStatement") {
				return true;
			}
			if (parentName == "doWhileStatement") {
				return true;
			}
			if (parentName == "forStatement"
			    && e.PreviousElement() == e.Parent.Elements().First(e2 => e2.TokenText() == ";")) {
				return true;
			}
			var p = e.SafeParent().SafeParent();
			if (p.SafeName() == "callExpression" && p.FirstElement().Value == "console.log" &&
			    p.Element("arguments").Element("assignmentExpression") == e) {
				return true;
			}
			return false;
		}
	}

	public class JavaScriptComplexBranchExperiment : BitLearningExperimentGroupingWithId {
		public JavaScriptComplexBranchExperiment() : base("expression") {}

		protected override Processor Processor {
			get { return JavaScriptExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool IsAccepted(XElement e) {
			var parentName = e.Parent.SafeName();
			if (parentName == "ifStatement") {
				return true;
			}
			if (parentName == "whileStatement") {
				return true;
			}
			if (parentName == "doWhileStatement") {
				return true;
			}
			if (parentName == "forStatement"
			    && e.PreviousElement() == e.Parent.Elements().First(e2 => e2.TokenText() == ";")) {
				return true;
			}
			return false;
		}
	}

	public class JavaScriptIfExperiment : BitLearningExperimentGroupingWithId {
		public JavaScriptIfExperiment() : base("expression") {}

		protected override Processor Processor {
			get { return JavaScriptExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool IsAccepted(XElement e) {
			var parentName = e.Parent.SafeName();
			if (parentName == "ifStatement") {
				return true;
			}
			return false;
		}
	}

	public class JavaScriptWhileExperiment : BitLearningExperimentGroupingWithId {
		public JavaScriptWhileExperiment() : base("expression") {}

		protected override Processor Processor {
			get { return JavaScriptExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool IsAccepted(XElement e) {
			var parentName = e.Parent.SafeName();
			if (parentName == "whileStatement") {
				return true;
			}
			return false;
		}
	}

	public class JavaScriptDoWhileExperiment : BitLearningExperimentGroupingWithId {
		public JavaScriptDoWhileExperiment() : base("expression") {}

		protected override Processor Processor {
			get { return JavaScriptExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool IsAccepted(XElement e) {
			var parentName = e.Parent.SafeName();
			if (parentName == "doWhileStatement") {
				return true;
			}
			return false;
		}
	}

	public class JavaScriptForExperiment : BitLearningExperimentGroupingWithId {
		public JavaScriptForExperiment() : base("expression") {}

		protected override Processor Processor {
			get { return JavaScriptExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool IsAccepted(XElement e) {
			var parentName = e.Parent.SafeName();
			if (parentName == "forStatement"
			    && e.PreviousElement() == e.Parent.Elements().First(e2 => e2.TokenText() == ";")) {
				return true;
			}
			return false;
		}
	}

	public class JavaScriptConsoleLogExperiment : BitLearningExperimentGroupingWithId {
		public JavaScriptConsoleLogExperiment() : base("assignmentExpression") {}

		protected override Processor Processor {
			get { return JavaScriptExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool IsAccepted(XElement e) {
			var p = e.SafeParent().SafeParent();
			if (p.SafeName() == "callExpression" && p.FirstElement().Value == "console.log" &&
			    p.Element("arguments").Element("assignmentExpression") == e) {
				return true;
			}
			return false;
		}
	}

	public class JavaScriptStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaScriptExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		public JavaScriptStatementExperiment() : base("statement") {}

		protected override bool IsAccepted(XElement e) {
			return true;
		}
	}

	public class JavaScriptComplexStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaScriptExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		public JavaScriptComplexStatementExperiment() : base("statement") {}

		protected override bool IsAccepted(XElement e) {
			if (e.FirstElement().Name() == "statementBlock") {
				return false;
			}
			if (e.FirstElement().Name() == "labelledStatement") {
				return false;
			}
			if (e.FirstElement().Name() == "emptyStatement") {
				return false;
			}
			return true;
		}
	}

	public class JavaScriptBlockExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaScriptExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		public JavaScriptBlockExperiment() : base("statement") {}

		protected override bool IsAccepted(XElement e) {
			if (e.FirstElement().Name() == "statementBlock") {
				return true;
			}
			return false;
		}
	}

	public class JavaScriptLabeledStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaScriptExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		public JavaScriptLabeledStatementExperiment() : base("statement") {}

		protected override bool IsAccepted(XElement e) {
			if (e.FirstElement().Name() == "labelledStatement") {
				return true;
			}
			return false;
		}
	}

	public class JavaScriptEmptyStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaScriptExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		public JavaScriptEmptyStatementExperiment() : base("statement") {}

		protected override bool IsAccepted(XElement e) {
			if (e.FirstElement().Name() == "emptyStatement") {
				return true;
			}
			return false;
		}
	}
}