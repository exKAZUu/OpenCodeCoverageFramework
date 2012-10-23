#region License

// Copyright (C) 2009-2012 Kazunori Sakamoto
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
using Occf.Core.Utils;
using Paraiba.IO;

namespace Occf.Tools.Cui {
	public class BugLocalizer {
		private const string S = "  ";
		private const int W = 12;

		private static readonly string Usage =
				Program.Header +
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
				+ OccfNames.CoverageData + "\n" +
				"";

		public static bool Run(IList<string> args) {
			if (args.Count < 2) {
				return Program.Print(Usage);
			}

			var iArgs = 0;
			var rootDir = new DirectoryInfo(args[iArgs++]);
			if (!rootDir.Exists) {
				return
						Program.Print(
								"Root directory doesn't exist.\nroot:" + rootDir.FullName);
			}

			var resultFile = new FileInfo(args[iArgs++]);
			if (!resultFile.Exists) {
				return
						Program.Print(
								"Error: test result file doesn't exist.\nresult:" + resultFile.FullName);
			}

			var covDataFile = args.Count >= iArgs + 1
					                  ? new FileInfo(args[iArgs++]) : null;
			covDataFile = PathFinder.FindCoverageDataPath(covDataFile, rootDir);
			if (!covDataFile.SafeExists()) {
				return
						Program.Print(
								"Coverage data file doesn't exist.\ncoverage:" + covDataFile.FullName);
			}

			Localize(rootDir, resultFile, covDataFile);

			return true;
		}

		private static void Localize(
				DirectoryInfo rootDir, FileInfo resultFile, FileInfo covDataFile) {
			var formatter = new BinaryFormatter();
			var covInfoFile = PathFinder.FindCoverageInfoPath(rootDir);
			var testInfoFile = PathFinder.FindTestInfoPath(rootDir);
			var covInfo = InfoReader.ReadCoverageInfo(covInfoFile, formatter);
			var testInfo = InfoReader.ReadTestInfo(testInfoFile, formatter);

			testInfo.InitializeForStoringData(true);
			ReadJUnitResult(resultFile, testInfo);
			CoverageDataReader.ReadFile(testInfo, covDataFile);

			LocalizeStatements(testInfo, covInfo, new Dictionary<FileInfo, Dictionary<int, int>>());
		}

	    /// <summary>
	    /// Localize bugs in statements.
	    /// </summary>
	    /// <param name="testInfo"></param>
	    /// <param name="covInfo"></param>
	    /// <param name="lindDic"></param>
	    public static void LocalizeStatements(TestInfo testInfo, CoverageInfo covInfo, Dictionary<FileInfo, Dictionary<int, int>> lindDic) { // Targeting only statement
	        Console.WriteLine("before: Python.create");
            var engine = Python.CreateEngine();
	        Console.WriteLine("before: engine.create");
			var scope = engine.CreateScope();
			var fileName = "BugLocalization.py";
	        Console.WriteLine("before scriptPath");
			var scriptPath = Path.Combine(OccfGlobal.CurrentDirectory, fileName);
			if (!File.Exists(scriptPath)) {
				scriptPath = Path.Combine(OccfGlobal.ExeDirectory, fileName);
			}
	        Console.WriteLine("before: engine.Execute");
			engine.ExecuteFile(scriptPath, scope);
	        Console.WriteLine("before: calcMetricFunc");
			var calcMetricFunc =
					scope.GetVariable<Func<double, double, double, double, IEnumerable>>(
							"CalculateMetric");
			Console.WriteLine(
					"risk, executedAndPassedCount / passedCount, executedAndFailedCount / failedCount");
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
				foreach (double metric in metrics) {
					metricsString += (delimiter + (metric).ToString("f3"));
					delimiter = ", ";
				}

                //Dictionaryを検索してKeyに対象ファイルが存在したらオリジナル行番号に変換
			    string tag;
			    var fileInfo = new FileInfo(stmt.Item2.RelativePath);
                if(lindDic.ContainsKey(fileInfo)) {
                    var orgStartLine = lindDic[fileInfo][stmt.Item2.Position.StartLine];
                    var orgEndLine = lindDic[fileInfo][stmt.Item2.Position.EndLine];
                    var orgStartLineString = orgStartLine == orgEndLine 
                                            ? orgStartLine.ToString() : (orgStartLine + " - " + orgEndLine); 
                    tag = stmt.Item2.Tag + ": " + orgStartLineString;
                }else {
                    tag = stmt.Item2.Tag + ": " + stmt.Item2.Position.SmartLineString;
                }
                
				Console.WriteLine(tag.PadRight(45) + ": " + metricsString);
			}
		}

		//private static IEnumerable<double> CalculateMetric(
		//        int executedAndPassedCount, int passedCount, int executedAndFailedCount,
		//        int failedCount) {
		//    if (passedCount == 0) {
		//        return new[] { 0.0 };
		//    }
		//    if (failedCount == 0) {
		//        return new[] { 1.0 };
		//    }
		//    var p = (double)executedAndPassedCount / passedCount;
		//    var f = (double)executedAndFailedCount / failedCount;
		//    return new[] { p / (p + f), p, f };
		//}

		private static string GetFailedTestCasePrefix(ref int index) {
			return (++index) + ") ";
		}

		private static readonly Regex FailedTestLineRegex =
				new Regex(@"([\w\d]*)(?:\[\d*\])?\(([\w\d.]*)\)");

		public static void ReadJUnitResult(FileInfo resultFile, TestInfo testInfo) {
			using (var reader = new StreamReader(resultFile.FullName)) {
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
					var match = FailedTestLineRegex.Match(line.Substring(prefix.Length));
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