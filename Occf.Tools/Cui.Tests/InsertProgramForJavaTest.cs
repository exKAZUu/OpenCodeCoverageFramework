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
using Occf.Core.CoverageInformation;
using Occf.Core.TestInformation;
using Occf.Core.Tests;
using Occf.Core.Utils;
using Paraiba.Core;
using Paraiba.IO;

namespace Occf.Tools.Cui.Tests {
    [TestFixture]
    public class InsertProgramForJavaTest {
        private const string JavaCommand = "java";
        private const string JavacCommand = "javac";

        [Test]
        [TestCase("GetMid", "GetMidTest")]
        [TestCase("GetMid3", "GetMid3Test")]
        public void InsertMeasurementCode(string projectName, string testTargetNames) {
            var outDir = new DirectoryInfo(Fixture.CleanOuputPath());
            var inDirPath = Fixture.GetProjectInputPath(projectName);
            var expDirPath = Fixture.GetProjectExpectationPath(projectName);
            ParaibaFile.CopyRecursively(inDirPath, outDir.FullName);

            VerifyMeasureAndLocalize(
                    testTargetNames, inDirPath, expDirPath, outDir);
        }

        private static void VerifyMeasureAndLocalize(
                string testTargetNames, string inDirPath, string expDirPath, DirectoryInfo outDir) {
            var ret = Inserter.Run(
                    new[] {
                            "-r",
                            outDir.FullName,
                            "-t",
                            outDir.GetDirectory("test").FullName,
                    });
            Assert.That(ret, Is.True);

            // jarとdllファイルが存在するか
            var jar = outDir.GetFile("CoverageWriter.jar");
            Assert.That(jar.Exists, Is.True);

            var covInfoFile = outDir.GetFile(OccfNames.CoverageInfo);
            var testInfoFile = outDir.GetFile(OccfNames.TestInfo);
            var recordFile = outDir.GetFile(OccfNames.Record);
            var resultFile = outDir.GetFile("testresult.txt");

            var targets = Directory.EnumerateFiles(
                    expDirPath, "*.java",
                    SearchOption.AllDirectories)
                    .Concat(
                            Directory.EnumerateFiles(
                                    expDirPath, OccfNames.BackupSuffix,
                                    SearchOption.AllDirectories))
                    .Concat(new[] { covInfoFile.FullName, testInfoFile.FullName });
            foreach (var target in targets) {
                AssertEqualFiles(target, expDirPath, inDirPath);
            }

            Compile(outDir.FullName);
            RunTest(outDir.FullName, testTargetNames);

            var covInfo = CoverageInfo.Read(covInfoFile);
            var testInfo = TestInfo.Read(testInfoFile);

            testInfo.InitializeForStoringData(true);
            BugLocalizer.ReadJUnitResult(resultFile, testInfo);
            CoverageDataReader.ReadFile(testInfo, recordFile);

            var localizeStatements = BugLocalizer.LocalizeStatements(covInfo, testInfo,
                    "metrics/BugLocalization.py");
            Assert.That(localizeStatements, Is.Not.Null);
        }

        private static void Compile(string outDirPath) {
            var args = new[] {
                    "-cp",
                    ".;CoverageWriter.jar;junit-4.8.2.jar",
                    "-sourcepath",
                    "src",
                    "-d",
                    ".",
                    @"test\*.java"
            };
            var info = new ProcessStartInfo {
                    FileName = JavacCommand,
                    Arguments = args.JoinString(" "),
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = outDirPath,
            };
            try {
                using (var p = Process.Start(info)) p.WaitForExit();
            } catch (Win32Exception e) {
                throw new InvalidOperationException("Failed to launch 'javac'.", e);
            }
        }

        private static void RunTest(string outDirPath, string testTargetNames) {
            var args = new List<string> {
                    "-cp",
                    ".;CoverageWriter.jar;junit-4.8.2.jar",
                    "org.junit.runner.JUnitCore",
                    testTargetNames,
            };
            var info = new ProcessStartInfo {
                    FileName = JavaCommand,
                    Arguments = args.JoinString(" "),
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WorkingDirectory = outDirPath,
            };
            try {
                using (var p = Process.Start(info))
                using (var fs = new StreamWriter(Path.Combine(outDirPath, "testresult.txt")))
                    fs.WriteFromStream(p.StandardOutput);
            } catch (Win32Exception e) {
                throw new InvalidOperationException("Failed to launch 'java'.", e);
            }
        }

        private static void AssertEqualFiles(
                string inFilePath, string outDirPath, string inDirPath) {
            var relative = ParaibaPath.GetRelativePath(inFilePath, inDirPath);
            var actualJava = ParaibaPath.GetFullPath(relative, outDirPath);
            Assert.That(
                    File.ReadAllBytes(actualJava),
                    Is.EqualTo(File.ReadAllBytes(inFilePath)));
        }
    }
}