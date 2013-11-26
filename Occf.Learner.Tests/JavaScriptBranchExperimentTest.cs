using System.IO;
using System.Linq;
using Accord.Statistics.Kernels;
using NUnit.Framework;
using ParserTests;

namespace Occf.Learner.Core.Tests {
	[TestFixture]
	public class JavaScriptBranchExperimentTest {
		[Test]
		public void TestPageObjectGenerator() {
			var path = Fixture.GetInputProjectPath("JavaScript", "ionic");

			var allPaths = Directory.GetFiles(path, "*.js", SearchOption.AllDirectories)
					.ToList();
			var seedPaths = new[] {
				Fixture.GetInputCodePath("JavaScript", "extracting_branch.js"),
			};

			var exp = new JavaScriptBranchExperiment(allPaths);
			var learner = new SvmLearner(new Linear());
			exp.LearnUntilBeStable(seedPaths, learner, 0.5);
		}
	}
}