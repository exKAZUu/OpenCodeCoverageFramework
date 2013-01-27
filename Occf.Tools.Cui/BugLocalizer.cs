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
	                    + "path of root directory." + "\n" +
	                    S + "".PadRight(W) + "(including source and test code)" + "\n" +
	                    S + "-e -result <result>".PadRight(W)
	                    + "path of test result file which contains indexes of" + "\n" +
	                    S + "".PadRight(W) + "failed test case with csv." + "\n" +
	                    S + "-c -coverage <coverage>".PadRight(W)
	                    + "path of coverage data whose name is " + "\n" +
	                    S + "".PadRight(W) + "\"" +OccfNames.CoverageData + "\".\n" +
                        S + "-m -metrics <metrics>".PadRight(W)
                        + "type of metrics of fault localization." + "\n" +
                        S + "".PadRight(W) + "Python script file in the \"metrics\" directory." + "\n" +
                        S + "".PadRight(W) + "default:Tarantura[.py], Ochiai[.py], Jaccard[.py]" + "\n" +
                        S + "".PadRight(W) + "       :Russell[.py] or SBI[.py]."
                        + "\n" +
                        S + "-v -csv <out_dir>".PadRight(W)
                        + "path of the directory for csv file for output." 
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

            const string metricsDirName = "metrics";
            const string fileDelimiter = "/";

            var metricsFilePath = metricsDirName + fileDelimiter + "BugLocalization.py";
            if (!string.IsNullOrEmpty(metricsType))
            {

                //短縮コード
                switch (metricsType) {
                    case "tar":
                        metricsType = "Tarantula.py";
                        break;
                    case "och":
                        metricsType = "Ochiai.py";
                        break;
                    case "jac":
                        metricsType = "Jaccard.py";
                        break;
                    case "rus":
                        metricsType = "Russell.py";
                        break;
                    case "sbi":
                        metricsType = "SBI.py";
                        break;
                }

                if (!metricsType.EndsWith(".py")) {
                    metricsType += ".py";
                }

                metricsFilePath = metricsDirName + fileDelimiter + metricsType;
                var metricsFileInfo = new FileInfo(metricsFilePath);
                if (metricsFileInfo.Exists) {
                    Console.WriteLine("Error: not find \"" + metricsFilePath + "\"");
                    Console.WriteLine("Path: " + metricsFileInfo.FullName);
                    metricsFilePath = metricsDirName + fileDelimiter + "BugLocalization.py";
                    Console.WriteLine("chage default file : " + metricsFilePath);
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
            
			Localize(rootDir, resultFile, covDataFile, metricsFilePath, csvDir);

			return true;
		}

		private static void Localize(DirectoryInfo rootDir, FileInfo resultFile, 
                                     FileInfo covDataFile, string metricsFilePath, DirectoryInfo csvDir) {
			var formatter = new BinaryFormatter();
			var covInfoFile = FileUtil.GetCoverageInfo(rootDir);
			var testInfoFile = FileUtil.GetTestInfo(rootDir);
			var covInfo = CoverageInfo.ReadCoverageInfo(covInfoFile, formatter);
			var testInfo = TestInfo.ReadTestInfo(testInfoFile, formatter);

			testInfo.InitializeForStoringData(true);
			ReadJUnitResult(resultFile, testInfo);
			CoverageDataReader.ReadFile(testInfo, covDataFile);

			LocalizeStatements(testInfo, covInfo, new Dictionary<FileInfo, Dictionary<int, int>>(), metricsFilePath);

            if (csvDir != null) {
                LocalizeStatementsCsv(csvDir, testInfo, covInfo, new Dictionary<FileInfo, Dictionary<int, int>>());
            }
            
		}

		/// <summary>
		/// Localize bugs in statements.
		/// </summary>
		/// <param name="testInfo"></param>
		/// <param name="covInfo"></param>
		/// <param name="lindDic"></param>
		/// <param name="metricsFilePath"></param>
		public static void LocalizeStatements(
				TestInfo testInfo, CoverageInfo covInfo, Dictionary<FileInfo, 
                Dictionary<int, int>> lindDic, string metricsFilePath) { // Targeting only statement
			var engine = Python.CreateEngine();
			var scope = engine.CreateScope();
			//var fileName = "BugLocalization.py";
            var fileName = metricsFilePath;

			var scriptPath = Path.Combine(OccfGlobal.CurrentDirectory, fileName);
			if (!File.Exists(scriptPath)) {
				scriptPath = Path.Combine(OccfGlobal.ExeDirectory, fileName);
			}

			engine.ExecuteFile(scriptPath, scope);

			var calcMetricFunc =
					scope.GetVariable<Func<double, double, double, double, IEnumerable>>(
							"CalculateMetric");
            Console.WriteLine("metrics : " + new FileInfo(metricsFilePath).Name);
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

                //Dicに対象ファイルが存在するかしない場合はnullを返す。
                var fileInfo = lindDic.Keys.FirstOrDefault(info => info.FullName.EndsWith(stmt.Item2.RelativePath));

			    var orgLineNumFlag = true;
				if (fileInfo != null && fileInfo.Exists) {
                    //Dicに存在したとき

				    var nowStartLine = stmt.Item2.Position.StartLine;
				    var nowEndLine = stmt.Item2.Position.EndLine;
                    var lineDicKey = lindDic[fileInfo];

                    var orgStartLine = OrgLineNum(nowStartLine, lineDicKey);
				    var orgEndLine = OrgLineNum(nowEndLine, lineDicKey);

                    if (orgStartLine <= 0 || orgEndLine <= 0) {
                        //main用挿入コード
                        orgLineNumFlag = false;
                    }

					var orgStartLineString = orgStartLine == orgEndLine 
										? orgStartLine.ToString() : (orgStartLine + " - " + orgEndLine);
					//var orgStartLineString = orgStartLine.ToString();
					tag = stmt.Item2.Tag + ": " + orgStartLineString;
					

				} else {
				   tag = stmt.Item2.Tag + ": " + stmt.Item2.Position.SmartLineString;
				}

				if (orgLineNumFlag) {
					Console.WriteLine(tag.PadRight(45) + ": " + metricsString);
				}
			}
		}

        public static int OrgLineNum(int nowLineNum, Dictionary<int,int> lineDicKey) {
            int orgLineDiff;

            if (lineDicKey.ContainsKey(nowLineNum + 1)){
                orgLineDiff = lineDicKey[nowLineNum + 1];
                if (orgLineDiff == -2) {
                    return -1;
                }
            } else {//一致ない場合は前後をみる
                var before = lineDicKey[lineDicKey.Keys.Where(s => s <= (nowLineNum + 1)).Max()];
                var after = lineDicKey[lineDicKey.Keys.Where(s => s >= (nowLineNum + 1)).Min()];

                if (before == -2 && after == -2) {
                    //Main用挿入コード
                    return -1;
                }
                orgLineDiff = after;
            }
            var orgLineNum = nowLineNum - orgLineDiff;
 
            return orgLineNum;
        }

        public static void LocalizeStatementsCsv(
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
                //Dicに対象ファイルが存在するかしない場合はnullを返す。
                var fileInfo = lindDic.Keys.FirstOrDefault(info => info.FullName.EndsWith(stmt.Item2.RelativePath));

                int startLine;
                int endLine;
                var fileName = new FileInfo(stmt.Item2.RelativePath).Name;

                var orgLineNumFlag = true;
                if (fileInfo != null && fileInfo.Exists) {
                    //Dicに存在したとき

                    var nowStartLine = stmt.Item2.Position.StartLine;
                    var nowEndLine = stmt.Item2.Position.EndLine;
                    var lineDicKey = lindDic[fileInfo];

                    startLine = OrgLineNum(nowStartLine, lineDicKey);
                    endLine = OrgLineNum(nowEndLine, lineDicKey);

                    if (startLine <= 0 || endLine <= 0){
                        //main用挿入コード時
                        orgLineNumFlag = false;
                    }

                } else {
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

            blElementList.Sort(delegate(BLElement ble1, BLElement ble2) { return ble1.StartLine - ble2.StartLine; });

            CsvWriter(csvDir, blElementList);
        }

        public static void CsvWriter(DirectoryInfo csvDir, List<BLElement> blElements) {

            const string fileHeadder = "/OccfBL_";
            const string fileType = ".csv";
            const string lineOne = "startLine,endLine,P(all),P(exe),F(all),F(exe)";
            var orgFileNameStart = fileHeadder.Length - 1;
            var orgFileNameLength = fileHeadder.Length + fileType.Length - 1;

            //ファイルの初期化
            var fileList = new List<string>();
            var fileInfos = new List<FileInfo>();

            foreach (var blElement in blElements) {
                if (!fileList.Contains(blElement.FileName)) {
                    fileList.Add(blElement.FileName);
                    fileInfos.Add(new FileInfo(csvDir.FullName + fileHeadder + blElement.FileName + fileType));
                }
            }
            
            //重複解除 上で重複しないかな？
            /*
            for (var i = fileInfos.Count - 1; i >= 0; i--) {
                for (var j = i - 1; j >= 0; j--) {
                    if (fileInfos[i].FullName == fileInfos[j].FullName) {
                        fileInfos.Remove(fileInfos[i]);
                        break;
                    }
                }
            }*/

            foreach (var fileInfo in fileInfos) {
                using (var writer = new StreamWriter(fileInfo.FullName, false)) {
                    writer.WriteLine(fileInfo.Name.Substring(orgFileNameStart, orgFileNameLength));
                    writer.WriteLine(lineOne);
                    writer.Close();
                    Console.WriteLine("create csv file : " + fileInfo.Name);
                }
            }

            foreach (var blElement in blElements) {
                using (var writer = new StreamWriter(csvDir.FullName + fileHeadder + blElement.FileName + fileType, true)){
                    writer.WriteLine(blElement.CsvString());
                    writer.Close();
                }
            }

            if (!fileInfos.Any()) {
                Console.WriteLine("didn't create csv file");
                Console.WriteLine("There was no data to create csv file");
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