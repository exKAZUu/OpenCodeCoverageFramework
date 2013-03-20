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
using System.Linq;
using System.Xml.Linq;
using Occf.Core.CoverageInformation;
using Occf.Core.CoverageInformation.Elements;
using Occf.Core.TestInformation;
using Paraiba.Collections.Generic;

namespace Occf.Core.Manipulators.Transformers {
	public static class CodeTransformer {
		public const int FalseOnly = ((int)CoverageState.FalseOnly) - 1;
		public const int TrueOnly = ((int)CoverageState.TrueOnly) - 1;
		public const int Done = ((int)CoverageState.Done) - 1;

		public static void InstrumentStatement(
				CoverageInfo info, XElement root, LanguageSupport support, string relativePath) {
			// ステートメントを挿入できるようにブロックを補う
			support.AstTransformer.SupplementBlock(root);

			InsertIntoStatement(info, root, support, relativePath);
			InsertIntoVariableInitializer(info, root, support, relativePath);

			// Add import for loggin executed items
			support.AstTransformer.InsertImport(root);
		}

		public static void InstrumentPredicate(
				CoverageInfo info, XElement root, LanguageSupport support, string relativePath) {
			// switch文を正しく測定できるようにdefault節を追加する
			support.AstTransformer.SupplementDefaultCase(root);

			InsertIntoBranchAndCondition(info, root, support, relativePath);
			InsertIntoSwitchCase(info, root, support, relativePath);
			InsertIntoForeach(info, root, support, relativePath);

			// Add import for loggin executed items
			support.AstTransformer.InsertImport(root);
		}

		public static void InstrumentStatementAndPredicate(
				CoverageInfo info, XElement root, LanguageSupport support, string relativePath) {
			// ステートメントを挿入できるようにブロックを補う
			support.AstTransformer.SupplementBlock(root);

			// switch文を正しく測定できるようにdefault節を追加する
			support.AstTransformer.SupplementDefaultCase(root);

			InsertIntoStatement(info, root, support, relativePath);
			InsertIntoBranchAndCondition(info, root, support, relativePath);

			// Add the measurement code as a statement into switch and foreach
			// after inserting the measurement code into statements
			InsertIntoSwitchCase(info, root, support, relativePath);
			InsertIntoForeach(info, root, support, relativePath);

			// Add the measurement code as a prediction into variable initializers
			// after inserting the measurement code into predictions
			InsertIntoVariableInitializer(info, root, support, relativePath);

			// Add import for loggin executed items
			support.AstTransformer.InsertImport(root);
		}

		private static void InsertIntoVariableInitializer(
				CoverageInfo info, XElement root, LanguageSupport support, string relativePath) {
			var statemetIndex = info.Targets.Count;
			var nodes = support.AstAnalyzer.FindVariableInitializers(root);
			foreach (var node in nodes.ToList()) {
				// ステートメントに測定用コードを埋め込む
				support.AstTransformer.InsertInitializer(node, info.Targets.Count, ElementType.Statement);
				// カバレッジ情報を生成
				var covElem = new CoverageElement(relativePath, node, support.Tagger);
				info.Targets.Add(covElem);
			}
			info.StatementRanges.Add(Tuple.Create(statemetIndex, info.Targets.Count));
		}

		private static void InsertIntoStatement(
				CoverageInfo info, XElement root, LanguageSupport support, string relativePath) {
			var statemetIndex = info.Targets.Count;
			var nodes = support.AstAnalyzer.FindStatements(root);
			foreach (var node in nodes.ToList()) {
				// ステートメントに測定用コードを埋め込む
			    var posNode = support.AstAnalyzer.GetBaseElementForStatement(node);
				support.AstTransformer.InsertStatementBefore(
						posNode, info.Targets.Count, Done, ElementType.Statement);
				// カバレッジ情報を生成
				var covElem = new CoverageElement(relativePath, node, support.Tagger);
				info.Targets.Add(covElem);
			}
			info.StatementRanges.Add(Tuple.Create(statemetIndex, info.Targets.Count));
		}

		private static void InsertIntoForeach(
				CoverageInfo info, XElement node, LanguageSupport support, string relativePath) {
			var startBranchIndex = info.Targets.Count;
			var startBranchConditionIndex = info.TargetGroups.Count;
			var analyzer = support.AstAnalyzer;
			var transformer = support.AstTransformer;
			var foreachNodes = analyzer.FindForeach(node);
			foreach (var foreachNode in foreachNodes.ToList()) {
				// ステートメントに測定用コードを埋め込む
				var head = analyzer.FindForeachHead(foreachNode).First();
				var tail = analyzer.FindForeachTail(foreachNode).First();
				// 同じid( = count)を共有
				var id = info.Targets.Count;
				// Record(TrueOnly)    _lastState = TrueOnly
				// foreach() {
				//   Record(FalseOnly) ループ判定成立 (State |= FalseOnly)
				//   statement;
				//   Record(TrueOnly)  _lastState = TrueOnly
				// }
				// Record(TrueOnly)    ループ判定不成立 (State |= TrueOnly if _lastState == TrueOnly)
				transformer.InsertStatementBefore(
						foreachNode, id, TrueOnly, ElementType.DecisionAndCondition);
				transformer.InsertStatementAfter(
						head, id, FalseOnly, ElementType.DecisionAndCondition);
				transformer.InsertStatementBefore(
						tail, id, TrueOnly, ElementType.DecisionAndCondition);
				transformer.InsertStatementAfter(
						tail, id, TrueOnly, ElementType.DecisionAndCondition);
				// カバレッジ情報を生成
				var covElem = new ForeachCoverageElement(relativePath, foreachNode, support.Tagger);
				info.Targets.Add(covElem);
				var elementGroup = new CoverageElementGroup(covElem);
				info.TargetGroups.Add(elementGroup);
			}
			info.BranchRanges.Add(Tuple.Create(startBranchIndex, info.Targets.Count));
			info.BranchConditionRanges.Add(Tuple.Create(startBranchConditionIndex, info.TargetGroups.Count));
		}

		private static void InsertIntoSwitchCase(
				CoverageInfo info, XElement node, LanguageSupport support, string relativePath) {
			var analyzer = support.AstAnalyzer;
			var startSwitchIndex = info.TargetGroups.Count;
			var switchNodes = analyzer.FindSwitches(node);
			foreach (var switchNode in switchNodes.ToList()) {
				var caseElements = new List<CoverageElement>();
				var caseNodes = analyzer.FindCaseLabelTails(switchNode);
				foreach (var caseNode in caseNodes.ToList()) {
					// ステートメントに測定用コードを埋め込む
					support.AstTransformer.InsertStatementAfter(
							caseNode, info.Targets.Count, Done, ElementType.SwitchCase);
					// カバレッジ情報を生成
					var covElem = new CoverageElement(relativePath, caseNode, support.Tagger);
					info.Targets.Add(covElem);
					caseElements.Add(covElem);
				}
				// switchのカバレッジ情報を生成
				var element = new CoverageElement(relativePath, switchNode, support.Tagger);
				// 条件分岐と論理項のカバレッジ情報をまとめる
				var elementGroup = new CoverageElementGroup(element, caseElements);
				info.TargetGroups.Add(elementGroup);
			}
			info.SwitchRanges.Add(Tuple.Create(startSwitchIndex, info.TargetGroups.Count));
		}

		private static void InsertIntoBranchAndCondition(
				CoverageInfo info, XElement node, LanguageSupport support, string relativePath) {
			var analyzer = support.AstAnalyzer;
			var branchNodes = analyzer.FindBranches(node);
			var startBranchIndex = info.Targets.Count;
			var startBranchConditionIndex = info.TargetGroups.Count;
			foreach (var branchNode in branchNodes.ToList()) {
				// 全ての論理項を列挙
				var condNodeList = analyzer.FindConditions(branchNode).ToList();
				// 論理項に測定用コードを埋め込み，カバレッジ情報を生成
				var condElements = InsertIntoConditionCoverage(info, condNodeList, support, relativePath);

				// 条件分岐のカバレッジ情報を生成
				var element = new CoverageElement(relativePath, branchNode, support.Tagger);
				// 条件分岐と論理項のカバレッジ情報をまとめる
				var elementGroup = new CoverageElementGroup(element, condElements);
				info.TargetGroups.Add(elementGroup);

				// 論理項を含む条件式か否かを判断
				support.AstTransformer.InsertPredicate(
						branchNode, info.Targets.Count,
						elementGroup.Targets.Count > 0
								? ElementType.Decision
								: ElementType.DecisionAndCondition);
				info.Targets.Add(element);
			}
			info.BranchConditionRanges.Add(Tuple.Create(startBranchConditionIndex, info.TargetGroups.Count));
			info.BranchRanges.Add(Tuple.Create(startBranchIndex, info.Targets.Count));
		}

		private static void InsertIntoBranch(
				CoverageInfo info, XElement node, LanguageSupport support, string relativePath) {
			var branchNodes = support.AstAnalyzer.FindBranches(node);
			var startBranchIndex = info.Targets.Count;
			foreach (var branchNode in branchNodes.ToList()) {
				support.AstTransformer.InsertPredicate(branchNode, info.Targets.Count, ElementType.Decision);

				// 条件分岐のカバレッジ情報を生成
				var element = new CoverageElement(relativePath, branchNode, support.Tagger);
				info.Targets.Add(element);
			}
			info.BranchRanges.Add(Tuple.Create(startBranchIndex, info.Targets.Count));
		}

		private static List<CoverageElement> InsertIntoConditionCoverage(
				CoverageInfo info, ICollection<XElement> condNodeList, LanguageSupport support,
				string relativePath) {
			return condNodeList
					.SelectToList(
							node => {
								support.AstTransformer.InsertPredicate(node, info.Targets.Count, ElementType.Condition);

								var covElem = new CoverageElement(relativePath, node, support.Tagger);
								info.Targets.Add(covElem);
								return covElem;
							});
		}

		public static void InsertIntoTestCase(
				TestInfo info, XElement root, LanguageSupport support, string relativePath) {
			var nodes = support.AstAnalyzer.FindTestCases(root);
			var trans = support.AstTransformer;
			foreach (var node in nodes.ToList()) {
				var id = info.TestCases.Count;
				var testCase = trans.InsertTestCaseId(node, id, relativePath);
				info.TestCases.Add(testCase);
			}
		}
	}
}