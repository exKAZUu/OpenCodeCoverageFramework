using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Occf.Core.Tests;
using Occf.Tools.Core;
using Paraiba.Core;
using Paraiba.IO;

namespace Occf.Tools.Cui.Tests {
	[TestFixture]
	public class InsertProgramTest {
		private const string JavaPath = "java";
		private const string JavacPath = "javac";

		[Test]
		[TestCase("GetMid", "GetMidTest")]
		[TestCase("GetMid3", "GetMid3Test")]
		public void InsertMeasurementCode(string projectName, string testTargetNames) {
			var outDirPath = Fixture.CleanOuputPath();
			var inDirPath = Fixture.GetProjectInputPath(projectName);
			var expDirPath = Fixture.GetProjectExpectationPath(projectName);
			FileUtility.CopyRecursively(inDirPath, outDirPath);

			var profile = ScriptCoverageProfile.Load("Java");
			Inserter.InsertMeasurementCode(
					outDirPath,
					Path.Combine(outDirPath, "test"), outDirPath, profile);

			// jarとdllファイルが存在するか
			var jar = Path.Combine(outDirPath, "CoverageWriter.File.jar");
			Assert.That(File.Exists(jar), Is.True);
			var dll = Path.Combine(outDirPath, "Occf.Writer.File.Java.dll");
			Assert.That(File.Exists(dll), Is.True);

			var covinfo = Path.Combine(outDirPath, Names.CoverageInfo);
			var testinfo = Path.Combine(outDirPath, Names.TestInfo);

			var targets = Directory.EnumerateFiles(
					expDirPath, "*.java",
					SearchOption.AllDirectories)
					.Concat(
							Directory.EnumerateFiles(
									expDirPath, Names.BackupSuffix,
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
					UseShellExecute = false,
					WorkingDirectory = outDirPath,
			};
			try {
				using (var p = Process.Start(info)) {
					p.WaitForExit();
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