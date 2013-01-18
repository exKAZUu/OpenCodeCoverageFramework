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
using Occf.Core.CoverageInformation.Elements;
using Occf.Core.Manipulators.Transformers;
using Occf.Core.TestInformation;

namespace Occf.Core.CoverageInformation {
	public class CoverageDataReader {
		public static void ReadFile(CoverageInfo covInfo, FileInfo fileInfo) {
			using (var fs = new FileStream(fileInfo.FullName, FileMode.Open)) {
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

		public static void ReadFile(TestInfo testInfo, FileInfo fileInfo) {
			ReadFile(
					testInfo, fileInfo.FullName, testInfo.TestCases.Count > 0 ? testInfo.TestCases[0] : null);
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