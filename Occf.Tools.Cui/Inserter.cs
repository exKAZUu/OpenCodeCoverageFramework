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
				"Usage: Occf insert -r <root_dir> [options] <target_file1> <target_file2>"
				+ "\n" +
				"" + "\n" +
				S + "-r, -root <root_dir>".PadRight(W)
				+ "root directory for writing project information and library files to measure coverage"
				+ "\n" +
				S + "-t, -test <test>".PadRight(W) +
				"test code directory for execluding files as measurement targets and for localizing faults"
				+ "\n" +
				S + "-n, -lang <name>".PadRight(W) +
				"language of target source. <name> can be Java(default), C, Python2 or Python3."
				+ "\n" +
				S + "-p, -pattern <name>".PadRight(W)
				+ "search pattern for exploring target source files in the root directory."
				+ "\n" +
				S + "-i, -lib <path>".PadRight(W)
				+ "path of library directory where library files for measuring coverage are copied."
				+ "\n" +
				"";

		public static bool Run(IList<string> args) {
			var rootDirPath = "";
			var testDirPath = "";
			var languageName = "Java";
			var libDirPath = "";
			var patterns = new List<string>();

			// parse options
			var p = new OptionSet {
					{ "r|root=", v => rootDirPath = v },
					{ "t|test=", v => testDirPath = v },
					{ "l|lang=", v => languageName = v },
					{ "p|pattern=", patterns.Add },
					{ "i|lib=", v => libDirPath = v },
			};
			try {
				args = p.Parse(args);
			} catch {
				return Program.Print(Usage);
			}

			if (args.Count < 1 || string.IsNullOrEmpty(rootDirPath)) {
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

			var rootDir = new DirectoryInfo(rootDirPath);
			if (!rootDir.Exists) {
				return
						Program.Print(
								"Root directory doesn't exist.\nroot:" + rootDir.FullName);
			}

			DirectoryInfo testDir = null;
			if (string.IsNullOrEmpty(testDirPath)) {
				testDir = new DirectoryInfo(testDirPath);
				if (!testDir.Exists) {
					return
							Program.Print(
									"Error: test code directory doesn't exist.\ntest:" + testDir.FullName);
				}
			}

			var libDir = rootDir;
			if (!string.IsNullOrEmpty(libDirPath)) {
				libDir = new DirectoryInfo(libDirPath);
				if (!libDir.Exists) {
					return
							Program.Print(
									"Error: working directory doesn't exist.\nwork:" + libDir.FullName);
				}
			}

			InsertMeasurementCode(rootDir, patterns, testDir, libDir, mode);
			return true;
		}

		public static void InsertMeasurementCode(
				DirectoryInfo rootDir, List<string> patterns, DirectoryInfo testDir,
				DirectoryInfo libDir,
				CoverageMode mode) {
			Contract.Requires<ArgumentException>(rootDir.Exists);
			Contract.Requires<ArgumentException>(
					testDir == null || testDir.Exists);
			Contract.Requires<ArgumentException>(libDir.Exists);
			Contract.Requires<ArgumentNullException>(mode != null);
			patterns = patterns ?? new List<string>();

			var covInfo = new CoverageInfo(
					rootDir.FullName, mode.Name, SharingMethod.File);
			var testInfo = testDir != null
					               ? new TestInfo(rootDir.FullName)
					               : null;

			RemoveExistingCoverageDataFiles(rootDir, libDir);

			mode.RemoveLibraries(libDir);

			WriteProductionCodeFiles(rootDir, patterns, testDir, mode, covInfo);
			if (testInfo != null) {
				WriteTestCodeFiles(rootDir, testDir, mode, testInfo);
			} else {
				// Initialize test information with empty contents
				testInfo = new TestInfo(rootDir.FullName);
				testInfo.TestCases.Add(
						new TestCase("nothing", "nothing", new CodePosition()));
			}

			WriteInfoFiles(rootDir, covInfo, testInfo);

			CopyLibraries(mode, libDir);
		}

		private static void CopyLibraries(
				CoverageMode mode, DirectoryInfo dirInfo) {
			mode.CopyLibraries(dirInfo);
		}

		private static void RemoveExistingCoverageDataFiles(
				DirectoryInfo rootDir, DirectoryInfo workDir) {
			const string filePattern = "*" + OccfNames.CoverageData;
			var targets = rootDir.EnumerateFiles(
					filePattern, SearchOption.AllDirectories);
			if (workDir != null) {
				targets = targets.Concat(workDir.EnumerateFiles(filePattern));
			}
			foreach (var target in targets.ToList()) {
				target.Delete();
			}
		}

		private static void WriteProductionCodeFiles(
				DirectoryInfo rootDir, IEnumerable<string> patterns, DirectoryInfo testDir,
				CoverageMode mode, CoverageInfo info) {
			var paths = mode.FilePatterns.SelectMany(
					pat => rootDir.EnumerateFiles(
							pat, SearchOption.AllDirectories));
			paths =
					paths.Concat(
							patterns.Where(pattern => !mode.FilePatterns.Contains(pattern))
							        .SelectMany(
									        pat => rootDir.EnumerateFiles(pat, SearchOption.AllDirectories)));
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
						mode, info, path, rootDir);
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
				path.CopyTo(bakPath, false);
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