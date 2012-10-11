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
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Code2Xml.Core.Position;
using NDesk.Options;
using Occf.Core.CoverageCode;
using Occf.Core.CoverageInformation;
using Occf.Core.Modes;
using Occf.Core.TestInfos;
using Occf.Core.Utils;
using Paraiba.IO;

namespace Occf.Tools.Cui {
	public class Inserter {
		private const string S = "  ";
		private const int W = 20;

		private static readonly string Usage =
				Program.Header + 
				"" + "\n" +
				"Usage: Occf insert <root> [<test>] [options]" + "\n" +
				"" + "\n" +
				S + "<root>".PadRight(W)
				+ "path of root directory (including source and test code)" + "\n" +
				S + "<test>".PadRight(W) + "path of test code directory" + "\n" +
				S + "-l, -lang <name>".PadRight(W)
				+
				"language of target source. <name> can be Java(default), C, Python2 or Python3."
				+ "\n" +
				S + "-w, -work <path>".PadRight(W)
				+ "path of working directory used as current directory at testing." + "\n" +
				S + "".PadRight(W)
				+ "library files to measure coverage are copied in specified directory"
				+ "\n" +
				"";

		public static bool Run(IList<string> args) {
			var languageName = "Java";
			var workDirPath = "";

			// parse options
			var p = new OptionSet {
					{ "l|lang=", v => languageName = v },
					{ "w|work=", v => workDirPath = v },
			};
			try {
				args = p.Parse(args);
			} catch {
				return Program.Print(Usage);
			}

			if (args.Count < 1) {
				return Program.Print(Usage);
			}

			CoverageMode mode;
			try {
				mode = CoverageModes.GetCoverageModeByClassName(languageName);
			} catch {
				return
						Program.Print(
								"error: cant't load script file for programming language of "
								+ languageName);
			}

			var rootDir = new DirectoryInfo(args[0]);
			if (!rootDir.Exists) {
				return
						Program.Print(
								"Root directory doesn't exist.\nroot:" + rootDir.FullName);
			}

			DirectoryInfo testDir = null;
			if (args.Count >= 2) {
				testDir = new DirectoryInfo(args[1]);
				if (!testDir.Exists) {
					return
							Program.Print(
									"Error: test code directory doesn't exist.\ntest:" + testDir.FullName);
				}
			}

			var workDir = rootDir;
			if (!string.IsNullOrEmpty(workDirPath)) {
				workDir = new DirectoryInfo(workDirPath);
				if (!workDir.Exists) {
					return
							Program.Print(
									"Error: working directory doesn't exist.\nwork:" + workDir.FullName);
				}
			}

			InsertMeasurementCode(rootDir, testDir, workDir, mode);
			return true;
		}

		public static void InsertMeasurementCode(
				DirectoryInfo rootDir, DirectoryInfo testDir, DirectoryInfo workDir,
				CoverageMode mode) {
			Contract.Requires<ArgumentException>(rootDir.Exists);
			Contract.Requires<ArgumentException>(
					testDir == null || testDir.Exists);
			Contract.Requires<ArgumentException>(workDir.Exists);
			Contract.Requires<ArgumentNullException>(mode != null);

			var covInfo = new CoverageInfo(
					rootDir.FullName, mode.Name, SharingMethod.File);
			var testInfo = testDir != null
					               ? new TestInfo(rootDir.FullName)
					               : null;

			RemoveExistingCoverageDataFiles(rootDir, workDir);

			RemoveExistingLibraries(mode, workDir);

			WriteProductionCodeFiles(rootDir, testDir, mode, covInfo);
			if (testInfo != null) {
				WriteTestCodeFiles(rootDir, testDir, mode, testInfo);
			} else {
				// Initialize test information with empty contents
				testInfo = new TestInfo(rootDir.FullName);
				testInfo.TestCases.Add(new TestCase("nothing", "nothing", new CodePosition()));
			}

			WriteInfoFiles(rootDir, covInfo, testInfo);

			CopyLibraries(mode, workDir);
		}

		private static void RemoveExistingLibraries(
				CoverageMode mode, DirectoryInfo dirInfo) {
			mode.RemoveLibraries(dirInfo);
		}

		private static void CopyLibraries(
				CoverageMode mode, DirectoryInfo dirInfo) {
			mode.CopyLibraries(dirInfo);
		}

		private static void RemoveExistingCoverageDataFiles(
				DirectoryInfo rootDir, DirectoryInfo workDir) {
			var targets = rootDir.EnumerateFiles(
					OccfNames.CoverageData, SearchOption.AllDirectories);
			if (workDir != null) {
				targets =
						targets.Concat(workDir.EnumerateFiles(OccfNames.CoverageData));
			}
			foreach (var target in targets.ToList()) {
				target.Delete();
			}
		}

		private static void WriteProductionCodeFiles(
				DirectoryInfo rootDir, DirectoryInfo testDir, CoverageMode prof,
				CoverageInfo info) {
			var paths = prof.FilePatterns.SelectMany(
					pattern => rootDir.EnumerateFiles(
							pattern, SearchOption.AllDirectories));
			// ignore test code in the directory of production code
			if (testDir != null) {
				paths = paths.Where(f => !f.FullName.StartsWith(testDir.FullName));
			}

			foreach (var path in paths.ToList()) {
				var bakPath = rootDir.GetFile(path + OccfNames.BackupSuffix).FullName;
                //対象ファイルに対してKlee_backやLine_backがあるときは作成しない
                if(!(File.Exists(path.FullName+OccfNames.LineBackUpSuffix)) 
                    && !(File.Exists(path.FullName+OccfNames.KleeBackUpSuffix))) {
                    path.CopyTo(bakPath, true);
                }
				var outPath = CoverageCodeGenerator.WriteCoveragedCode(
						prof, info, path, rootDir);
				Console.WriteLine("wrote:" + outPath);
			}
		}

		private static void WriteTestCodeFiles(
				DirectoryInfo rootDir, DirectoryInfo testDir, CoverageMode prof,
				TestInfo info) {
			var paths = prof.FilePatterns.SelectMany(
					pattern => testDir.EnumerateFiles(
							pattern, SearchOption.AllDirectories));
			foreach (var path in paths.ToList()) {
				var bakPath = rootDir.GetFile(path + OccfNames.BackupSuffix).FullName;
				path.CopyTo(bakPath, true);
				var outPath = CoverageCodeGenerator.AnalyzeAndWriteIdentifiedTest(
						prof, info, path, rootDir);
				Console.WriteLine("wrote:" + outPath);
			}
		}

		private static void WriteInfoFiles(
				DirectoryInfo rootDir, CoverageInfo covInfo, TestInfo testInfo) {
			var formatter = new BinaryFormatter();
			WriteCoverageInfo(rootDir, covInfo, formatter);
			if (testInfo == null) {
				return;
			}
			WriteTestInfo(rootDir, testInfo, formatter);
		}

		private static void WriteCoverageInfo(
				DirectoryInfo rootDir, CoverageInfo covInfo, BinaryFormatter formatter) {
			var covPath = Path.Combine(rootDir.FullName, OccfNames.CoverageInfo);
			using (var fs = new FileStream(covPath, FileMode.Create)) {
				formatter.Serialize(fs, covInfo);
			}
		}

		private static void WriteTestInfo(
				DirectoryInfo rootDir, TestInfo testInfo, BinaryFormatter formatter) {
			var testPath = Path.Combine(rootDir.FullName, OccfNames.TestInfo);
			using (var fs = new FileStream(testPath, FileMode.Create)) {
				formatter.Serialize(fs, testInfo);
			}
		}
	}
}