﻿using System;
using System.IO;
using System.Linq;

namespace Occf.Core.Tests {
	public static class Fixture {
		public static string FixturePath = Path.Combine("..", "..", "fixture");
	
		private const string Expectation = "expectation";
		private const string Input = "input";

		private const string Coverage = "coverage";
		private const string Test = "test";
		private const string Project = "project";
		private const string Output = "output";

		public static string CleanOuputPath() {
			var path = GetOutputPath();
			if (Directory.Exists(path)) {
				Directory.Delete(path, true);
			}
			Directory.CreateDirectory(path);
			return path.GetFullPathAddingSubNames();
		}

		public static string GetFullPathAddingSubNames(this string path, params string[] subNames) {
			return Path.GetFullPath(subNames.Aggregate(path, Path.Combine));
		}

		public static string GetOutputPath(params string[] names) {
			return Path.Combine(FixturePath, Output)
				.GetFullPathAddingSubNames(names);
		}

		public static string GetCoverageInputPath(params string[] names) {
			return Path.Combine(FixturePath, Coverage, Input)
				.GetFullPathAddingSubNames(names);
		}

		public static string GetCoverageExpectationPath(params string[] names) {
			return Path.Combine(FixturePath, Coverage, Expectation)
				.GetFullPathAddingSubNames(names);
		}

		public static string GetTestInputPath(params string[] names) {
			return Path.Combine(FixturePath, Test, Input)
				.GetFullPathAddingSubNames(names);
		}

		public static string GetTestExpectationPath(params string[] names) {
			return Path.Combine(FixturePath, Test, Expectation)
				.GetFullPathAddingSubNames(names);
		}

		public static string GetProjectInputPath(params string[] names) {
			return Path.Combine(FixturePath, Project, Input)
				.GetFullPathAddingSubNames(names);
		}

		public static string GetProjectExpectationPath(params string[] names) {
			return Path.Combine(FixturePath, Project, Expectation)
				.GetFullPathAddingSubNames(names);
		}
	}
}