#region License

// Copyright (C) 2011-2012 The Unicoen Project
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using IronPython.Hosting;
using Occf.Core.CoverageInformation;
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
			if (args.Count < 2) {
				return Program.Print(Usage);
			}

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

			testInfo.InitializeForStoringData();
			ReadTestResult(retPath, testInfo);
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

		private static string GetFailedTestCasePrefix(ref int index) {
			return (++index) + ") ";
		}

		private static readonly Regex regex = new Regex(@"([\w\d]*)(?:\[\d*\])?\(([\w\d.]*)\)");

		public static void ReadTestResult(string resultFilePath, TestInfo testInfo) {
			using (var reader = new StreamReader(resultFilePath)) {
				var failedTestIndex = 0;
				var prefix = GetFailedTestCasePrefix(ref failedTestIndex);
				while (true) {
					var line = reader.ReadLine();
					if (line == null) {
						return;
					}
					if (!line.StartsWith(prefix)) {
						continue;
					}
					var match = regex.Match(line.Substring(prefix.Length));
					if (!match.Success) {
						continue;
					}
					var name = match.Groups[2] + "." + match.Groups[1];
					// TODO: O(n^2)
					var testCase = testInfo.TestCases.FirstOrDefault(t => t.Name == name);
					if (testCase != null) {
						testCase.Passed = false;
					}

					prefix = GetFailedTestCasePrefix(ref failedTestIndex);
				}
			}
		}
	}
}