#region License

// Copyright (C) 2011-2012 Kazunori Sakamoto
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
using Occf.Core.Operators.Inserters;
using Occf.Core.TestInfos;

namespace Occf.Core.CoverageInformation {
    public class CoverageDataReader {
        public static void ReadFile(CoverageInfo covInfo, string dirPath) {
            using (var fs = new FileStream(dirPath, FileMode.Open)) {
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
                            var state = (CoverageState)(value & ((1 << 2) - 1));
                            var element = covInfo.Targets[id];
                            element.UpdateState(state);
                        }
                        break;
                    }
                }
            }
        }

        public static void ReadFile(TestInfo testInfo, string dirPath) {
            var testId = 0;
            using (var fs = new FileStream(dirPath, FileMode.Open)) {
                while (true) {
                    var id = (fs.ReadByte() << 24) + (fs.ReadByte() << 16) +
                             (fs.ReadByte() << 8) + (fs.ReadByte() << 0);
                    var value = fs.ReadByte();
                    if (value == -1) {
                        return;
                    }
                    switch ((ElementType)(value >> 2)) {
                    case ElementType.Statement:
                        testInfo.TestCases[testId].Statements.Add(id);
                        testInfo.TestCases[testId].StatementConditionDecisions.
                                Add(id);
                        testInfo.TestCases[testId].Paths.Add(id);
                        break;
                    case ElementType.Decision:
                        id = (value & 1) == 0 ? id : -id;
                        testInfo.TestCases[testId].Decisions.Add(id);
                        testInfo.TestCases[testId].ConditionDecisions.Add(id);
                        testInfo.TestCases[testId].StatementConditionDecisions.
                                Add(id);
                        break;
                    case ElementType.Condition:
                        id = (value & 1) == 0 ? id : -id;
                        testInfo.TestCases[testId].Conditions.Add(id);
                        testInfo.TestCases[testId].ConditionDecisions.Add(id);
                        testInfo.TestCases[testId].StatementConditionDecisions.
                                Add(id);
                        break;
                    case ElementType.DecisionAndCondition:
                        id = (value & 1) == 0 ? id : -id;
                        testInfo.TestCases[testId].Decisions.Add(id);
                        testInfo.TestCases[testId].Conditions.Add(id);
                        testInfo.TestCases[testId].ConditionDecisions.Add(id);
                        testInfo.TestCases[testId].StatementConditionDecisions.
                                Add(id);
                        break;
                    case ElementType.TestCase:
                        testId = id;
                        if (testInfo.TestCases.Count <= testId) {
                            throw new InvalidOperationException(
                                    "There is contradiction between the coverage data and the source code. Please retry to measure coverage data.");
                        }
                        break;
                    }
                }
            }
        }
    }
}