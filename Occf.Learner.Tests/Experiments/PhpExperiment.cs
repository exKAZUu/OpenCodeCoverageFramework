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
	public class PhpExperiment {
		private readonly StreamWriter _writer = File.CreateText(@"C:\Users\exKAZUu\Desktop\php.txt");

		public static Processor Processor =
				new MemoryCachchProcessor(new FileCacheProcessor(ProcessorLoader.PhpUsingAntlr3));

		private static IEnumerable<TestCaseData> TestCases {
			get {
				var exps = new BitLearningExperimentGroupingWithId[] {
					new PhpComplexStatementExperiment(),
					new PhpComplexBranchExperiment(),
					new PhpIfExperiment(),
					new PhpWhileExperiment(),
					new PhpDoWhileExperiment(),
					new PhpForExperiment(),
					new PhpPreconditionsExperiment(),
					new PhpBlockExperiment(),
					new PhpLabeledStatementExperiment(),
					new PhpEmptyStatementExperiment(),
				};
				const string langName = "Php";
				var learningSets = new[] {
					Tuple.Create(Fixture.GetInputProjectPath(langName, "cockpit"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.php"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "composer-service"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.php"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "php-mvc"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.php"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "sovereign"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.php"), }),
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
			var allPaths = Directory.GetFiles(projectPath, "*.php", SearchOption.AllDirectories)
					.ToList();
			exp.AutomaticallyLearnUntilBeStable(allPaths, seedPaths, _writer);
			Assert.That(exp.WrongCount, Is.EqualTo(0));
		}

		[Test, TestCaseSource("TestCases")]
		public void CheckLearnable(
				BitLearningExperimentGroupingWithId exp, string projectPath, IList<string> seedPaths) {
			var allPaths = Directory.GetFiles(projectPath, "*.php", SearchOption.AllDirectories)
					.ToList();
			//exp.CheckLearnable(allPaths, seedPaths);
		}
	}

	public class PhpComplexBranchExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return PhpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		public PhpComplexBranchExperiment() : base("expression") {}

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
			var pName = e.Parent.FirstElement().Name();
			if (pName == "If") {
				return true;
			}
			if (pName == "While") {
				return true;
			}
			if (pName == "Do") {
				return true;
			}
			if (e.Parent.Name() == "commaList" && e.Parent.NextElement() == null
			    && e.Parent.Parent.Name() == "forCondition") {
				return true;
			}
			return false;
		}
	}

	public class PhpIfExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return PhpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
			var pName = e.Parent.FirstElement().Name();
			if (pName == "If") {
				return true;
			}
			return false;
		}

		public PhpIfExperiment() : base("expression") {}
	}

	public class PhpWhileExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return PhpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
			var pName = e.Parent.Name();
			if (pName == "While") {
				return true;
			}
			return false;
		}

		public PhpWhileExperiment() : base("expression") {}
	}

	public class PhpDoWhileExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return PhpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
			var pName = e.Parent.Name();
			if (pName == "Do") {
				return true;
			}
			return false;
		}

		public PhpDoWhileExperiment() : base("expression") {}
	}

	public class PhpForExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return PhpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
			var pName = e.Parent.Name();
			if (e.Parent.Name() == "commaList" && e.Parent.NextElement() == null
			    && e.Parent.Parent.Name() == "forCondition") {
				return true;
			}
			return false;
		}

		public PhpForExperiment() : base("expression") {}
	}

	public class PhpPreconditionsExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return PhpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
			// TODO:
			var pName = e.Parent.Name();
			if (pName == "for_condition") {
				return true;
			}
			return false;
		}

		public PhpPreconditionsExperiment() : base("expression") {}
	}

	public class PhpComplexStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return PhpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
			// ラベルはループ文に付くため，ラベルの中身は除外
			if (e.FirstElement().Name() != "UnquotedString") {
				return false;
			}
			// ブロック自身は意味を持たないステートメントで、中身だけが必要なので除外
			if (e.Element("bracketedBlock") != null) {
				return false;
			}
			// 空文
			if (e.FirstElement().TokenText() == ";") {
				return false;
			}
			return true;
		}

		public PhpComplexStatementExperiment() : base("statement") {}
	}

	public class PhpBlockExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return PhpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
			// ブロック自身は意味を持たないステートメントで、中身だけが必要なので除外
			if (e.Element("bracketedBlock") != null) {
				return true;
			}
			return false;
		}

		public PhpBlockExperiment() : base("statement") {}
	}

	public class PhpLabeledStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return PhpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
			// ラベルはループ文に付くため，ラベルの中身は除外
			if (e.FirstElement().Name() != "UnquotedString") {
				return true;
			}
			return false;
		}

		public PhpLabeledStatementExperiment() : base("statement") {}
	}

	public class PhpEmptyStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return PhpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
			// 空文
			if (e.FirstElement().TokenText() == ";") {
				return true;
			}
			return false;
		}

		public PhpEmptyStatementExperiment() : base("statement") {}
	}
}