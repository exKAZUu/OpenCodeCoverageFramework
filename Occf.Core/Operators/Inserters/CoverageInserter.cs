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
using Occf.Core.Profiles;
using Paraiba.Collections.Generic;

namespace Occf.Core.Operators.Inserters {
	public class CoverageInserter {
		public const int FalseOnly = ((int)CoverageState.FalseOnly) - 1;
		public const int TrueOnly = ((int)CoverageState.TrueOnly) - 1;
		public const int Done = ((int)CoverageState.Done) - 1;

		public static void InstrumentStatement(
				CoverageInfo info, XElement root, CoverageMode mode,
				string relativePath) {
			// ステートメントを挿入できるようにブロックを補う
			mode.NodeInserter.SupplementBlock(root);

			AddIntoStatement(info, root, mode, relativePath);
			AddIntoVariableInitializer(info, root, mode, relativePath);

			// Add import for loggin executed items
			mode.NodeInserter.InsertImport(root);
		}

		public static void InstrumentPredicate(
				CoverageInfo info, XElement root, CoverageMode mode,
				string relativePath) {
			// switch文を正しく測定できるようにdefault節を追加する
			mode.NodeInserter.SupplementDefaultCase(root);

			AddIntoBranchAndCondition(info, root, mode, relativePath);
			AddIntoSwitchCase(info, root, mode, relativePath);
			AddIntoForeach(info, root, mode, relativePath);

			// Add import for loggin executed items
			mode.NodeInserter.InsertImport(root);
		}

		public static void InstrumentStatementAndPredicate(
				CoverageInfo info, XElement root, CoverageMode mode,
				string relativePath) {
			// ステートメントを挿入できるようにブロックを補う
			mode.NodeInserter.SupplementBlock(root);

			// switch文を正しく測定できるようにdefault節を追加する
			mode.NodeInserter.SupplementDefaultCase(root);

			AddIntoStatement(info, root, mode, relativePath);
			AddIntoBranchAndCondition(info, root, mode, relativePath);

			// Add the measurement code as a statement into switch and foreach
			// after inserting the measurement code into statements
			AddIntoSwitchCase(info, root, mode, relativePath);
			AddIntoForeach(info, root, mode, relativePath);

			// Add the measurement code as a prediction into variable initializers
			// after inserting the measurement code into predictions
			AddIntoVariableInitializer(info, root, mode, relativePath);

			// Add import for loggin executed items
			mode.NodeInserter.InsertImport(root);
		}

		private static void AddIntoVariableInitializer(
				CoverageInfo info, XElement root, CoverageMode mode,
				string relativePath) {
			var statemetIndex = info.Targets.Count;
			var nodeList = mode.InitializerSelector.Select(root).ToList();
			foreach (var node in nodeList) {
				// ステートメントに測定用コードを埋め込む
				mode.NodeInserter.InsertInitializer(
						node, info.Targets.Count, ElementType.Statement);
				// カバレッジ情報を生成
				var covElem = new CoverageElement(
						relativePath, node, mode.Tagger);
				info.Targets.Add(covElem);
			}
			info.StatementRanges.Add(
					Tuple.Create(statemetIndex, info.Targets.Count));
		}

		private static void AddIntoStatement(
				CoverageInfo info, XElement root, CoverageMode mode,
				string relativePath) {
			var statemetIndex = info.Targets.Count;
			var nodeList = mode.StatementSelector.Select(root).ToList();
			foreach (var node in nodeList) {
				// ステートメントに測定用コードを埋め込む
				mode.NodeInserter.InsertStatementBefore(
						node, info.Targets.Count, Done, ElementType.Statement);
				// カバレッジ情報を生成
				var covElem = new CoverageElement(
						relativePath, node, mode.Tagger);
				info.Targets.Add(covElem);
			}
			info.StatementRanges.Add(
					Tuple.Create(statemetIndex, info.Targets.Count));
		}

		private static void AddIntoForeach(
				CoverageInfo info, XElement node, CoverageMode mode,
				string relativePath) {
			var startBranchIndex = info.Targets.Count;
			var startBranchConditionIndex = info.TargetGroups.Count;
			var foreachNodes = mode.ForeachSelector.Select(node);
			foreach (var foreachNode in foreachNodes) {
				// ステートメントに測定用コードを埋め込む
				var head =
						mode.ForeachHeadSelector.Select(foreachNode).First();
				var tail =
						mode.ForeachTailSelector.Select(foreachNode).First();
				// 同じid( = count)を共有
				var id = info.Targets.Count;
				// Record(TrueOnly)    _lastState = TrueOnly
				// foreach() {
				//   Record(FalseOnly) ループ判定成立 (State |= FalseOnly)
				//   statement;
				//   Record(TrueOnly)  _lastState = TrueOnly
				// }
				// Record(TrueOnly)    ループ判定不成立 (State |= TrueOnly if _lastState == TrueOnly)
				mode.NodeInserter.InsertStatementBefore(
						foreachNode, id, TrueOnly, ElementType.DecisionAndCondition);
				mode.NodeInserter.InsertStatementAfter(
						head, id, FalseOnly, ElementType.DecisionAndCondition);
				mode.NodeInserter.InsertStatementBefore(
						tail, id, TrueOnly, ElementType.DecisionAndCondition);
				mode.NodeInserter.InsertStatementAfter(
						tail, id, TrueOnly, ElementType.DecisionAndCondition);
				// カバレッジ情報を生成
				var covElem = new ForeachCoverageElement(
						relativePath, foreachNode, mode.Tagger);
				info.Targets.Add(covElem);
				var elementGroup = new CoverageElementGroup(covElem);
				info.TargetGroups.Add(elementGroup);
			}
			info.BranchRanges.Add(
					Tuple.Create(startBranchIndex, info.Targets.Count));
			info.BranchConditionRanges.Add(
					Tuple.Create(
							startBranchConditionIndex, info.TargetGroups.Count));
		}

		private static void AddIntoSwitchCase(
				CoverageInfo info, XElement node, CoverageMode mode,
				string relativePath) {
			var startSwitchIndex = info.TargetGroups.Count;
			var switchNodes = mode.SwitchSelector.Select(node);
			foreach (var switchNode in switchNodes) {
				var caseElements = new List<CoverageElement>();
				var caseNodes = mode.CaseLabelTailSelector.Select(switchNode);
				foreach (var caseNode in caseNodes) {
					// ステートメントに測定用コードを埋め込む
					mode.NodeInserter.InsertStatementAfter(
							caseNode, info.Targets.Count, Done,
							ElementType.SwitchCase);
					// カバレッジ情報を生成
					var covElem = new CoverageElement(
							relativePath, caseNode, mode.Tagger);
					info.Targets.Add(covElem);
					caseElements.Add(covElem);
				}
				// switchのカバレッジ情報を生成
				var element = new CoverageElement(
						relativePath, switchNode, mode.Tagger);
				// 条件分岐と論理項のカバレッジ情報をまとめる
				var elementGroup = new CoverageElementGroup(
						element, caseElements);
				info.TargetGroups.Add(elementGroup);
			}
			info.SwitchRanges.Add(
					Tuple.Create(startSwitchIndex, info.TargetGroups.Count));
		}

		private static void AddIntoBranchAndCondition(
				CoverageInfo info, XElement node, CoverageMode mode,
				string relativePath) {
			var branchNodeList = mode.BranchSelector.Select(node).ToList();
			var startBranchIndex = info.Targets.Count;
			var startBranchConditionIndex = info.TargetGroups.Count;
			foreach (var branchNode in branchNodeList) {
				// 全ての論理項を列挙
				var condNodeList =
						mode.ConditionSelector.Select(branchNode).ToList();
				// 論理項に測定用コードを埋め込み，カバレッジ情報を生成
				var condElements = InsertConditionCoverage(
						info, condNodeList, mode, relativePath);

				// 条件分岐のカバレッジ情報を生成
				var element = new CoverageElement(
						relativePath, branchNode, mode.Tagger);
				// 条件分岐と論理項のカバレッジ情報をまとめる
				var elementGroup = new CoverageElementGroup(
						element, condElements);
				info.TargetGroups.Add(elementGroup);

				// 論理項を含む条件式か否かを判断
				mode.NodeInserter.InsertPredicate(
						branchNode, info.Targets.Count,
						elementGroup.Targets.Count > 0
								? ElementType.Decision
								: ElementType.DecisionAndCondition);
				info.Targets.Add(element);
			}
			info.BranchConditionRanges.Add(
					Tuple.Create(
							startBranchConditionIndex, info.TargetGroups.Count));
			info.BranchRanges.Add(
					Tuple.Create(startBranchIndex, info.Targets.Count));
		}

		private static void AddIntoBranch(
				CoverageInfo info, XElement node, CoverageMode mode,
				string relativePath) {
			var branchNodeList = mode.BranchSelector.Select(node).ToList();
			var startBranchIndex = info.Targets.Count;
			foreach (var branchNode in branchNodeList) {
				mode.NodeInserter.InsertPredicate(
						branchNode, info.Targets.Count,
						ElementType.Decision);

				// 条件分岐のカバレッジ情報を生成
				var element = new CoverageElement(
						relativePath, branchNode, mode.Tagger);
				info.Targets.Add(element);
			}
			info.BranchRanges.Add(
					Tuple.Create(startBranchIndex, info.Targets.Count));
		}

		private static List<CoverageElement> InsertConditionCoverage(
				CoverageInfo info, ICollection<XElement> condNodeList,
				CoverageMode mode, string relativePath) {
			return condNodeList
					.SelectToList(
							node => {
								mode.NodeInserter.InsertPredicate(
										node, info.Targets.Count,
										ElementType.Condition);

								var covElem = new CoverageElement(
										relativePath, node, mode.Tagger);
								info.Targets.Add(covElem);
								return covElem;
							});
		}
	}
}