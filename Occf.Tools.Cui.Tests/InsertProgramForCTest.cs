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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Occf.Core.Modes;
using Occf.Core.Tests;
using Occf.Core.Utils;
using Paraiba.Core;
using Paraiba.IO;

namespace Occf.Tools.Cui.Tests {
	[TestFixture]
	public class InsertProgramForCTest {
		private const string GccCommand = "gcc";

		[Test]
		[TestCase("Block1")]
		[TestCase("sort")]
		public void InsertMeasurementCode(string projectName) {
			OccfGlobal.SaveCurrentState();

			var outDirPath = Fixture.CleanOuputPath();
			var outDir = new DirectoryInfo(Fixture.CleanOuputPath());
			var inDirPath = Fixture.GetProjectInputPath(projectName);
			var expDirPath = Fixture.GetProjectExpectationPath(projectName);
			FileUtility.CopyRecursively(inDirPath, outDirPath);

			VerifyMeasureAndLocalize(inDirPath, expDirPath, outDir, outDirPath);
		}

		private static void VerifyMeasureAndLocalize(string inDirPath, string expDirPath,
				DirectoryInfo outDir, string outDirPath) {
			// カレントディレクトリを途中で変更しても動作するか検証
			Environment.CurrentDirectory = "C:\\";

			var profile = CoverageModes.GetCoverageModeByClassName("C");
			Inserter.InsertMeasurementCode(outDir, outDir, null, null, outDir, profile);

			// .cと.hファイルが存在するか
			Assert.That(
					File.Exists(Path.Combine(outDirPath, "covman.c")),
					Is.True);
			Assert.That(
					File.Exists(Path.Combine(outDirPath, "covman.h")),
					Is.True);

			var covinfo = Path.Combine(outDirPath, OccfNames.CoverageInfo);
			var testinfo = Path.Combine(outDirPath, OccfNames.TestInfo);

			var targets = Directory.EnumerateFiles(
					expDirPath, "*.c",
					SearchOption.AllDirectories)
					.Concat(
							Directory.EnumerateFiles(
									expDirPath, OccfNames.BackupSuffix,
									SearchOption.AllDirectories))
					.Concat(new[] { covinfo, testinfo });
			foreach (var target in targets) {
				AssertEqualFiles(target, expDirPath, inDirPath);
			}

			Compile(outDirPath);
			RunTest(outDirPath);

			var ret = BugLocalizer.Run(
			        new[] {
			                outDirPath,
			                Path.Combine(outDirPath, "testresult.txt")
			        });
			Assert.That(ret, Is.True);
		}

		private static void Compile(string outDirPath) {
			var args = Directory.EnumerateFiles(
					outDirPath, "*.c", SearchOption.TopDirectoryOnly);
			var info = new ProcessStartInfo {
					FileName = GccCommand,
					Arguments = args.JoinString(" "),
					CreateNoWindow = true,
					UseShellExecute = false,
					WorkingDirectory = outDirPath,
			};
			try {
				using (var p = Process.Start(info)) {
					p.WaitForExit();
				}
			} catch (Win32Exception e) {
				throw new InvalidOperationException("Failed to launch " + info.FileName + ".", e);
			}
		}

		private static void RunTest(string outDirPath) {
			var info = new ProcessStartInfo {
					FileName = Path.Combine(outDirPath, "a.exe"),
					Arguments = new string[0].JoinString(" "),
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					UseShellExecute = false,
					WorkingDirectory = outDirPath,
			};
			try {
				// TODO
				File.WriteAllText(Path.Combine(outDirPath, "testresult.txt"), "");
				using (var p = Process.Start(info)) {
					p.WaitForExit();
				}
			} catch (Win32Exception e) {
				throw new InvalidOperationException("Failed to launch " + info.FileName + ".", e);
			}
		}

		private static void AssertEqualFiles(
				string inFilePath, string outDirPath, string inDirPath) {
			var relative = XPath.GetRelativePath(inFilePath, inDirPath);
			var actualJava = XPath.GetFullPath(relative, outDirPath);
			Assert.That(
					File.ReadAllBytes(actualJava),
					Is.EqualTo(File.ReadAllBytes(inFilePath)));
		}
	}
}