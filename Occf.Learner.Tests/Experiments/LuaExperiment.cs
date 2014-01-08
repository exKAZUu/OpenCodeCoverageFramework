using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core;
using Code2Xml.Languages.ANTLRv4.Processors.Lua;
using NUnit.Framework;
using Paraiba.Xml.Linq;
using ParserTests;

namespace Occf.Learner.Core.Tests.Experiments {
	[TestFixture]
	public class LuaExperiment {
		private static IEnumerable<TestCaseData> TestCases {
			get {
				var exps = new BitLearningExperimentGroupingWithId[] {
					new LuaComplexStatementExperiment(),
					new LuaComplexBranchExperiment(),
					new LuaStatementExperiment(),
					new LuaIfExperiment(),
					new LuaWhileExperiment(),
					new LuaDoWhileExperiment(),
					new LuaLabeledStatementExperiment(),
					new LuaEmptyStatementExperiment(),
				};
				const string langName = "Lua";
				var learningSets = new[] {
					Tuple.Create(Fixture.GetInputProjectPath(langName, "koreader"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.Lua"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "lapis"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.Lua"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "lsyncd"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.Lua"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "luafun"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.Lua"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "luakit"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.Lua"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "middleclass"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.Lua"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "pacpac"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.Lua"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "Penlight"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.Lua"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "Tir"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.Lua"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "vlsub"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.Lua"), }),
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
			var allPaths = Directory.GetFiles(projectPath, "*.lua", SearchOption.AllDirectories)
					.ToList();
			exp.LearnUntilBeStable(allPaths, seedPaths);
			Assert.That(exp.WrongCount, Is.EqualTo(0));
		}

		[Test, TestCaseSource("TestCases")]
		public void CheckLearnable(
				BitLearningExperimentGroupingWithId exp, string projectPath, IList<string> seedPaths) {
			var allPaths = Directory.GetFiles(projectPath, "*.lua", SearchOption.AllDirectories)
					.ToList();
			//exp.CheckLearnable(allPaths, seedPaths);
		}
	}

	public class LuaComplexBranchExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return new LuaProcessor(); }
		}

		public LuaComplexBranchExperiment() : base("exp") {}

		protected override bool IsAccepted(XElement e) {
			var siblings = e.Siblings().ToList();
			var parent = e.Parent;
			if (parent.SafeName() == "stat" && siblings[0].Value == "if") {
				return true;
			}
			if (parent.SafeName() == "stat" && siblings[0].Value == "while") {
				return true;
			}
			if (parent.SafeName() == "stat" && siblings[0].Value == "repeat") {
				return true;
			}
			return false;
		}
	}

	public class LuaIfExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return new LuaProcessor(); }
		}

		public LuaIfExperiment() : base("exp") {}

		protected override bool IsAccepted(XElement e) {
			var siblings = e.Siblings().ToList();
			var parent = e.Parent;
			if (parent.SafeName() == "stat" && siblings[0].Value == "if") {
				return true;
			}
			return false;
		}
	}

	public class LuaWhileExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return new LuaProcessor(); }
		}

		public LuaWhileExperiment() : base("exp") {}

		protected override bool IsAccepted(XElement e) {
			var siblings = e.Siblings().ToList();
			var parent = e.Parent;
			if (parent.SafeName() == "stat" && siblings[0].Value == "while") {
				return true;
			}
			return false;
		}
	}

	public class LuaDoWhileExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return new LuaProcessor(); }
		}

		public LuaDoWhileExperiment() : base("exp") {}

		protected override bool IsAccepted(XElement e) {
			var siblings = e.Siblings().ToList();
			var parent = e.Parent;
			if (parent.SafeName() == "stat" && siblings[0].Value == "repeat") {
				return true;
			}
			return false;
		}
	}

	public class LuaComplexStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return new LuaProcessor(); }
		}

		public LuaComplexStatementExperiment() : base("stat") {}

		protected override bool IsAccepted(XElement e) {
			if (e.FirstElement().Name() == "label") {
				return false;
			}
			if (e.FirstElement().Value == ";") {
				return false;
			}
			return true;
		}
	}

	public class LuaStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return new LuaProcessor(); }
		}

		public LuaStatementExperiment() : base("stat") {}

		protected override bool IsAccepted(XElement e) {
			return true;
		}
	}

	public class LuaLabeledStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return new LuaProcessor(); }
		}

		public LuaLabeledStatementExperiment() : base("stat") {}

		protected override bool IsAccepted(XElement e) {
			if (e.FirstElement().Name() == "label") {
				return true;
			}
			return false;
		}
	}

	public class LuaEmptyStatementExperiment : BitLearningExperimentGroupingWithId {
		protected override Processor Processor {
			get { return new LuaProcessor(); }
		}

		public LuaEmptyStatementExperiment() : base("stat") {}

		protected override bool IsAccepted(XElement e) {
			if (e.FirstElement().Value == ";") {
				return true;
			}
			return false;
		}
	}
}