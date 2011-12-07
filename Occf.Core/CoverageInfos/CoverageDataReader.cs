using System;
using System.IO;
using Occf.Core.Operators.Inserters;
using Occf.Core.TestInfos;

namespace Occf.Core.CoverageInfos {
	public class CoverageDataReader {
		public static void ReadFile(CoverageInfo covInfo, string dirPath) {
			CoverageElement lastElement = null;
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
						if (0 <= id && id < covInfo.TargetList.Count) {
							var state = (CoverageState)(value & ((1 << 2) - 1));
							var element = covInfo.TargetList[id];
							element.UpdateState(lastElement, state);
							lastElement = element;
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
						testInfo.TestCases[testId].StatementConditionDecisions.Add(id);
						testInfo.TestCases[testId].Paths.Add(id);
						break;
					case ElementType.Decision:
						id = (value & 1) == 0 ? id : -id;
						testInfo.TestCases[testId].Decisions.Add(id);
						testInfo.TestCases[testId].ConditionDecisions.Add(id);
						testInfo.TestCases[testId].StatementConditionDecisions.Add(id);
						break;
					case ElementType.Condition:
						id = (value & 1) == 0 ? id : -id;
						testInfo.TestCases[testId].Conditions.Add(id);
						testInfo.TestCases[testId].ConditionDecisions.Add(id);
						testInfo.TestCases[testId].StatementConditionDecisions.Add(id);
						break;
					case ElementType.DecisionAndCondition:
						id = (value & 1) == 0 ? id : -id;
						testInfo.TestCases[testId].Decisions.Add(id);
						testInfo.TestCases[testId].Conditions.Add(id);
						testInfo.TestCases[testId].ConditionDecisions.Add(id);
						testInfo.TestCases[testId].StatementConditionDecisions.Add(id);
						break;
					case ElementType.TestCase:
						testId = id;
						if (testInfo.TestCases.Count <= testId) {
							throw new InvalidOperationException("There is contradiction between the coverage data and the source code. Please retry to measure coverage data.");
						}
						break;
					}
				}
			}
		}
	}
}