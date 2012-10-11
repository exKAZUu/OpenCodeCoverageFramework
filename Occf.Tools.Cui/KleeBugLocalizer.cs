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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Code2Xml.Core.Position;
using Occf.Core.CoverageInformation;
using Occf.Core.TestInfos;
using Occf.Core.Utils;
using Paraiba.IO;

namespace Occf.Tools.Cui {
	public class KleeBugLocalizer {
		private const string S = "  ";
		private const int W = 20;

		private static readonly string Usage =
				Program.Header +
				"" + "\n" +
				"Usage: Occf klee <root_directory> <test_directory>" + "\n" +
				"" + "\n" +
				S + "<root_directory>".PadRight(W) + "a path of a directory containing '.occf_coverage_info'" + "\n" +
				"\n" +
				S + "<test_directory>".PadRight(W) + "a path of klee test directory" + "\n" +
				"\n" +
				"";

		public static bool Run(IList<string> args) {
			// parse options
			if (args.Count != 2) {
				return Program.Print(Usage);
			}
			Localize(args);
			return true;
		}

		private static void Localize(IList<string> args) {
			var formatter = new BinaryFormatter();
			var rootDirInfo = new DirectoryInfo(args[0]);
			var testDirInfo = new DirectoryInfo(args[1]);
			var covInfoFile = PathFinder.FindCoverageInfoPath(rootDirInfo);
			var covInfo = InfoReader.ReadCoverageInfo(covInfoFile, formatter);
			var testInfo = AnalyzeKleeTestFiles(testDirInfo);
			AnalyzeTestResult(rootDirInfo, testInfo);

            //Line対応のMapのMapを作成、
		    var lineDic = LineDicCreater(rootDirInfo, testDirInfo);
            
			BugLocalizer.LocalizeStatements(testInfo, covInfo, lineDic);
		}

		private static TestInfo AnalyzeKleeTestFiles(DirectoryInfo testDirInfo) {
			var files = testDirInfo.EnumerateFiles("*.ktest");
			var testInfo = new TestInfo(testDirInfo.FullName);
			testInfo.InitializeForStoringData(false);
			foreach (var file in files) {
				var relativePath = XPath.GetRelativePath(file.FullName, testDirInfo.FullName);
				var testCase = new TestCase(relativePath, file.FullName, new CodePosition());
				testInfo.TestCases.Add(testCase);
				testCase.InitializeForStoringData(false);
				var dataPath = file.FullName + OccfNames.CoverageData;
				CoverageDataReader.ReadFile(testInfo, dataPath, testCase);
			}
			return testInfo;
		}

		private static void AnalyzeTestResult(DirectoryInfo rootDirInfo, TestInfo testInfo) {
			var fileInfo = rootDirInfo.GetFile(OccfNames.SuccessfulTests);
			using (var reader = fileInfo.OpenText()) {
				foreach (var line in reader.ReadLines()) {
					var testCase = testInfo.TestCases.FirstOrDefault(t => t.Name.EndsWith(line));
					if (testCase != null)
					{
						testCase.Passed = true;
					}
					else {
						Console.Error.WriteLine("[WARNING] the testcase of '" + line +"' is not founded.");
					}
				}
			}
		}

        private static Dictionary<FileInfo, Dictionary<int, int>> 
            LineDicCreater(DirectoryInfo rootDirInfo, DirectoryInfo testDirTnfo) {
            
            var fileList = new List<FileInfo>();
            fileList.AddRange(rootDirInfo.GetFiles("*.c", SearchOption.AllDirectories));
            fileList.AddRange(rootDirInfo.GetFiles("*.cpp", SearchOption.AllDirectories));
            fileList.AddRange(rootDirInfo.GetFiles("*.cxx", SearchOption.AllDirectories));

            if (testDirTnfo != null && testDirTnfo.Exists) {
                for (var i = fileList.Count - 1; i > 0; i--) {
                    if (fileList[i].FullName.StartsWith(testDirTnfo.FullName)) {
                        fileList.Remove(fileList[i]);
                    }
                }
            }

            var lineDic = fileList.ToDictionary(fileInfo => fileInfo, LineSymmetyCreater);

            return lineDic;
        }

        private static Dictionary<int, int> LineSymmetyCreater(FileInfo fileInfo) {
            var lineDic = new Dictionary<int, int>();
            var fileName = fileInfo.Name;
            var lineAppender = @"""" + fileName + @"""";
            var apdLength = lineAppender.Length;
            const string header = @"# ";
            const string divider = @" ";
            var leastLength = header.Length + divider.Length + apdLength;

            var trueLineNum = 0;

            using (var reader = new StreamReader(fileInfo.FullName)) {
                var nowLineNum = 1;
                string line;
                
                while ((line = reader.ReadLine()) != null) {
                    if (line.Length > leastLength) {
                        var lastSentence = line.Substring(line.Length - apdLength, apdLength);
                        if (lastSentence.Equals(lineAppender)) {
                            var digitNum = line.Length - leastLength;
                            trueLineNum = int.Parse(line.Substring(header.Length, digitNum));
                        }
                    }
                    lineDic.Add(nowLineNum, trueLineNum);
                    nowLineNum++;
                }
                reader.Close();
            }

            return lineDic;
        }
	}
}