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
	public class JavaExperiment {
		private readonly StreamWriter _writer = File.CreateText(@"C:\Users\exKAZUu\Desktop\java.txt");

		public static Processor Processor =
				new MemoryCachchProcessor(new FileCacheProcessor(ProcessorLoader.JavaUsingAntlr3));

		private static IEnumerable<TestCaseData> TestCases {
			get {
				var exps = new BitLearningExperimentGroupingWithId[] {
					new JavaComplexStatementExperiment(),
					new JavaComplexBranchExperiment(),
					new JavaIfExperiment(),
					new JavaWhileExperiment(),
					new JavaDoWhileExperiment(),
					new JavaForExperiment(),
					new JavaPreconditionsExperiment(),
					new JavaStatementExperiment(),
					new JavaBlockExperiment(),
					new JavaLabeledStatementExperiment(),
					new JavaEmptyStatementExperiment(),
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
					foreach (var learningSet in learningSets) {
						yield return new TestCaseData(exp, learningSet.Item1, learningSet.Item2);
					}
				}
			}
		}

		[Test, TestCaseSource("TestCases")]
		public void Test(
				BitLearningExperimentGroupingWithId exp, string projectPath, IList<string> seedPaths) {
			var allPaths = Directory.GetFiles(projectPath, "*.java", SearchOption.AllDirectories)
					.ToList();
			exp.LearnUntilBeStable(allPaths, seedPaths, _writer);
			if (exp.WrongCount > 0) {
				Console.WriteLine("--------------- WronglyAcceptedElements ---------------");
				foreach (var we in exp.WronglyAcceptedElements) {
					var e = we.AncestorsAndSelf().ElementAtOrDefault(5) ?? we;
					Console.WriteLine(we.Text());
					Console.WriteLine(e.Text());
					Console.WriteLine("---------------------------------------------");
				}
				Console.WriteLine("---- WronglyRejectedElements ----");
				foreach (var we in exp.WronglyRejectedElements) {
					var e = we.AncestorsAndSelf().ElementAtOrDefault(5) ?? we;
					Console.WriteLine(we.Text());
					Console.WriteLine(e.Text());
					Console.WriteLine("---------------------------------------------");
				}
			}
			Assert.That(exp.WrongCount, Is.EqualTo(0));
		}

		[Test, TestCaseSource("TestCases")]
		public void CheckLearnable(
				BitLearningExperimentGroupingWithId exp, string projectPath, IList<string> seedPaths) {
			var allPaths = Directory.GetFiles(projectPath, "*.java", SearchOption.AllDirectories)
					.ToList();
			//exp.CheckLearnable(allPaths, seedPaths);
		}
	}

	public class JavaSuperComplexBranchExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		public JavaSuperComplexBranchExperiment() : base("expression") {}

		protected override bool IsAccepted(XElement e) {
			var p = e.Parent;
			var pp = p.Parent;
			var isPar = p.SafeName() == "parExpression";
			var isStmt = pp.SafeName() == "statement";
			if (isStmt && isPar && pp.FirstElementOrDefault().SafeValue() == "if") {
				return true;
			}
			if (isStmt && isPar && pp.FirstElementOrDefault().SafeValue() == "while") {
				return true;
			}
			if (isStmt && isPar && pp.FirstElementOrDefault().SafeValue() == "do") {
				return true;
			}
			if (p.SafeName() == "forstatement" && p.Elements().Count(e2 => e2.TokenText() == ";") >= 2) {
				return true;
			}
			{
				var primary = e.SafeParent().SafeParent().SafeParent().SafeParent();
				if (primary.SafeName() != "primary") {
					return false;
				}
				//if (primary.Elements().All(e2 => e2.TokenText() != "Preconditions")) {
				//	return false;
				//}
				if (primary.Elements().All(e2 => e2.TokenText() != "checkArgument")) {
					return false;
				}
				//if (primary.NthElementOrDefault(0).SafeValue() != "Preconditions") {
				//	return false;
				//}
				//if (primary.NthElementOrDefault(2).SafeValue() != "checkArgument") {
				//	return false;
				//}
				if (e.ElementsBeforeSelf().Any()) {
					return false;
				}
				return true;
			}
		}
	}

	public class JavaComplexBranchExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		public JavaComplexBranchExperiment() : base("expression") {}

		protected override bool IsAccepted(XElement e) {
			var p = e.Parent;
			var pp = p.Parent;
			var isPar = p.SafeName() == "parExpression";
			var isStmt = pp.SafeName() == "statement";
			if (isStmt && isPar && pp.FirstElementOrDefault().SafeValue() == "if") {
				return true;
			}
			if (isStmt && isPar && pp.FirstElementOrDefault().SafeValue() == "while") {
				return true;
			}
			if (isStmt && isPar && pp.FirstElementOrDefault().SafeValue() == "do") {
				return true;
			}
			if (p.SafeName() == "forstatement" && p.Elements().Count(e2 => e2.TokenText() == ";") >= 2) {
				return true;
			}
			return false;
		}
	}

	public class JavaIfExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		public JavaIfExperiment() : base("expression") {}

		protected override bool IsAccepted(XElement e) {
			var p = e.Parent;
			var pp = p.Parent;
			var isPar = p.SafeName() == "parExpression";
			var isStmt = pp.SafeName() == "statement";
			if (isStmt && isPar && pp.FirstElementOrDefault().SafeValue() == "if") {
				return true;
			}
			return false;
		}
	}

	public class JavaWhileExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		public JavaWhileExperiment() : base("expression") {}

		protected override bool IsAccepted(XElement e) {
			var p = e.Parent;
			var pp = p.Parent;
			var isPar = p.SafeName() == "parExpression";
			var isStmt = pp.SafeName() == "statement";
			if (isStmt && isPar && pp.FirstElementOrDefault().SafeValue() == "while") {
				return true;
			}
			return false;
		}
	}

	public class JavaDoWhileExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		public JavaDoWhileExperiment() : base("expression") {}

		protected override bool IsAccepted(XElement e) {
			var p = e.Parent;
			var pp = p.Parent;
			var isPar = p.SafeName() == "parExpression";
			var isStmt = pp.SafeName() == "statement";
			if (isStmt && isPar && pp.FirstElementOrDefault().SafeValue() == "do") {
				return true;
			}
			return false;
		}
	}

	public class JavaForExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		public JavaForExperiment() : base("expression") {}

		protected override bool IsAccepted(XElement e) {
			var p = e.Parent;
			if (p.SafeName() == "forstatement" && p.Elements().Count(e2 => e2.TokenText() == ";") >= 2) {
				return true;
			}
			return false;
		}
	}

	public class JavaPreconditionsExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		public JavaPreconditionsExperiment() : base("expression") {}

		protected override bool IsAccepted(XElement e) {
			var primary = e.SafeParent().SafeParent().SafeParent().SafeParent();
			if (primary.SafeName() != "primary") {
				return false;
			}
			//if (primary.Elements().All(e2 => e2.TokenText() != "Preconditions")) {
			//	return false;
			//}
			if (primary.Elements().All(e2 => e2.TokenText() != "checkArgument")) {
				return false;
			}
			//if (primary.NthElementOrDefault(0).SafeValue() != "Preconditions") {
			//	return false;
			//}
			//if (primary.NthElementOrDefault(2).SafeValue() != "checkArgument") {
			//	return false;
			//}
			if (e.ElementsBeforeSelf().Any()) {
				return false;
			}
			return true;
		}
	}

	public class JavaComplexStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		public JavaComplexStatementExperiment() : base("statement") {}

		protected override bool IsAccepted(XElement e) {
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
		}
	}

	public class JavaBlockExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		public JavaBlockExperiment() : base("statement") {}

		protected override bool IsAccepted(XElement e) {
			// ブロック自身は意味を持たないステートメントで、中身だけが必要なので除外
			if (e.FirstElement().Name() == "block") {
				return true;
			}
			return false;
		}
	}

	public class JavaStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		public JavaStatementExperiment() : base("statement") {}

		protected override bool IsAccepted(XElement e) {
			return true;
		}
	}

	public class JavaLabeledStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		public JavaLabeledStatementExperiment() : base("statement") {}

		protected override bool IsAccepted(XElement e) {
			// ラベルはループ文に付くため，ラベルの中身は除外
			var second = e.Parent.NthElementOrDefault(1);
			if (second != null && second.Value == ":"
			    && e.Parent.Name() == "statement") {
				return true;
			}
			return false;
		}
	}

	public class JavaEmptyStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return JavaExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		public JavaEmptyStatementExperiment() : base("statement") {}

		protected override bool IsAccepted(XElement e) {
			if (e.FirstElement().Value == ";") {
				return true;
			}
			return false;
		}
	}
}