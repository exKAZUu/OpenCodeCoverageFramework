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
using Occf.Core.CoverageInformation;
using Occf.Core.TestInformation;
using Occf.Core.Utils;
using Paraiba.IO;

namespace Occf.Tools.Cui {
	public class PathAnalyzer {
		private const string S = "  ";
		private const int W = 12;

		private static readonly string Usage =
				"Occf 1.0.0" + "\n" +
						"Copyright (C) 2011 Kazunori SAKAMOTO" + "\n" +
						"" + "\n" +
						"Usage: Occf path <root> [<coverage>]" + "\n" +
						"" + "\n" +
						S + "<root>".PadRight(W)
						+ "path of root directory (including source and test code)" + "\n" +
						S + "<coverage>".PadRight(W) + "path of coverage data whose name is "
						+ OccfNames.CoverageData + "\n" +
						"";

		public static bool Run(IList<string> args) {
			if (args.Count < 1) {
				return Program.Print(Usage);
			}
			var iArgs = 0;
			var rootDir = new DirectoryInfo(args[iArgs++]);
			if (!rootDir.Exists) {
				return
						Program.Print(
								"Root directory doesn't exist.\nroot:" + rootDir.FullName);
			}
			var covDataFile = args.Count >= iArgs + 1
					? new FileInfo(args[iArgs++]) : null;
			covDataFile = FileUtil.GetCoverageData(covDataFile, rootDir);
			if (!covDataFile.SafeExists()) {
				return
						Program.Print(
								"Coverage data file doesn't exist.\ncoverage:" + covDataFile.FullName);
			}

			return Analyze(rootDir, covDataFile);
		}

		private static bool Analyze(DirectoryInfo rootDir, FileInfo covDataFile) {
			var formatter = new BinaryFormatter();
			var covInfoFile = FileUtil.GetCoverageInfo(rootDir);
			var testInfoFile = FileUtil.GetTestInfo(rootDir);
			var covInfo = CoverageInfo.ReadCoverageInfo(covInfoFile, formatter);
			var testInfo = TestInfo.ReadTestInfo(testInfoFile, formatter);
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
	}
}