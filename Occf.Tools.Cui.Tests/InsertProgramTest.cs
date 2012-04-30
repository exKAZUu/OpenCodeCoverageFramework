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
using Occf.Core;
using Occf.Core.Profiles;
using Occf.Core.Tests;
using Occf.Core.Utils;
using Paraiba.Core;
using Paraiba.IO;

namespace Occf.Tools.Cui.Tests {
	[TestFixture]
	public class InsertProgramTest {
		private const string JavaPath = "java";
		private const string JavacPath = "javac";

		[Test]
		public void RunChangingOhterEnvironment() {}

		[Test]
		[TestCase("GetMid", "GetMidTest")]
		[TestCase("GetMid3", "GetMid3Test")]
		public void InsertMeasurementCode(string projectName, string testTargetNames) {
			OccfGlobal.SaveCurrentDirectory();

			var outDirPath = Fixture.CleanOuputPath();
			var outDir = new DirectoryInfo(Fixture.CleanOuputPath());
			var inDirPath = Fixture.GetProjectInputPath(projectName);
			var expDirPath = Fixture.GetProjectExpectationPath(projectName);
			FileUtility.CopyRecursively(inDirPath, outDirPath);

			VerifyMeasureAndLocalize(
					testTargetNames, inDirPath, expDirPath, outDir, outDirPath);
		}

		private static void VerifyMeasureAndLocalize(
				string testTargetNames, string inDirPath, string expDirPath,
				DirectoryInfo outDir, string outDirPath) {

			// カレントディレクトリを途中で変更しても動作するか検証
			Environment.CurrentDirectory = "C:\\";

			var profile = CoverageProfiles.GetCoverageProfileByClassName("Java");
			Inserter.InsertMeasurementCode(
					outDir, new DirectoryInfo(Path.Combine(outDirPath, "test")), outDir,
					profile);

			// jarとdllファイルが存在するか
			var jar = Path.Combine(outDirPath, "CoverageWriter.File.jar");
			Assert.That(File.Exists(jar), Is.True);
			var dll = Path.Combine(outDirPath, "Occf.Writer.File.Java.dll");
			Assert.That(File.Exists(dll), Is.True);

			var covinfo = Path.Combine(outDirPath, OccfNames.CoverageInfo);
			var testinfo = Path.Combine(outDirPath, OccfNames.TestInfo);

			var targets = Directory.EnumerateFiles(
					expDirPath, "*.java",
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
			RunTest(outDirPath, testTargetNames);

			var ret = BugLocalizer.Run(
					new[] {
							outDirPath,
							Path.Combine(outDirPath, "testresult.txt")
					});
			Assert.That(ret, Is.True);
		}

		private static void Compile(string outDirPath) {
			var args = new[] {
					"-cp",
					".;CoverageWriter.File.jar;junit-4.8.2.jar",
					"-sourcepath",
					"src",
					"-d",
					".",
					@"test\*.java"
			};
			var info = new ProcessStartInfo {
					FileName = JavacPath,
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
				throw new InvalidOperationException("Failed to launch 'javac'.", e);
			}
		}

		private static void RunTest(string outDirPath, string testTargetNames) {
			var args = new List<string> {
					"-cp",
					".;CoverageWriter.File.jar;junit-4.8.2.jar",
					"org.junit.runner.JUnitCore",
					testTargetNames,
			};
			var info = new ProcessStartInfo {
					FileName = JavaPath,
					Arguments = args.JoinString(" "),
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					UseShellExecute = false,
					WorkingDirectory = outDirPath,
			};
			try {
				using (var p = Process.Start(info))
				using (var fs = new StreamWriter(Path.Combine(outDirPath, "testresult.txt"))) {
					fs.WriteFromStream(p.StandardOutput);
				}
			} catch (Win32Exception e) {
				throw new InvalidOperationException("Failed to launch 'java'.", e);
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