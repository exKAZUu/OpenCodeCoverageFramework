using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.CoverageInfos;
using Occf.Core.Extensions;
using Paraiba.Collections.Generic;

namespace Occf.Core.Operators.Inserters {
	public class CoverageInserter {
		public static void InsertImport(XElement root, NodeInserter nodeGen) {
			nodeGen.InsertImport(root);
		}

		public static void InsertStatement(
				CoverageInfo info, XElement root, CoverageProfile profile,
				string relativePath) {
			// ステートメントを挿入できるようにブロックを補う
			profile.NodeInserter.SupplementBlock(root);

			PrivateInsertStatement(info, root, profile, relativePath);
			PrivateInsertInitializer(info, root, profile, relativePath);
		}

		public static void InsertBranchAndCondition(
				CoverageInfo info,
				XElement root,
				CoverageProfile profile,
				string relativePath) {
			// switch文を正しく測定できるようにdefault節を追加する
			profile.NodeInserter.SupplementDefaultCase(root);

			var startBranchConditionIndex = info.TargetGroupList.Count;
			var branchNodeList = profile.BranchSelector.Select(root).ToList();
			InsertCondition(
					info, profile, relativePath, startBranchConditionIndex, branchNodeList);
			InsertBranch(
					info, profile.NodeInserter, startBranchConditionIndex, branchNodeList);
			InsertSwitchCase(info, root, profile, relativePath);
			InsertForeach(info, root, profile, relativePath);
		}

		public static void InsertStatementAndBranchAndCondition(
				CoverageInfo info, XElement root, CoverageProfile profile,
				string relativePath) {
			// ステートメントを挿入できるようにブロックを補う
			profile.NodeInserter.SupplementBlock(root);

			PrivateInsertStatement(info, root, profile, relativePath);

			// switch文を正しく測定できるようにdefault節を追加する
			profile.NodeInserter.SupplementDefaultCase(root);

			var startBranchConditionIndex = info.TargetGroupList.Count;
			var branchNodeList = profile.BranchSelector.Select(root).ToList();
			InsertCondition(
			        info, profile, relativePath, startBranchConditionIndex, branchNodeList);
			InsertBranch(
			        info, profile.NodeInserter, startBranchConditionIndex, branchNodeList);
			InsertSwitchCase(info, root, profile, relativePath);
			InsertForeach(info, root, profile, relativePath);

			PrivateInsertInitializer(info, root, profile, relativePath);
		}

		private static void PrivateInsertInitializer(
				CoverageInfo info, XElement root, CoverageProfile profile,
				string relativePath) {
			var statemetIndex = info.TargetList.Count;
			foreach (var node in profile.InitializerSelector.Select(root).ToList()) {
				// ステートメントに測定用コードを埋め込む
				profile.NodeInserter.InsertInitializer(
						node, info.TargetList.Count, ElementType.Statement);
				// カバレッジ情報を生成
				var covElem = new CoverageElement(relativePath, node, profile.Tagger);
				info.TargetList.Add(covElem);
			}
			info.StatementRanges.Add(
					Tuple.Create(statemetIndex, info.TargetList.Count));
		}

		private static void PrivateInsertStatement(
				CoverageInfo info, XElement root, CoverageProfile profile,
				string relativePath) {
			var statemetIndex = info.TargetList.Count;
			foreach (var node in profile.StatementSelector.Select(root).ToList()) {
				// ステートメントに測定用コードを埋め込む
				profile.NodeInserter.InsertStatementBefore(
						node, info.TargetList.Count, 2, ElementType.Statement);
				// カバレッジ情報を生成
				var covElem = new CoverageElement(relativePath, node, profile.Tagger);
				info.TargetList.Add(covElem);
			}
			info.StatementRanges.Add(
					Tuple.Create(statemetIndex, info.TargetList.Count));
		}

		private static void InsertForeach(
				CoverageInfo info, XElement node, CoverageProfile profile,
				string relativePath) {
			var startBranchIndex = info.TargetList.Count;
			var startBranchConditionIndex = info.TargetGroupList.Count;
			foreach (var foreachNode in profile.ForeachSelector.Select(node)) {
				// ステートメントに測定用コードを埋め込む
				var head = profile.ForeachHeadSelector.Select(foreachNode).First();
				var tail = profile.ForeachTailSelector.Select(foreachNode).First();
				profile.NodeInserter.InsertStatementBefore(
						tail, info.TargetList.Count, 2, ElementType.DecisionAndCondition);
				// ダミーカバレッジ情報を生成
				var dummyElem = new CoverageElement(
						relativePath, foreachNode, profile.Tagger);
				info.TargetList.Add(dummyElem);
				var count = info.TargetList.Count;
				profile.NodeInserter.InsertStatementAfter(
						head, count, 0, ElementType.DecisionAndCondition);
				profile.NodeInserter.InsertStatementAfter(
						tail, count, 1, ElementType.DecisionAndCondition);
				// カバレッジ情報を生成
				var covElem = new LoopCoverageElement(
						relativePath, foreachNode, profile.Tagger, dummyElem);
				info.TargetList.Add(covElem);
				var elementGroup = new CoverageElementGroup(
						covElem, new List<CoverageElement>());
				info.TargetGroupList.Add(elementGroup);
			}
			info.BranchRanges.Add(Tuple.Create(startBranchIndex, info.TargetList.Count));
			info.BranchConditionRanges.Add(
					Tuple.Create(
							startBranchConditionIndex,
							info.TargetGroupList.Count));
		}

		private static void InsertSwitchCase(
				CoverageInfo info, XElement node, CoverageProfile profile,
				string relativePath) {
			var startSwitchIndex = info.TargetGroupList.Count;
			foreach (var switchNode in profile.SwitchSelector.Select(node)) {
				var caseElements = new List<CoverageElement>();
				foreach (var caseNode in profile.CaseLabelTailSelector.Select(switchNode)) {
					// ステートメントに測定用コードを埋め込む
					profile.NodeInserter.InsertStatementAfter(
							caseNode, info.TargetList.Count, 2, ElementType.SwitchCase);
					// カバレッジ情報を生成
					var covElem = new CoverageElement(relativePath, caseNode, profile.Tagger);
					info.TargetList.Add(covElem);
					caseElements.Add(covElem);
				}
				// switchのカバレッジ情報を生成
				var element = new CoverageElement(relativePath, switchNode, profile.Tagger);
				// 条件分岐と論理項のカバレッジ情報をまとめる
				var elementGroup = new CoverageElementGroup(element, caseElements);
				info.TargetGroupList.Add(elementGroup);
			}
			info.SwitchRanges.Add(
					Tuple.Create(
							startSwitchIndex,
							info.TargetGroupList.Count));
		}

		private static void InsertCondition(
				CoverageInfo info, CoverageProfile profile,
				string relativePath, int startBranchConditionIndex,
				IEnumerable<XElement> branchNodeList) {
			foreach (var branchNode in branchNodeList) {
				// 全ての論理項を列挙
				var condNodeList = profile.ConditionSelector.Select(branchNode).ToList();
				// 論理項に測定用コードを埋め込み，カバレッジ情報を生成
				var condElements = InsertConditionCoverage(
						info, condNodeList, profile, relativePath);

				// 条件分岐のカバレッジ情報を生成
				var element = new CoverageElement(relativePath, branchNode, profile.Tagger);
				// 条件分岐と論理項のカバレッジ情報をまとめる
				var elementGroup = new CoverageElementGroup(element, condElements);
				info.TargetGroupList.Add(elementGroup);
			}
			info.BranchConditionRanges.Add(
					Tuple.Create(
							startBranchConditionIndex,
							info.TargetGroupList.Count));
		}

		private static void InsertBranch(
				CoverageInfo info, NodeInserter nodeGen, int startBranchConditionIndex,
				IEnumerable<XElement> branchNodeList) {
			var startBranchIndex = info.TargetList.Count;
			foreach (var branchNode in branchNodeList) {
				// 条件分岐が持つ論理項の集合を取得
				var covElemGrp = info.TargetGroupList[startBranchConditionIndex++];
				// 論理項を含む条件式か否かを判断
				nodeGen.InsertPredicate(
						branchNode, info.TargetList.Count,
						covElemGrp.Targets.Count > 0
								? ElementType.Decision : ElementType.DecisionAndCondition);

				info.TargetList.Add(covElemGrp.ParentElement);
			}
			info.BranchRanges.Add(Tuple.Create(startBranchIndex, info.TargetList.Count));
		}

		private static List<CoverageElement> InsertConditionCoverage(
				CoverageInfo info, ICollection<XElement> condNodeList,
				CoverageProfile profile,
				string relativePath) {
			return condNodeList
					.SelectToList(
							node => {
								profile.NodeInserter.InsertPredicate(
										node, info.TargetList.Count, ElementType.Condition);

								var covElem = new CoverageElement(relativePath, node, profile.Tagger);
								info.TargetList.Add(covElem);
								return covElem;
							});
		}
	}
}