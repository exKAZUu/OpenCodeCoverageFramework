using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using NDesk.Options;
using Occf.Core.CoverageCode;
using Occf.Core.CoverageInformation;
using Occf.Core.Extensions;
using Occf.Core.TestInfos;
using Occf.Tools.Core;

namespace Occf.Tools.Cui {
	public class Inserter {
		private const string S = "  ";
		private const int W = 20;

		private static readonly string Usage =
				"Occf 1.0.0" + "\n" +
				"Copyright (C) 2011 Kazunori SAKAMOTO" + "\n" +
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

			if (args.Count < 1)
				return Program.Print(Usage);
			var rootDirPath = args[0];
			var testDirPath = "";
			if (args.Count >= 2)
				testDirPath = args[1];

			ScriptCoverageProfile profile;
			try {
				profile = ScriptCoverageProfile.Load(languageName);
			} catch {
				return
						Program.Print(
								"error: cant't load script file for programming language of "
								+ languageName);
			}

			if (!Directory.Exists(rootDirPath)) {
				return
						Program.Print(
								"error: root directory doesn't exist.\nroot:" + rootDirPath);
			}
			rootDirPath = Path.GetFullPath(rootDirPath);

			if (!string.IsNullOrEmpty(testDirPath)) {
				if (!Directory.Exists(testDirPath)) {
					return
							Program.Print(
									"error: test code directory doesn't exist.\ntest:" + testDirPath);
				}
				testDirPath = Path.GetFullPath(testDirPath);
			} else {
				testDirPath = null;
			}

			if (!string.IsNullOrEmpty(workDirPath)) {
				if (!Directory.Exists(workDirPath)) {
					return
							Program.Print(
									"error: working directory doesn't exist.\nwork:" + workDirPath);
				}
				workDirPath = Path.GetFullPath(workDirPath);
			} else {
				workDirPath = rootDirPath;
			}

			InsertMeasurementCode(rootDirPath, testDirPath, workDirPath, profile);
			return true;
		}

		public static void InsertMeasurementCode(
				string rootDirPath, string testDirPath, string workDirPath,
				ScriptCoverageProfile profile) {
			Contract.Requires<ArgumentException>(Directory.Exists(rootDirPath));
			Contract.Requires<ArgumentException>(
					string.IsNullOrEmpty(testDirPath) || Directory.Exists(testDirPath));
			Contract.Requires<ArgumentException>(Directory.Exists(workDirPath));
			Contract.Requires<ArgumentNullException>(profile != null);

			var covInfo = new CoverageInfo(
					rootDirPath, profile.Name, SharingMethod.File);
			var testInfo = !string.IsNullOrEmpty(testDirPath)
			               		? new TestInfo(0, rootDirPath)
			               		: null;

			RemoveExistingCoverageDataFiles(rootDirPath, workDirPath);

			RemoveExistingLibraries(profile, workDirPath);

			WriteProductionCodeFiles(rootDirPath, testDirPath, profile, covInfo);
			if (testInfo != null)
				WriteTestCodeFiles(rootDirPath, testDirPath, profile, testInfo);

			WriteInfos(rootDirPath, covInfo, testInfo);

			CopyLibraries(profile, workDirPath);
		}

		private static void RemoveExistingLibraries(CoverageProfile profile, string dirPath) {
			foreach (var name in profile.LibraryNames) {
				var dstPath = Path.Combine(dirPath, name);
				File.Delete(dstPath);
			}
		}

		private static void CopyLibraries(CoverageProfile profile, string dirPath) {
			foreach (var name in profile.LibraryNames) {
				var srcPath = Path.Combine(Names.Library, name);
				var dstPath = Path.Combine(dirPath, name);
				File.Copy(srcPath, dstPath, true);
			}
		}

		private static void RemoveExistingCoverageDataFiles(
				string rootDirPath, string workDirPath) {
			var targets = Directory.EnumerateFiles(
					rootDirPath, Names.CoverageData, SearchOption.AllDirectories);
			if (!string.IsNullOrEmpty(workDirPath))
				targets =
						targets.Concat(Directory.EnumerateFiles(workDirPath, Names.CoverageData));
			foreach (var target in targets) {
				File.Delete(target);
			}
		}

		private static void WriteProductionCodeFiles(
				string rootDirPath,
				string testDirPath,
				CoverageProfile prof,
				CoverageInfo info) {
			var paths = Directory.EnumerateFiles(
					rootDirPath, prof.FilePattern, SearchOption.AllDirectories);
			// ignore test code in the directory of production code
			if (!string.IsNullOrEmpty(testDirPath)) {
				paths = paths.Where(s => !s.StartsWith(testDirPath));
			}

			foreach (var path in paths.ToList()) {
				var bakPath = path + Names.BackupSuffix;
				File.Copy(path, bakPath, true);
				var outPath = CoverageCodeGenerator.WriteCoveragedCode(
						prof, info, path, rootDirPath);
				Console.WriteLine("wrote:" + outPath);
			}
		}

		private static void WriteTestCodeFiles(
				string rootDirPath,
				string testDirPath, CoverageProfile prof,
				TestInfo info) {
			var paths = Directory.EnumerateFiles(
					testDirPath, prof.FilePattern, SearchOption.AllDirectories);

			foreach (var path in paths.ToList()) {
				var bakPath = path + Names.BackupSuffix;
				File.Copy(path, bakPath, true);
				var outPath = CoverageCodeGenerator.WriteIdentifiedTest(
						prof, info, path, rootDirPath);
				Console.WriteLine("wrote:" + outPath);
			}
		}

		private static void WriteInfos(
				string rootDirPath, CoverageInfo covInfo, TestInfo testInfo) {
			var formatter = new BinaryFormatter();

			var covPath = Path.Combine(rootDirPath, Names.CoverageInfo);
			using (var fs = new FileStream(covPath, FileMode.Create)) {
				formatter.Serialize(fs, covInfo);
			}

			if (testInfo == null)
				return;
			var testPath = Path.Combine(rootDirPath, Names.TestInfo);
			using (var fs = new FileStream(testPath, FileMode.Create)) {
				formatter.Serialize(fs, testInfo);
			}
		}
	}
}