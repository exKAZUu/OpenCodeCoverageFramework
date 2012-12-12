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
using NDesk.Options;
using Occf.Core.CoverageInformation;
using Occf.Core.Modes;
using Occf.Core.Utils;
using Paraiba.IO;

namespace Occf.Tools.Cui {
	public class PathAnalyzer {
		private const string S = "  ";
		private const int W = 12;
        
		private static readonly string Usage =
                Program.Header +
                "" + "\n" +
				"Usage: Occf path -r <root_dir> [options]" + "\n" +
				"" + "\n" +
				S + "-r, -root, <root_dir>".PadRight(W)
				+ "path of root directory (including source and test code)" + "\n" +
				S + "-c, -cov".PadRight(W) + "path of coverage data whose name is "
				+ OccfNames.CoverageData + "\n" +
                S + "-o, -out".PadRight(W)
                + "output coverage information to file" + "\n"
                + "";

		public static bool Run(IList<string> args) {
            var rootDirPath = "";
            var covFilePath = "";
            var outFilePath = "";

            // parse options
            var p = new OptionSet {
					{ "r|root=", v => rootDirPath = v },
					{ "c|cov=", v => covFilePath = v },
                    { "o|out=", v => outFilePath = v },
			};

            // コマンドをパース "-"指定されないのだけargs[]に残る
            try {
                args = p.Parse(args);
            } catch {
                Console.WriteLine("catch");
                return Program.Print(Usage);
            }

            if (String.IsNullOrEmpty(rootDirPath)) {
		        return Program.Print(Usage);
		    }

            var rootDir = new DirectoryInfo(rootDirPath);
            if(!rootDir.Exists) {
                return
                        Program.Print(
                                "Root directory doesn't exist.\nroot:" + rootDir.FullName);
            }

            FileInfo covFile = null;
            if(!string.IsNullOrEmpty(covFilePath)) {
                covFile = new FileInfo(covFilePath);
                if(!covFile.Exists) {
                    return
                            Program.Print(
                                    "Error: coverage file doesn't exist.\ncoverage:" + covFile.FullName);
                }
            }
            covFile = PathFinder.FindCoverageDataPath(covFile, rootDir);
            if(!covFile.SafeExists()) {
                return
                        Program.Print(
                                "Coverage data file doesn't exist.\ncoverage:" + covFile.FullName);
            }

            var outFile = new FileInfo(outFilePath);

            if (outFilePath == "") {
                return Analyze(rootDir, covFile);
            } else {
                Console.WriteLine("writing : " + outFile.FullName);
                return AnalyzeToFile(rootDir, covFile, outFile);
            }
		}

		private static bool Analyze(DirectoryInfo rootDir, FileInfo covDataFile) {
			var formatter = new BinaryFormatter();
			var covInfoFile = PathFinder.FindCoverageInfoPath(rootDir);
			var testInfoFile = PathFinder.FindTestInfoPath(rootDir);
			var covInfo = InfoReader.ReadCoverageInfo(covInfoFile, formatter);
			var testInfo = InfoReader.ReadTestInfo(testInfoFile, formatter);
			testInfo.InitializeForStoringData(true);
			CoverageDataReader.ReadFile(testInfo, covDataFile);

			foreach (var testCase in testInfo.TestCases) {
				Console.WriteLine(
						"**** " + testCase.RelativePath + ": " + testCase.Name + " ****");
				var stmts = testCase.Paths.Select(i => covInfo.Targets[i]);
				foreach (var stmt in stmts) {
					Console.WriteLine(stmt.RelativePath + ": " + stmt.Position.SmartLineString);
				}
				Console.WriteLine();
				Console.WriteLine();
			}

			return true;
		}

        private static bool AnalyzeToFile(DirectoryInfo rootDir, FileInfo covDataFile, FileInfo outFile) {
            var formatter = new BinaryFormatter();
            var covInfoFile = PathFinder.FindCoverageInfoPath(rootDir);
            var testInfoFile = PathFinder.FindTestInfoPath(rootDir);
            var covInfo = InfoReader.ReadCoverageInfo(covInfoFile, formatter);
            var testInfo = InfoReader.ReadTestInfo(testInfoFile, formatter);
            testInfo.InitializeForStoringData(true);
            CoverageDataReader.ReadFile(testInfo, covDataFile);

            using (var sw = new StreamWriter(outFile.FullName)) {
                foreach (var testCase in testInfo.TestCases) {
                    sw.WriteLine(
                            "**** " + testCase.RelativePath + ": " + testCase.Name + " ****");
                    var stmts = testCase.Paths.Select(i => covInfo.Targets[i]);
                    foreach (var stmt in stmts) {
                        sw.WriteLine(stmt.RelativePath + ": " + stmt.Position.SmartLineString);
                    }
                    sw.WriteLine();
                    sw.WriteLine();
                }
            }

            return true;
        }
	}
}