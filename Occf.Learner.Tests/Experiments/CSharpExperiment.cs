using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core;
using Code2Xml.Core.Processors;
using NUnit.Framework;
using ParserTests;

namespace Occf.Learner.Core.Tests.Experiments {
	[TestFixture]
	public class CSharpExperiment {
		private readonly StreamWriter _writer = File.CreateText(@"C:\Users\exKAZUu\Desktop\cs.txt");

		public static Processor Processor =
				new MemoryCachchProcessor(new FileCacheProcessor(ProcessorLoader.CSharpUsingAntlr3));

		private static IEnumerable<TestCaseData> TestCases {
			get {
				var exps = new BitLearningExperimentGroupingWithId[] {
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
					Tuple.Create(Fixture.GetInputProjectPath(langName, "MechJeb2"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.cs"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "MediaPortal-1"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.cs"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "MonoTouch.Dialog"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.cs"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "moq4"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.cs"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "Nancy"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.cs"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "Newtonsoft.Json"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.cs"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "NuGetGallery"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.cs"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "ravendb"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.cs"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "ServiceStack"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.cs"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "SignalR"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.cs"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "StarryboundServer"),
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
				BitLearningExperimentGroupingWithId exp, string projectPath, IList<string> seedPaths) {
			var allPaths = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories)
					.ToList();
			exp.LearnUntilBeStable(allPaths, seedPaths, _writer);
			Assert.That(exp.WrongCount, Is.EqualTo(0));
		}

		[Test, TestCaseSource("TestCases")]
		public void CheckLearnable(
				BitLearningExperimentGroupingWithId exp, string projectPath, IList<string> seedPaths) {
			var allPaths = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories)
					.ToList();
			//exp.CheckLearnable(allPaths, seedPaths);
		}
	}

	public class CSharpBranchExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return CSharpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		public CSharpBranchExperiment() : base("boolean_expression") {}

		protected override bool IsAccepted(XElement e) {
			var pName = e.Parent.Name();
			if (pName == "if_statement") {
				return true;
			}
			if (pName == "while_statement") {
				return true;
			}
			if (pName == "do_statement") {
				return true;
			}
			if (pName == "for_condition") {
				return true;
			}
			return false;
		}
	}

	public class CSharpIfExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return CSharpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool IsAccepted(XElement e) {
			var pName = e.Parent.Name();
			if (pName == "if_statement") {
				return true;
			}
			return false;
		}

		public CSharpIfExperiment() : base("boolean_expression") {}
	}

	public class CSharpWhileExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return CSharpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool IsAccepted(XElement e) {
			var pName = e.Parent.Name();
			if (pName == "while_statement") {
				return true;
			}
			return false;
		}

		public CSharpWhileExperiment() : base("boolean_expression") {}
	}

	public class CSharpDoWhileExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return CSharpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool IsAccepted(XElement e) {
			var pName = e.Parent.Name();
			if (pName == "do_statement") {
				return true;
			}
			return false;
		}

		public CSharpDoWhileExperiment() : base("boolean_expression") {}
	}

	public class CSharpForExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return CSharpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool IsAccepted(XElement e) {
			var pName = e.Parent.Name();
			if (pName == "for_condition") {
				return true;
			}
			return false;
		}

		public CSharpForExperiment() : base("boolean_expression") {}
	}

	public class CSharpPreconditionsExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return CSharpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool IsAccepted(XElement e) {
			// TODO:
			var pName = e.Parent.Name();
			if (pName == "for_condition") {
				return true;
			}
			return false;
		}

		public CSharpPreconditionsExperiment() : base("expression") {}
	}

	public class CSharpStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return CSharpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		protected override bool IsAccepted(XElement e) {
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
		}

		public CSharpStatementExperiment() : base("statement") {}
	}

	public class CSharpBlockExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return CSharpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		protected override bool IsAccepted(XElement e) {
			// ブロック自身は意味を持たないステートメントで、中身だけが必要なので除外
			var e2 = e.Element("embedded_statement");
			if (e2 != null && e2.Element("block") != null) {
				return true;
			}
			return false;
		}

		public CSharpBlockExperiment() : base("statement") {}
	}

	public class CSharpLabeledStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return CSharpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		protected override bool IsAccepted(XElement e) {
			// ラベルはループ文に付くため，ラベルの中身は除外
			if (e.Element("labeled_statement") != null) {
				return true;
			}
			return false;
		}

		public CSharpLabeledStatementExperiment() : base("statement") {}
	}
}