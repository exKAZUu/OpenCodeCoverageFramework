#region License

// Copyright (C) 2011-2014 Kazunori Sakamoto
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
					Tuple.Create(
							Fixture.GetInputProjectPath(langName, "pageobjectgenerator"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.java"), }),
					Tuple.Create(
							Fixture.GetInputProjectPath(langName, "presto"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.java"), }),
					Tuple.Create(
							Fixture.GetInputProjectPath(langName, "storm"),
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
			exp.AutomaticallyLearnUntilBeStable(allPaths, seedPaths, _writer);
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

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
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

		public IEnumerable<XElement> SelectBooleanExpressions(XElement e) {
			var expressions = e.Descendants("expression")
					.Where(
							e_ => IsIf(e_) || IsWhile(e_) ||
							      IsDoWhile(e_) || IsFor(e_) || IsCheckArgument(e_))
					.Where(e_ => !IsChild(e_, IsCatchBlock));
			return expressions;
		}

		public bool IsCatchBlock(XElement e) {
			// catch { .. snip .. } 
			return e.Name() == "block" && e.Parent.Name() == "catchClause";
		}

		public bool IsChild(XElement e, Func<XElement, bool> isCatchBlock) {
			return e.Ancestors().Any(isCatchBlock);
		}

		public bool IsIf(XElement e) {
			// if (cond) { .. snip .. }
			XElement p = e.Parent, pp = p.Parent;
			return p.Name() == "parExpression" && pp.Name() == "statement"
			       && pp.FirstElementOrDefault().TokenText() == "if";
		}

		public bool IsWhile(XElement e) {
			// while (cond) { .. snip .. }
			XElement p = e.Parent, pp = p.Parent;
			return p.Name() == "parExpression" && pp.Name() == "statement"
			       && pp.FirstElementOrDefault().TokenText() == "while";
		}

		public bool IsDoWhile(XElement e) {
			// do { .. snip .. } while (cond);
			XElement p = e.Parent, pp = p.Parent;
			return p.Name() == "parExpression" && pp.Name() == "statement"
			       && pp.FirstElementOrDefault().TokenText() == "do";
		}

		public bool IsFor(XElement e) {
			// do { .. snip .. } while (cond);
			var p = e.Parent;
			return p.Name() == "forstatement"
			       && p.Elements().Count(e_ => e_.TokenText() == ";") >= 2;
		}

		public bool IsCheckArgument(XElement e) {
			// Preconditions.checkArgument(cond, msg);
			var primary = e.Parent.Parent.Parent.Parent;
			return primary.Name() == "primary"
			       && primary.Elements().Any(e2 => e2.TokenText() == "checkArgument")
			       && e.ElementsBeforeSelf().Count() == 0;
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

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
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

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
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

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
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

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
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

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
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

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
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

		public JavaComplexStatementExperiment() : base("statement", "blockStatement") {}

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
			if (e.Name() == "blockStatement") {
				if (e.FirstElement().Name() != "statement") {
					return true;
				}
				e = e.FirstElement();
			}

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

		/*
blockStatement 
    : localVariableDeclarationStatement
    | classOrInterfaceDeclaration
    | statement
    ;
statement 
    : block
    | ';'
    | IDENTIFIER ':' statement
	| expression ';'
    | ... (snip) ...
	;
		 */

		public IEnumerable<XElement> SelectStatements(XElement e) {
			var blockStatements = e.Descendants("blockStatement")
					.Where(e_ => IsVariableDeclaration(e_) || IsClassDeclaration(e_));
			var statements = e.Descendants("statement")
					.Where(e_ => !IsBlockStatement(e_) && !IsEmptyStatement(e_) && !IsLabeledStatement(e_));
			return blockStatements.Concat(statements);
		}

		public bool IsVariableDeclaration(XElement e) {
			// int value = 0;
			return e.FirstElement().Name() == "localVariableDeclarationStatement";
		}

		public bool IsClassDeclaration(XElement e) {
			// class A {}
			return e.FirstElement().Name() == "classOrInterfaceDeclaration";
		}

		public bool IsBlockStatement(XElement e) {
			// { int value = 0; }
			return e.FirstElement().Name() == "block";
		}

		public bool IsEmptyStatement(XElement e) {
			// ;
			return e.TokenText() == ";";
		}

		public bool IsLabeledStatement(XElement e) {
			// LABEL: should_be_ignored();
			var second = e.Parent.NthElementOrDefault(1);
			return second != null && second.Value == ":";
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

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
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

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
			return e.Name() == "statement";
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

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
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

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
			if (e.FirstElement().Value == ";") {
				return true;
			}
			return false;
		}
	}
}