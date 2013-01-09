﻿#region License

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
using NDesk.Options;
using Occf.Core.CoverageInformation;
using Occf.Core.TestInformation;
using Occf.Core.Utils;
using Paraiba.IO;

namespace Occf.Tools.Cui {
	public class BugLocalizer {
		private const string S = "  ";
		private const int W = 24;

		private static readonly string Usage =
				Program.Header +
						"" + "\n" +
						"Usage: Occf localize -r <root_dir> -e <result> [options]" + "\n" +
						"" + "\n" +
						S + "-r -root <root_dir>".PadRight(W)
						+ "path of root directory (including source and test code)" + "\n" +
						S + "-e -result <result>".PadRight(W)
						+"path of test result file which contains indexes of failed test case with csv"
						+ "\n" +
						S + "-c -coverage <coverage>".PadRight(W) 
                        + "path of coverage data whose name is "
						+ OccfNames.CoverageData + "\n" +
                        S + "-m -metrics <metrics>".PadRight(W)
                        + "type of metrics of fault localization." + "\n" + 
                        S + "".PadRight(W) + " tar[antura], och[iai], jac[card] or rus[sell]"
                        + "\n" +
                        S + "-v -csv <csv_dir>".PadRight(W)
                        + "path of csv file directory" 
                        + "\n" +
						"";

		public static bool Run(IList<string> args) {

		    var rootDirPath = "";
		    var resultFilePath = "";
		    var covDataFilePath = "";
		    var metricsType = "";
		    var csvDirPath = "";

            // parse options
            var p = new OptionSet {
					{ "r|root=", v => rootDirPath = v },
					{ "e|result=", v => resultFilePath = v },
					{ "c|coverage=", v=> covDataFilePath = v },
                    { "m|metrics=", v => metricsType = v },
                    { "v|csv=", v => csvDirPath = v },
			};

            // コマンドをパース "-"指定されないのだけargs[]に残る
            try{
                args = p.Parse(args);
            } catch {
                Console.WriteLine("catch");
                return Program.Print(Usage);
            }

			if (args.Count > 0) {
				return Program.Print(Usage);
			}

            if (string.IsNullOrEmpty(rootDirPath)) {
                return Program.Print(Usage);
            }

			var rootDir = new DirectoryInfo(rootDirPath);
			if (!rootDir.Exists) {
				return
						Program.Print(
								"Root directory doesn't exist.\nroot:" + rootDir.FullName);
			}

            if (string.IsNullOrEmpty(resultFilePath)) {
                return Program.Print(Usage);
            }

			var resultFile = new FileInfo(resultFilePath);
			if (!resultFile.Exists) {
				return
						Program.Print(
								"Error: test result file doesn't exist.\nresult:" + resultFile.FullName);
			}

			FileInfo covDataFile = null;
            if (!string.IsNullOrEmpty(covDataFilePath)) {
                covDataFile = new FileInfo(covDataFilePath);
            }
			covDataFile = FileUtil.GetCoverageData(covDataFile, rootDir);
			if (!covDataFile.SafeExists()) {
				return
						Program.Print(
								"Coverage data file doesn't exist.\ncoverage:" + covDataFile.FullName);
			}

            var metricsFileName = "BugLocalization.py";
            if (!string.IsNullOrEmpty(metricsType)) {
                switch (metricsType) {
                    case "tar":
                    case "tarantula":
                        metricsFileName = "Tarantula.py";
                        break;
                    case "och":
                    case "ochiai":
                        metricsFileName = "Ochiai.py";
                        break;
                    case "jac":
                    case "jaccard":
                        metricsFileName = "Jaccard.py";
                        break;
                    case "rus":
                    case "russell":
                        metricsFileName = "Russell.py";
                        break;
                    default:
                    return Program.Print(Usage);
                }
            }


            DirectoryInfo csvDir = null;
            if (!string.IsNullOrEmpty(csvDirPath)){
                csvDir = new DirectoryInfo(csvDirPath);
                if (!csvDir.Exists){
                    return
                            Program.Print(
                                    "Csv-File directory doesn't exist.\ncsv:" + csvDir.FullName);
                }
            }
            
			Localize(rootDir, resultFile, covDataFile, metricsFileName, csvDir);

			return true;
		}

		private static void Localize(DirectoryInfo rootDir, FileInfo resultFile, 
                                     FileInfo covDataFile, string metricsFileName, DirectoryInfo csvDir) {
			var formatter = new BinaryFormatter();
			var covInfoFile = FileUtil.GetCoverageInfo(rootDir);
			var testInfoFile = FileUtil.GetTestInfo(rootDir);
			var covInfo = CoverageInfo.ReadCoverageInfo(covInfoFile, formatter);
			var testInfo = TestInfo.ReadTestInfo(testInfoFile, formatter);

			testInfo.InitializeForStoringData(true);
			ReadJUnitResult(resultFile, testInfo);
			CoverageDataReader.ReadFile(testInfo, covDataFile);

			LocalizeStatements(testInfo, covInfo, new Dictionary<FileInfo, Dictionary<int, int>>(), metricsFileName);

            if (csvDir != null) {
                LocalizeStatementsCSV(csvDir, testInfo, covInfo, new Dictionary<FileInfo, Dictionary<int, int>>());
            }
            
		}

		/// <summary>
		/// Localize bugs in statements.
		/// </summary>
		/// <param name="testInfo"></param>
		/// <param name="covInfo"></param>
		/// <param name="lindDic"></param>
		/// <param name="metricsFileName"></param>
		public static void LocalizeStatements(
				TestInfo testInfo, CoverageInfo covInfo, Dictionary<FileInfo, 
                Dictionary<int, int>> lindDic, string metricsFileName) { // Targeting only statement
			var engine = Python.CreateEngine();
			var scope = engine.CreateScope();
			//var fileName = "BugLocalization.py";
            var fileName = metricsFileName;

			var scriptPath = Path.Combine(OccfGlobal.CurrentDirectory, fileName);
			if (!File.Exists(scriptPath)) {
				scriptPath = Path.Combine(OccfGlobal.ExeDirectory, fileName);
			}

			engine.ExecuteFile(scriptPath, scope);

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
                FileInfo fileInfo = null;
                foreach (var fileinfos in lindDic.Keys){
                    var fileFullname = fileinfos.FullName;
                    var itemPath = stmt.Item2.RelativePath;
                    if (fileFullname.EndsWith(itemPath)) {
                        fileInfo = fileinfos;
                        break;
                    }
                }
				
				var orgLineNumFlag = true;
				if (fileInfo != null && fileInfo.Exists) {
					var orgStartLine = lindDic[fileInfo][stmt.Item2.Position.StartLine];
					var orgEndLine = lindDic[fileInfo][stmt.Item2.Position.EndLine];
					var orgStartLineString = orgStartLine == orgEndLine 
										? orgStartLine.ToString() : (orgStartLine + " - " + orgEndLine);
					//var orgStartLineString = orgStartLine.ToString();
					tag = stmt.Item2.Tag + ": " + orgStartLineString;
					if (orgStartLine == 0) {
						orgLineNumFlag = false;
					}
				} else {
				   tag = stmt.Item2.Tag + ": " + stmt.Item2.Position.SmartLineString;
				}

				if (orgLineNumFlag) {
					Console.WriteLine(tag.PadRight(45) + ": " + metricsString);
				}
			}
		}

        public static void LocalizeStatementsCSV(
                DirectoryInfo csvDir, TestInfo testInfo, CoverageInfo covInfo, 
                Dictionary<FileInfo, Dictionary<int, int>> lindDic){ // Targeting only statement
            
            var blElementList = new List<BLElement>();

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

                //Dictionaryを検索してKeyに対象ファイルが存在したらオリジナル行番号に変換

                FileInfo fileInfo = null;
                foreach (var fileinfos in lindDic.Keys) {
                    var fileFullname = fileinfos.FullName;
                    var itemPath = stmt.Item2.RelativePath;
                    if (fileFullname.EndsWith(itemPath)) {
                        fileInfo = fileinfos;
                        break;
                    }
                }

                int startLine;
                int endLine;
                string fileName;

                var orgLineNumFlag = true;
                if (fileInfo != null && fileInfo.Exists) {
                    var orgStartLine = lindDic[fileInfo][stmt.Item2.Position.StartLine];
                    var orgEndLine = lindDic[fileInfo][stmt.Item2.Position.EndLine];
                    
                    fileName = new FileInfo(stmt.Item2.RelativePath).Name;
                    startLine = orgStartLine;
                    endLine = orgEndLine;

                    if (orgStartLine == 0) {
                        orgLineNumFlag = false;
                    }

                } else {
                    fileName = new FileInfo(stmt.Item2.RelativePath).Name;
                    startLine = stmt.Item2.Position.StartLine;
                    endLine = stmt.Item2.Position.EndLine;
                }

                if (orgLineNumFlag) {
                    var blElement = new BLElement(fileName, startLine, endLine, 
                                                  passedCount, executedAndPassedCount,
                                                  failedCount, executedAndFailedCount);
                    blElementList.Add(blElement);
                }
                
            }

            /*
            var blad = new BLElement("Hogehoge.java", 12, 12, 2, 1, 1, 1);
            blElementList.Add(blad);
            */
            blElementList.Sort(delegate(BLElement ble1, BLElement ble2) { return ble1.StartLine - ble2.StartLine; });

            CsvWriter(csvDir, blElementList);
        }

        public static void CsvWriter(DirectoryInfo csvDir, List<BLElement> blElements) {
           
            //ファイルの初期化
            var fileList = new List<string>();
            var fileInfos = new List<FileInfo>();
            foreach (var blElement in blElements) {
                if (!fileList.Contains(blElement.FileName)) {
                    fileList.Add(blElement.FileName);
                    fileInfos.Add(new FileInfo(csvDir.FullName + "/OccfFL_" + blElement.FileName + ".csv"));
                }
            }
            
            foreach (var fileInfo in fileInfos) {
                using (var writer = new StreamWriter(fileInfo.FullName, false)) {
                    writer.WriteLine(fileInfo.Name.Substring(7, fileInfo.Name.Length-11));
                    writer.WriteLine("startLine,endLine,P(all),P(exe),F(all),F(exe)");
                    writer.Close();
                }
            }

            foreach (var blElement in blElements) {
                using (var writer = new StreamWriter(csvDir.FullName + "/OccfFL_" + blElement.FileName + ".csv", true)){
                    writer.WriteLine(blElement.CsvString());
                    writer.Close();
                }
            }

            Console.WriteLine("wrote csv file");
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