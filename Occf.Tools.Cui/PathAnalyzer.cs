using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Occf.Core.CoverageInfos;
using Occf.Core.TestInfos;
using Occf.Tools.Core;

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
				+ Names.CoverageData + "\n" +
				"";

		public static bool Run(IList<string> args) {
			if (args.Count < 1)
				return Program.Print(Usage);
			var iArgs = 0;
			var rootPath = args[iArgs++];
			if (!Directory.Exists(rootPath)) {
				return Program.Print("root directory doesn't exist.\nroot:" + rootPath);
			}
			rootPath = Path.GetFullPath(rootPath);

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

			return Analyze(rootPath, covPath);
		}

		private static bool Analyze(string rootPath, string covPath) {
			var formatter = new BinaryFormatter();
			var covInfoPath = PathFinder.FindCoverageInfoPath(rootPath);
			var testInfoPath = PathFinder.FindTestInfoPath(rootPath);
			var covInfo = InfoReader.ReadCoverageInfo(covInfoPath, formatter);
			var testInfo = InfoReader.ReadTestInfo(testInfoPath, formatter);
			testInfo.InitializeForStoringData();
			CoverageDataReader.ReadFile(testInfo, covPath);

			foreach (var testCase in testInfo.TestCases) {
				Console.WriteLine(
						"**** " + testCase.RelativePath + ": " + testCase.Name + " ****");
				var stmts = testCase.Paths.Select(i => covInfo.TargetList[i]);
				foreach (var stmt in stmts) {
					Console.WriteLine(stmt.RelativePath + ": " + stmt.Position.SmartLine);
				}
				Console.WriteLine();
				Console.WriteLine();
			}

			return true;
		}
	}
}