#region License

// Copyright (C) 2009-2013 Kazunori Sakamoto
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
using Occf.Core.CoverageInformation;
using Occf.Core.Manipulators;
using Occf.Core.TestInformation;
using Occf.Core.Utils;

namespace Occf.Tools.Cui {
	public class Inserter {
		private const string S = "  ";
		private const int W = 22;

		private static readonly string Usage =
				Program.Header +
						"" + "\n" +
						"Usage: Occf insert -r <root_dir> [options] [<file1> <file2> ...]"
						+ "\n" +
						"" + "\n" +
						S + "-r, -root <root_dir>".PadRight(W)
						+ "root directory for writing project information and "
						+ "\n" +
						S + "".PadRight(W) + "library files to measure coverage."
						+ "\n" +
						S + "-t, -test <test_dir>".PadRight(W) +
						"test code directory for execluding files as measurement"
						+ "\n" + S + "".PadRight(W) + "targets and for localizing faults."
						+ "\n" +
						S + "-l, -lang <name>".PadRight(W) +
						"language of target source. <name> can be Java(default),"
						+ "\n" + S + "".PadRight(W) + "C, Python2 or Python3."
						+ "\n" +
						S + "-p, -pattern <name>".PadRight(W)
						+ "search pattern for exploring target source files "
						+ "\n" + S + "".PadRight(W) + "in the root directory."
						+ "\n" +
						S + "-i, -lib <path>".PadRight(W)
						+ "path of library directory where library files for "
						+ "\n" + S + "".PadRight(W) + "measuring coverage are copied."
						+ "\n" +
						S + "<files>".PadRight(W)
						+ "file or directory paths to be inserted"
						+ "\n" + S + "".PadRight(W) + "(root directory is used if no specified file)."
						+ "\n" +
						"";

		public static bool Run(IList<string> args) {
			var rootDirPath = "";
			var testDirPath = "";
			var languageName = "Java";
			var libDirPath = "";
			var patterns = new List<string>();

			// Set an option grammar
			var p = new OptionSet {
					{ "r|root=", v => rootDirPath = v },
					{ "t|test=", v => testDirPath = v },
					{ "l|lang=", v => languageName = v },
					{ "i|lib=", v => libDirPath = v },
					{ "p|pattern=", patterns.Add },
			};

            // Parse optoins and return arguments excluding parsed options such as "-r root"
			try {
				args = p.Parse(args);
			} catch {
				Console.WriteLine("catch");
				return Program.Print(Usage);
			}

			if (string.IsNullOrEmpty(rootDirPath)) {
				return Program.Print(Usage);
			}

			LanguageSupport mode;
			try {
				mode = LanguageSupports.GetCoverageModeByClassName(languageName);
			} catch {
				return
						Program.Print("Error: cant't load script file for programming language of " + languageName);
			}

			var rootDir = new DirectoryInfo(rootDirPath);
			if (!rootDir.Exists) {
				return Program.Print("Root directory doesn't exist.\nroot:" + rootDir.FullName);
			}

			DirectoryInfo testDir = null;
			if (!string.IsNullOrEmpty(testDirPath)) {
				testDir = new DirectoryInfo(testDirPath);
				if (!testDir.Exists) {
					return Program.Print("Error: test code directory doesn't exist.\ntest:" + testDir.FullName);
				}
			}

			var libDir = rootDir;
			if (!string.IsNullOrEmpty(libDirPath)) {
				libDir = new DirectoryInfo(libDirPath);
				if (!libDir.Exists) {
					return Program.Print("Error: working directory doesn't exist.\nwork:" + libDir.FullName);
				}
			}

			if (patterns.Count == 0) {
				patterns = mode.FilePatterns.ToList();
			}

			var fileInfos = GetFileInfos(args, patterns, testDir);

			InsertMeasurementCode(rootDir, fileInfos, testDir, libDir, mode);

			return true;
		}

		public static ICollection<FileInfo> GetFileInfos(
				IEnumerable<string> fileOrDirPaths, IList<string> patterns, DirectoryInfo testDir) {
			var fileInfoList = new List<FileInfo>();
			foreach (var fileOrDirPath in fileOrDirPaths) {
				if (File.Exists(fileOrDirPath)) {
					fileInfoList.Add(new FileInfo(fileOrDirPath));
				} else if (Directory.Exists(fileOrDirPath)) {
					fileInfoList.AddRange(
							patterns
									.SelectMany(
											pat => new DirectoryInfo(fileOrDirPath)
													.GetFiles(pat, SearchOption.AllDirectories)));
				} else {
					Console.WriteLine("Error: the specified path doesn't exist.\npath:" + fileOrDirPath);
				}
			}
			IEnumerable<FileInfo> fileInfos = fileInfoList;

			// Ignore test code in the directory of production code
			if (testDir != null) {
				fileInfos = fileInfos.Where(f => !f.FullName.StartsWith(testDir.FullName));
			}

			// Ignore backup files
			fileInfos = fileInfos.Where(fi => !fi.FullName.EndsWith(OccfNames.LineBackUpSuffix))
					.Where(fi => !fi.FullName.EndsWith(OccfNames.KleeBackUpSuffix))
					.Where(fi => !fi.FullName.EndsWith(OccfNames.BackupSuffix));

			// Avoid duplications
			return fileInfos.ToDictionary(fi => fi.FullName).Values;
		}

		public static void InsertMeasurementCode(
				DirectoryInfo rootDir, ICollection<FileInfo> fileInfos, DirectoryInfo testDir,
				DirectoryInfo libDir, LanguageSupport mode) {
			Contract.Requires<ArgumentException>(rootDir.Exists);
			Contract.Requires<ArgumentException>(testDir == null || testDir.Exists);
			Contract.Requires<ArgumentException>(libDir.Exists);
			Contract.Requires<ArgumentNullException>(mode != null);

			//root
			var covInfo = new CoverageInfo(rootDir.FullName, mode.Name, SharingMethod.File);
			//(o)root or src?
			var testInfo = testDir != null
					? new TestInfo(rootDir.FullName)
					: null;
			//root
			RemoveExistingCoverageDataFiles(rootDir, libDir);

			mode.RemoveLibraries(libDir);
			//+src
			WriteProductionCodeFiles(rootDir, fileInfos, mode, covInfo);
			if (testInfo != null) {
				//(o)root or src
				WriteTestCodeFiles(rootDir, testDir, mode, testInfo);
			} else {
				// Initialize test information with empty contents
				testInfo = new TestInfo(rootDir.FullName);
				testInfo.TestCases.Add(new TestCase("nothing", "nothing", new CodePosition()));
			}
			//root
			WriteInfoFiles(rootDir, covInfo, testInfo);

			CopyLibraries(mode, libDir);
		}

		private static void CopyLibraries(LanguageSupport mode, DirectoryInfo dirInfo) {
			mode.CopyLibraries(dirInfo);
		}

		private static void RemoveExistingCoverageDataFiles(DirectoryInfo rootDir, DirectoryInfo workDir) {
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
				DirectoryInfo rootDir, IEnumerable<FileInfo> fileInfos, LanguageSupport mode, CoverageInfo info) {
			foreach (var path in fileInfos) {
				// 対象ファイルに対してKlee_backやLine_backがあるときは作成しない
				if (!(File.Exists(path.FullName + OccfNames.LineBackUpSuffix))
						&& !(File.Exists(path.FullName + OccfNames.KleeBackUpSuffix))) {
					var backPath = Path.Combine(rootDir.FullName, path.FullName + OccfNames.BackupSuffix);
					path.CopyTo(backPath, true);
				}

				var outPath = OccfCodeGenerator.WriteCoveragedCode(mode, info, path, rootDir);
				Console.WriteLine("wrote:" + outPath);
			}
		}

		private static void WriteTestCodeFiles(
				DirectoryInfo rootDir, DirectoryInfo testDir, LanguageSupport prof, TestInfo info) {
			var paths = prof.FilePatterns.SelectMany(
					pattern => testDir.EnumerateFiles(
							pattern, SearchOption.AllDirectories));
			foreach (var path in paths.ToList()) {
				var backPath = Path.Combine(rootDir.FullName, path.FullName + OccfNames.BackupSuffix);
				path.CopyTo(backPath, true); //バックアップファイルの上書き可
				var outPath = OccfCodeGenerator.AnalyzeAndWriteIdentifiedTest(prof, info, path, rootDir);
				Console.WriteLine("wrote:" + outPath);
			}
		}

		private static void WriteInfoFiles(DirectoryInfo rootDir, CoverageInfo covInfo, TestInfo testInfo) {
			var formatter = new BinaryFormatter();
			CoverageInfo.WriteCoverageInfo(rootDir, covInfo, formatter);
			if (testInfo == null) {
				return;
			}
			TestInfo.WriteTestInfo(rootDir, testInfo, formatter);
		}
	}
}