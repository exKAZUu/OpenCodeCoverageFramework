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
				var exps = new BitLearningExperimentWithGrouping[] {
					new LuaStatementExperiment(),
					new LuaBranchExperiment(),
					new LuaIfExperiment(),
					new LuaWhileExperiment(),
					new LuaDoWhileExperiment(),
					new LuaLabeledStatementExperiment(),
					new LuaEmptyStatementExperiment(),
				};
				const string langName = "Lua";
				var learningSets = new[] {
					Tuple.Create(Fixture.GetInputProjectPath(langName, "pageobjectgenerator"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.Lua"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "presto"),
							new List<string> { Fixture.GetInputCodePath(langName, "Seed.Lua"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "storm"),
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
				BitLearningExperimentWithGrouping exp, string projectPath, IList<string> seedPaths) {
			var allPaths = Directory.GetFiles(projectPath, "*.Lua", SearchOption.AllDirectories)
					.ToList();
			exp.LearnUntilBeStable(allPaths, seedPaths, 0.5);
			Assert.That(exp.WrongCount, Is.EqualTo(0));
		}
	}

	public class LuaBranchExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return new LuaProcessor(); }
		}

		public LuaBranchExperiment() : base("exp") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			// ifトークンが存在するかどうか
			var ifConds = ast.Descendants("stat")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "if")
					.SelectMany(e => e.Elements("exp"))
					.Select(e => Tuple.Create(e, 0));
			var whileConds = ast.Descendants("stat")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "while")
					.Select(e => e.Element("exp"))
					.Select(e => Tuple.Create(e, 1));
			var doConds = ast.Descendants("stat")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "repeat")
					.Select(e => e.Element("exp"))
					.Select(e => Tuple.Create(e, 2));
			return ifConds.Concat(whileConds).Concat(doConds);
		}
	}

	public class LuaIfExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return new LuaProcessor(); }
		}

		public LuaIfExperiment() : base("exp") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			// ifトークンが存在するかどうか
			return ast.Descendants("stat")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "if")
					.SelectMany(e => e.Elements("exp"))
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class LuaWhileExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return new LuaProcessor(); }
		}

		public LuaWhileExperiment() : base("exp") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			// whileトークンが存在するかどうか
			return ast.Descendants("stat")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "while")
					.Select(e => e.Element("exp"))
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class LuaDoWhileExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return new LuaProcessor(); }
		}

		public LuaDoWhileExperiment() : base("exp") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			// doトークンが存在するかどうか
			return ast.Descendants("stat")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "repeat")
					.Select(e => e.Element("exp"))
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class LuaStatementExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return new LuaProcessor(); }
		}

		public LuaStatementExperiment() : base("stat") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			return ast.Descendants("stat")
					.Where(e => {
						// ラベルはループ文に付くため，ラベルの中身は除外
						if (e.FirstElement().Name() == "label") {
							return false;
						}
						if (e.FirstElement().Value == ";") {
							return false;
						}
						return true;
					})
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class LuaLabeledStatementExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return new LuaProcessor(); }
		}

		public LuaLabeledStatementExperiment() : base("stat") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			return ast.Descendants("stat")
					.Where(e => e.FirstElement().Name() == "label")
					.Select(e => Tuple.Create(e, 0));
		}
	}

	public class LuaEmptyStatementExperiment : BitLearningExperimentWithGrouping {
		protected override Processor Processor {
			get { return new LuaProcessor(); }
		}

		public LuaEmptyStatementExperiment() : base("stat") {}

		protected override IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast) {
			return ast.Descendants("stat")
					.Where(e => e.FirstElement().Value == ";")
					.Select(e => Tuple.Create(e, 0));
		}
	}
}