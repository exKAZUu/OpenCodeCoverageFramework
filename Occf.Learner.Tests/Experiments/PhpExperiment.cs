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
	public class PhpExperiment {
		private readonly StreamWriter _writer = File.CreateText(@"C:\Users\exKAZUu\Desktop\php.txt");

		public static Processor Processor = ProcessorLoader.PhpUsingAntlr3;

		//new MemoryCacheCstGenerator(new FileCacheCstGenerator(ProcessorLoader.PhpUsingAntlr3));

		private static IEnumerable<TestCaseData> TestCases {
			get {
				var exps = new BitLearningExperimentGroupingWithId[] {
					new PhpComplexStatementExperiment(),
					new PhpSuperComplexBranchExperiment(),
					//new PhpComplexBranchExperiment(),
					//new PhpIfExperiment(),
					//new PhpWhileExperiment(),
					//new PhpDoWhileExperiment(),
					//new PhpForExperiment(),
					//new PhpEchoExperiment(),
					//new PhpStatementExperiment(),
					//new PhpBlockExperiment(),
					//new PhpLabeledStatementExperiment(),
					//new PhpEmptyStatementExperiment(),
				};
				const string langName = "Php";
				var learningSets = new[] {
					Tuple.Create(
							Fixture.GetInputProjectPath(langName, "bedrock"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.php"), }),
					Tuple.Create(
							Fixture.GetInputProjectPath(langName, "cockpit"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.php"), }),
					Tuple.Create(
							Fixture.GetInputProjectPath(langName, "composer-service"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.php"), }),
					Tuple.Create(
							Fixture.GetInputProjectPath(langName, "flight"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.php"), }),
					Tuple.Create(
							Fixture.GetInputProjectPath(langName, "flysystem"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.php"), }),
					Tuple.Create(
							Fixture.GetInputProjectPath(langName, "gush"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.php"), }),
					Tuple.Create(
							Fixture.GetInputProjectPath(langName, "laravel"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.php"), }),
					Tuple.Create(
							Fixture.GetInputProjectPath(langName, "phpdotenv"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.php"), }),
					Tuple.Create(
							Fixture.GetInputProjectPath(langName, "php-mvc"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.php"), }),
					Tuple.Create(
							Fixture.GetInputProjectPath(langName, "sovereign"),
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
			exp.AutomaticallyLearnUntilBeStable(allPaths, seedPaths, _writer, projectPath);
			exp.Clear();
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
			var pName = e.SafeParent().FirstElement().Name();
			if (pName == "If") {
				return true;
			}
			if (pName == "While") {
				return true;
			}
			if (pName == "Do") {
				return true;
			}
			if (e.SafeParent().Name() == "commaList" && !e.SafeParent().NextElements().Any()
			    && e.SafeParent().SafeParent().Name() == "forCondition") {
				return true;
			}
			return false;
		}
	}

	public class PhpSuperComplexBranchExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return PhpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		public PhpSuperComplexBranchExperiment() : base("expression") {}

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
			var pName = e.SafeParent().FirstElement().Name();
			if (pName == "If") {
				return true;
			}
			if (pName == "While") {
				return true;
			}
			if (pName == "Do") {
				return true;
			}
			if (e.SafeParent().Name() == "commaList" && e.SafeParent().SafeParent().Name() == "forCondition"
			    && !e.NextElements().Any()) {
				return true;
			}
			if (e.SafeParent().Name() == "commaList"
			    && e.SafeParent().SafeParent().Name() == "simpleStatement"
			    && e.SafeParent().SafeParent().FirstElement().Name() == "Echo"
			    && !e.PreviousElements().Any()) {
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
			var pName = e.SafeParent().FirstElement().Name();
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
			var pName = e.SafeParent().FirstElement().Name();
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
			var pName = e.SafeParent().FirstElement().Name();
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
			if (e.SafeParent().Name() == "commaList" && e.SafeParent().SafeParent().Name() == "forCondition"
			    && !e.NextElements().Any()) {
				return true;
			}
			return false;
		}

		public PhpForExperiment() : base("expression") {}
	}

	public class PhpEchoExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return PhpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return false; }
		}

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
			if (e.SafeParent().Name() == "commaList"
			    && e.SafeParent().SafeParent().Name() == "simpleStatement"
			    && e.SafeParent().SafeParent().FirstElement().Name() == "Echo"
			    && !e.PreviousElements().Any()) {
				return true;
			}
			return false;
		}

		public PhpEchoExperiment() : base("expression") {}
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

	public class PhpStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return PhpExperiment.Processor; }
		}

		protected override bool IsInner {
			get { return true; }
		}

		protected override bool ProtectedIsAcceptedUsingOracle(XElement e) {
			return true;
		}

		public PhpStatementExperiment() : base("statement") {}
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