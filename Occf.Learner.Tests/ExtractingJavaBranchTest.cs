﻿using System.IO;
using System.Linq;
using Accord.Statistics.Kernels;
using NUnit.Framework;
using ParserTests;

namespace Occf.Learner.Core.Tests {
	[TestFixture]
	public class ExtractingJavaBranchTest {
		[Test]
		public void TestPageObjectGenerator() {
			var path = Fixture.GetInputProjectPath("Java", "pageobjectgenerator");

			var allPaths = Directory.GetFiles(path, "*.java", SearchOption.AllDirectories)
					.ToList();
			var seedPaths = new[] {
				Path.Combine(path, "src", "main", "java", "com", "google", "testing", "pogen",
						"GenerateCommand.java"),
				Path.Combine(path, "src", "main", "java", "com", "google", "testing", "pogen", "generator",
						"test", "java", "TestCodeGenerator.java"),
			};

			var exp = new JavaBranchExperiment(allPaths);
			var learner = new SvmLearner(new Linear());
			exp.LearnUntilBeStable(seedPaths, learner, 0.5);
		}
	}
}