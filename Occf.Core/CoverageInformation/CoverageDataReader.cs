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
using System.IO;
using System.Text;
using Occf.Core.CoverageInformation.Elements;
using Occf.Core.Manipulators.Transformers;
using Occf.Core.TestInformation;

namespace Occf.Core.CoverageInformation {
    public class CoverageDataReader {
        public static void ReadFile(CoverageInfo covInfo, FileInfo fileInfo) {
            using (var fs = new FileStream(fileInfo.FullName, FileMode.Open)) {
                if (IsTextFile(fs)) {
                    ReadTextFile(covInfo, fs);
                } else {
                    ReadBinaryFile(covInfo, fs);
                }
            }
        }

        private static void ReadTextFile(CoverageInfo covInfo, FileStream fs) {
            using (var reader = new StreamReader(fs)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    var items = line.Split(' ');
                    var element = covInfo.Targets[int.Parse(items[0])];
                    var state = (CoverageState)(int.Parse(items[1]));
                    element.UpdateState(state);
                }
            }
        }

        private static void ReadBinaryFile(CoverageInfo covInfo, Stream fs) {
            while (true) {
                var id = (fs.ReadByte() << 24) + (fs.ReadByte() << 16) +
                        (fs.ReadByte() << 8) + (fs.ReadByte() << 0);
                var value = fs.ReadByte();
                if (value == -1) {
                    return;
                }
                switch ((ElementType)(value >> 2)) {
                case ElementType.TestCase:
                    break;
                default:
                    if (0 <= id && id < covInfo.Targets.Count) {
                        var element = covInfo.Targets[id];
                        var state = (CoverageState)(value & ((1 << 2) - 1));
                        element.UpdateState(state);
                    }
                    break;
                }
            }
        }

        private static bool IsTextFile(Stream fs) {
            var buffer = new byte[100];
            fs.Read(buffer, 0, buffer.Length);
            var str = Encoding.Unicode.GetString(buffer);
            var items = str.Split(new[] {' ', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries );
            var dummy = 0;
            return int.TryParse(items[0], out dummy) && int.TryParse(items[1], out dummy);
        }

        public static void ReadFile(TestInfo testInfo, FileInfo fileInfo) {
            ReadFile(testInfo, fileInfo.FullName,
                    testInfo.TestCases.Count > 0 ? testInfo.TestCases[0] : null);
        }

        public static void ReadFile(TestInfo testInfo, string filePath, TestCase initialTestCase) {
            var testCase = initialTestCase;
            using (var fs = new FileStream(filePath, FileMode.Open)) {
                while (true) {
                    var id = (fs.ReadByte() << 24) + (fs.ReadByte() << 16) +
                            (fs.ReadByte() << 8) + (fs.ReadByte() << 0);
                    var value = fs.ReadByte();
                    if (value == -1) {
                        return;
                    }
                    switch ((ElementType)(value >> 2)) {
                    case ElementType.Statement:
                        testCase.Statements.Add(id);
                        testCase.StatementConditionDecisions.Add(id);
                        testCase.Paths.Add(id);
                        break;
                    case ElementType.Decision:
                        id = (value & 1) == 0 ? id : -id;
                        testCase.Decisions.Add(id);
                        testCase.ConditionDecisions.Add(id);
                        testCase.StatementConditionDecisions.Add(id);
                        break;
                    case ElementType.Condition:
                        id = (value & 1) == 0 ? id : -id;
                        testCase.Conditions.Add(id);
                        testCase.ConditionDecisions.Add(id);
                        testCase.StatementConditionDecisions.Add(id);
                        break;
                    case ElementType.DecisionAndCondition:
                        id = (value & 1) == 0 ? id : -id;
                        testCase.Decisions.Add(id);
                        testCase.Conditions.Add(id);
                        testCase.ConditionDecisions.Add(id);
                        testCase.StatementConditionDecisions.Add(id);
                        break;
                    case ElementType.TestCase:
                        if (testInfo.TestCases.Count <= id) {
                            throw new InvalidOperationException(
                                    "There is contradiction between the coverage data and the source code. Please retry to measure coverage data.");
                        }
                        testCase = testInfo.TestCases[id];
                        break;
                    }
                }
            }
        }
    }
}