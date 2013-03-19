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
using NDesk.Options;
using Occf.Core.CoverageInformation;
using Occf.Core.TestInformation;
using Occf.Core.Utils;
using Paraiba.Collections.Generic;
using Paraiba.IO;

namespace Occf.Tools.Cui {
    public class DuplicationDetector {
        private const string S = "  ";
        private const int W = 12;

        private static readonly string Usage =
                "Occf 1.0.0" + "\n" +
                        "Copyright (C) 2011 Kazunori SAKAMOTO" + "\n" +
                        "" + "\n" +
                        "Usage: Occf dup[licate] <root> [<coverage>] [options]" + "\n" +
                        "" + "\n" +
                        S + "<root>".PadRight(W)
                        + "path of root directory (including source and test code)" + "\n" +
                        S + "<coverage>".PadRight(W) + "path of coverage data whose name is "
                        + OccfNames.Record + "\n" +
                        S + "-c, -criterion <name>".PadRight(W)
                        +
                        "a detection criterion. <name> can be statement(default), branch, condition, branch/condition, subpath, path."
                        + "\n" +
                        "";

        public static bool Run(IList<string> args) {
            if (args.Count < 2) {
                return Program.Print(Usage);
            }

            var criterion = "";
            // parse options
            var p = new OptionSet {
                    { "c|criterion", v => criterion = v },
            };
            try {
                args = p.Parse(args);
            } catch {
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
            covDataFile = FileUtil.GetCoverageRecord(covDataFile, rootDir);
            if (!covDataFile.SafeExists()) {
                return
                        Program.Print(
                                "Coverage data file doesn't exist.\ncoverage:"
                                        + covDataFile.FullName);
            }

            return Detect(rootDir, covDataFile, criterion);
        }

        private static bool Detect(DirectoryInfo rootDir, FileInfo covDataFile, string criterion) {
            var formatter = new BinaryFormatter();
            var testInfoPath = FileUtil.GetTestInfo(rootDir);
            var testInfo = TestInfo.ReadTestInfo(testInfoPath, formatter);
            testInfo.InitializeForStoringData(true);
            CoverageDataReader.ReadFile(testInfo, covDataFile);
            var testCases = testInfo.TestCases;

            Func<TestCase, TestCase, bool> isDuplicated;
            switch (criterion) {
            case "statement":
                isDuplicated = (tc, tc2) => tc.Statements.IsSubsetOf(tc2.Statements);
                break;
            case "decision":
            case "branch":
                isDuplicated = (tc, tc2) => tc.Decisions.IsSubsetOf(tc2.Decisions);
                break;
            case "condition":
                isDuplicated = (tc, tc2) => tc.Conditions.IsSubsetOf(tc2.Conditions);
                break;
            case "branch/condition":
            case "decision/condition":
            case "condition/branch":
            case "condition/decision":
                isDuplicated =
                        (tc, tc2) => tc.ConditionDecisions.IsSubsetOf(tc2.ConditionDecisions);
                break;
            case "subpath":
                isDuplicated = (tc, tc2) => tc.Paths.IsSubSequence(tc2.Paths);
                break;
            case "path":
                isDuplicated = (tc, tc2) => tc.Paths.SequenceEqual(tc2.Paths);
                break;
            default:
                throw new ArgumentException('"' + criterion + '"' + " is not supported.");
                break;
            }

            foreach (var tc in testCases) {
                var dups = testInfo.TestCases
                        .Where(tc2 => tc2 != tc && isDuplicated(tc, tc2));
                var tcStr = Environment.NewLine + tc.Name +
                        "(" + tc.RelativePath + ":" + tc.Position.SmartLineString + ")" +
                        " is duplicated with:" + Environment.NewLine;

                foreach (var dup in dups) {
                    var dupStr = dup.Name +
                            "(" + dup.RelativePath + ":" + dup.Position.SmartLineString
                            + ")";
                    Console.WriteLine(tcStr + "  " + dupStr);
                    tcStr = "";
                }
            }
            return true;
        }
    }
}