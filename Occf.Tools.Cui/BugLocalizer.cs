using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using IronPython.Hosting;
using Occf.Core.CoverageInfos;
using Occf.Core.TestInfos;
using Occf.Tools.Core;

namespace Occf.Tools.Cui {
	public class BugLocalizer {
		private const string S = "  ";
		private const int W = 12;

		private static readonly string Usage =
				"Occf 1.0.0" + "\n" +
				"Copyright (C) 2011 Kazunori SAKAMOTO" + "\n" +
				"" + "\n" +
				"Usage: Occf localize <root> <result> [<coverage>]" + "\n" +
				"" + "\n" +
				S + "<root>".PadRight(W)
				+ "path of root directory (including source and test code)" + "\n" +
				S + "<result>".PadRight(W)
				+
				"path of test result file which contains indexes of failed test case with csv"
				+ "\n" +
				S + "<coverage>".PadRight(W) + "path of coverage data whose name is "
				+ Names.CoverageData + "\n" +
				"";

		public static bool Run(IList<string> args) {
			if (args.Count < 2)
				return Program.Print(Usage);

			var iArgs = 0;
			var rootPath = args[iArgs++];
			if (!Directory.Exists(rootPath)) {
				return Program.Print("root directory doesn't exist.\nroot:" + rootPath);
			}
			rootPath = Path.GetFullPath(rootPath);

			var retPath = args[iArgs++];
			if (!Directory.Exists(rootPath)) {
				return
						Program.Print(
								"error: test result file doesn't exist.\nresult:" + retPath);
			}
			retPath = Path.GetFullPath(retPath);

			var covPath = args.Count >= iArgs + 1 ? args[iArgs++] : null;
			if (!File.Exists(covPath)) {
				covPath = PathFinder.FindCoverageDataPath(covPath);
			}
			if (!File.Exists(covPath)) {
				covPath = PathFinder.FindCoverageDataPath(rootPath);
			}
			if (!File.Exists(covPath)) {
				return
						Program.Print("coverage data file doesn't exist.\ncoverage:" + covPath);
			}
			covPath = Path.GetFullPath(covPath);

			Localize(rootPath, covPath, retPath);

			return true;
		}

		public static void Localize(
				string rootPath, string covPath, string retPath) {
			var formatter = new BinaryFormatter();
			var covInfoPath = PathFinder.FindCoverageInfoPath(rootPath);
			var testInfoPath = PathFinder.FindTestInfoPath(rootPath);
			var covInfo = InfoReader.ReadCoverageInfo(covInfoPath, formatter);
			var testInfo = InfoReader.ReadTestInfo(testInfoPath, formatter);

			ReadTestResult(retPath, testInfo);

			testInfo.InitializeForStoringData();
			CoverageDataReader.ReadFile(testInfo, covPath);

			var engine = Python.CreateEngine();
			var scope = engine.CreateScope();
			engine.ExecuteFile("BugLocalization.py", scope);
			var calcMetricFunc =
					scope.GetVariable<Func<double, double, double, double, IEnumerable>>(
							"CalculateMetric");

			// Targeting only statement
			foreach (var stmt in covInfo.StatementIndexAndTargets) {
				var passedTestCases = testInfo.TestCases.Where(t => t.Passed);
				var executedAndPassedTestCases =
						passedTestCases.Where(t => t.Statements.Contains(stmt.Item1));
				var failedTestCases = testInfo.TestCases.Where(t => !t.Passed);
				var executedAndFailedTestCases =
						failedTestCases.Where(t => t.Statements.Contains(stmt.Item1));

				var executedAndPassedCount = executedAndPassedTestCases.Count();
				var passedCount = passedTestCases.Count();
				var executedAndFailedCount = executedAndFailedTestCases.Count();
				var failedCount = failedTestCases.Count();
				var metrics = calcMetricFunc(
						executedAndPassedCount,
						passedCount,
						executedAndFailedCount,
						failedCount);
				var metricsString = "";
				var delimiter = "";
				foreach (var metric in metrics) {
					metricsString += (delimiter + ((double)metric).ToString("f3"));
					delimiter = ", ";
				}

				var tag = stmt.Item2.Tag + ": " + stmt.Item2.Position.SmartLine;
				Console.WriteLine(tag.PadRight(45) + ": " + metricsString);
			}
		}

		public static void ReadTestResult(string resultFilePath, TestInfo testInfo) {
			using (var reader = new StreamReader(resultFilePath)) {
				while (true) {
					var relativePath = reader.ReadLine();
					var line = reader.ReadLine();
					if (line == null)
						break;

					var testCases = testInfo.TestCases
							.SkipWhile(tc => tc.RelativePath != relativePath)
							.TakeWhile(tc => tc.RelativePath == relativePath);
					var isPasseds = line
							.Select(s => s == '.')
							.ToList();
					foreach (var t in testCases.Zip(isPasseds, Tuple.Create)) {
						t.Item1.Passed = t.Item2;
					}
				}
			}
		}
	}
}