﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Accord.Statistics.Kernels;
using Code2Xml.Core;
using NUnit.Framework;
using Occf.Learner.Core.Tests.LearningAlgorithms;
using Paraiba.Xml.Linq;
using ParserTests;

namespace Occf.Learner.Core.Tests {
	[TestFixture]
	public class JavaScriptExperimentTest {
				private static IEnumerable<TestCaseData> TestCases {
			get {
				var exps = new LearningExperiment[] {
					new JavaScriptBranchExperiment(),
					new JavaScriptConsoleLogExperiment(),
				};
				var algorithms = new LearningAlgorithm[] {
					new SvmLearner(new Linear()),
					//new NaiveBayesLearner(), 
					//new C45Learner(new SvmLearner(new Linear())),
				};
				const string langName = "JavaScript";
				var learningSets = new[] {
					//Tuple.Create(Fixture.GetInputProjectPath(langName, "cheet.js"),
					//		new List<string> { Fixture.GetInputCodePath(langName, "seed.js"), }),
					Tuple.Create(Fixture.GetInputProjectPath(langName, "ionic"),
							new List<string> { Fixture.GetInputCodePath(langName, "seed.js"), }),
					//Tuple.Create(Fixture.GetInputProjectPath(langName, "reportr"),
					//		new List<string> { Fixture.GetInputCodePath(langName, "seed.js"), }),
				};
				foreach (var exp in exps) {
					foreach (var algorithm in algorithms) {
						foreach (var learningSet in learningSets) {
							yield return new TestCaseData(exp, algorithm, learningSet.Item1, learningSet.Item2);
						}
					}
				}
			}
		}

		[Test, TestCaseSource("TestCases")]
		public void Test(
				LearningExperiment exp, LearningAlgorithm algorithm, string projectPath, IList<string> seedPaths) {
			var allPaths = Directory.GetFiles(projectPath, "*.js", SearchOption.AllDirectories)
					.ToList();
			exp.LearnUntilBeStable(allPaths, seedPaths, algorithm, 0.5);
			Assert.That(exp.WrongCount, Is.EqualTo(0));
		}
	}

	public class JavaScriptBranchExperiment : LearningExperiment {
		public JavaScriptBranchExperiment() : base("expression") {}

		protected override Processor Processor {
			get { return ProcessorLoader.JavaScriptUsingAntlr3; }
		}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			var ifConds = ast.Descendants("ifStatement")
					.Select(e => e.Element("expression"));
			var whileConds = ast.Descendants("whileStatement")
					.Select(e => e.Element("expression"));
			var doWhileConds = ast.Descendants("doWhileStatement")
					.Select(e => e.Element("expression"));
			var forConds = ast.Descendants("forStatement")
					.Select(e => e.Elements().First(e2 => e2.TokenText() == ";"))
					.Where(e => e.NextElement().Name() == "expression")
					.Select(e => e.NextElement());
			return ifConds.Concat(whileConds).Concat(doWhileConds).Concat(forConds);
		}
	}

	public class JavaScriptConsoleLogExperiment : LearningExperiment {
		public JavaScriptConsoleLogExperiment() : base("assignmentExpression") {}

		protected override Processor Processor {
			get { return ProcessorLoader.JavaScriptUsingAntlr3; }
		}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			var preConds = ast.Descendants("callExpression")
					.Where(e => e.FirstElement().Value == "console.log")
					.Select(e => e.Element("arguments").Element("assignmentExpression"))
					.Where(e => e != null);
			return preConds;
		}
	}
}