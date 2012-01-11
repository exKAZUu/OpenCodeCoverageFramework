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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.CoverageInformation;
using Occf.Core.Extensions;
using Paraiba.Collections.Generic;

namespace Occf.Core.Operators.Inserters {
    public class CoverageInserter {
        public const int FalseOnly = ((int)CoverageState.FalseOnly) - 1;
        public const int TrueOnly = ((int)CoverageState.TrueOnly) - 1;
        public const int Done = ((int)CoverageState.Done) - 1;

        public static void InstrumentStatement(
                CoverageInfo info, XElement root, CoverageProfile profile,
                string relativePath) {
            // ステートメントを挿入できるようにブロックを補う
            profile.NodeInserter.SupplementBlock(root);

            AddIntoStatement(info, root, profile, relativePath);
            AddIntoVariableInitializer(info, root, profile, relativePath);

            // Add import for loggin executed items
            profile.NodeInserter.InsertImport(root);
        }

        public static void InstrumentPredicate(
                CoverageInfo info, XElement root, CoverageProfile profile,
                string relativePath) {
            // switch文を正しく測定できるようにdefault節を追加する
            profile.NodeInserter.SupplementDefaultCase(root);

            AddIntoBranchAndCondition(info, root, profile, relativePath);
            AddIntoSwitchCase(info, root, profile, relativePath);
            AddIntoForeach(info, root, profile, relativePath);

            // Add import for loggin executed items
            profile.NodeInserter.InsertImport(root);
        }

        public static void InstrumentStatementAndPredicate(
                CoverageInfo info, XElement root, CoverageProfile profile,
                string relativePath) {
            // ステートメントを挿入できるようにブロックを補う
            profile.NodeInserter.SupplementBlock(root);

            // switch文を正しく測定できるようにdefault節を追加する
            profile.NodeInserter.SupplementDefaultCase(root);

            AddIntoBranchAndCondition(info, root, profile, relativePath);
            AddIntoSwitchCase(info, root, profile, relativePath);
            AddIntoForeach(info, root, profile, relativePath);

            AddIntoStatement(info, root, profile, relativePath);
            AddIntoVariableInitializer(info, root, profile, relativePath);

            // Add import for loggin executed items
            profile.NodeInserter.InsertImport(root);
        }

        private static void AddIntoVariableInitializer(
                CoverageInfo info, XElement root, CoverageProfile profile,
                string relativePath) {
            var statemetIndex = info.Targets.Count;
            var nodeList = profile.InitializerSelector.Select(root).ToList();
            foreach (var node in nodeList) {
                // ステートメントに測定用コードを埋め込む
                profile.NodeInserter.InsertInitializer(
                        node, info.Targets.Count, ElementType.Statement);
                // カバレッジ情報を生成
                var covElem = new CoverageElement(
                        relativePath, node, profile.Tagger);
                info.Targets.Add(covElem);
            }
            info.StatementRanges.Add(
                    Tuple.Create(statemetIndex, info.Targets.Count));
        }

        private static void AddIntoStatement(
                CoverageInfo info, XElement root, CoverageProfile profile,
                string relativePath) {
            var statemetIndex = info.Targets.Count;
            var nodeList = profile.StatementSelector.Select(root).ToList();
            foreach (var node in nodeList) {
                // ステートメントに測定用コードを埋め込む
                profile.NodeInserter.InsertStatementBefore(
                        node, info.Targets.Count, Done, ElementType.Statement);
                // カバレッジ情報を生成
                var covElem = new CoverageElement(
                        relativePath, node, profile.Tagger);
                info.Targets.Add(covElem);
            }
            info.StatementRanges.Add(
                    Tuple.Create(statemetIndex, info.Targets.Count));
        }

        private static void AddIntoForeach(
                CoverageInfo info, XElement node, CoverageProfile profile,
                string relativePath) {
            var startBranchIndex = info.Targets.Count;
            var startBranchConditionIndex = info.TargetGroups.Count;
            var foreachNodes = profile.ForeachSelector.Select(node);
            foreach (var foreachNode in foreachNodes) {
                // ステートメントに測定用コードを埋め込む
                var head =
                        profile.ForeachHeadSelector.Select(foreachNode).First();
                var tail =
                        profile.ForeachTailSelector.Select(foreachNode).First();
                // 同じid( = count)を共有
                var id = info.Targets.Count;
                // DummyRecord(TrueOnly)
                // foreach() {
                //   Record(FalseOnly) ループ判定成立
                //   statement;
                //   DummyRecord(TrueOnly)
                // }
                // Record(TrueOnly)    ループ判定不成立（TrueOnlyが連続して）
                profile.NodeInserter.InsertStatementBefore(
                        head, id, TrueOnly, ElementType.DecisionAndCondition);
                profile.NodeInserter.InsertStatementAfter(
                        head, id, FalseOnly, ElementType.DecisionAndCondition);
                profile.NodeInserter.InsertStatementBefore(
                        tail, id, TrueOnly, ElementType.DecisionAndCondition);
                profile.NodeInserter.InsertStatementAfter(
                        tail, id, TrueOnly, ElementType.DecisionAndCondition);
                // カバレッジ情報を生成
                var covElem = new ForeachCoverageElement(
                        relativePath, foreachNode, profile.Tagger);
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
                CoverageInfo info, XElement node, CoverageProfile profile,
                string relativePath) {
            var startSwitchIndex = info.TargetGroups.Count;
            var switchNodes = profile.SwitchSelector.Select(node);
            foreach (var switchNode in switchNodes) {
                var caseElements = new List<CoverageElement>();
                var caseNodes = profile.CaseLabelTailSelector.Select(switchNode);
                foreach (var caseNode in caseNodes) {
                    // ステートメントに測定用コードを埋め込む
                    profile.NodeInserter.InsertStatementAfter(
                            caseNode, info.Targets.Count, Done,
                            ElementType.SwitchCase);
                    // カバレッジ情報を生成
                    var covElem = new CoverageElement(
                            relativePath, caseNode, profile.Tagger);
                    info.Targets.Add(covElem);
                    caseElements.Add(covElem);
                }
                // switchのカバレッジ情報を生成
                var element = new CoverageElement(
                        relativePath, switchNode, profile.Tagger);
                // 条件分岐と論理項のカバレッジ情報をまとめる
                var elementGroup = new CoverageElementGroup(
                        element, caseElements);
                info.TargetGroups.Add(elementGroup);
            }
            info.SwitchRanges.Add(
                    Tuple.Create(startSwitchIndex, info.TargetGroups.Count));
        }

        private static void AddIntoBranchAndCondition(
                CoverageInfo info, XElement node, CoverageProfile profile,
                string relativePath) {
            var branchNodeList = profile.BranchSelector.Select(node).ToList();
            var startBranchIndex = info.Targets.Count;
            var startBranchConditionIndex = info.TargetGroups.Count;
            foreach (var branchNode in branchNodeList) {
                // 全ての論理項を列挙
                var condNodeList =
                        profile.ConditionSelector.Select(branchNode).ToList();
                // 論理項に測定用コードを埋め込み，カバレッジ情報を生成
                var condElements = InsertConditionCoverage(
                        info, condNodeList, profile, relativePath);

                // 条件分岐のカバレッジ情報を生成
                var element = new CoverageElement(
                        relativePath, branchNode, profile.Tagger);
                // 条件分岐と論理項のカバレッジ情報をまとめる
                var elementGroup = new CoverageElementGroup(
                        element, condElements);
                info.TargetGroups.Add(elementGroup);

                // 論理項を含む条件式か否かを判断
                profile.NodeInserter.InsertPredicate(
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
                CoverageInfo info, XElement node, CoverageProfile profile,
                string relativePath) {
            var branchNodeList = profile.BranchSelector.Select(node).ToList();
            var startBranchIndex = info.Targets.Count;
            foreach (var branchNode in branchNodeList) {
                profile.NodeInserter.InsertPredicate(
                        branchNode, info.Targets.Count,
                        ElementType.Decision);

                // 条件分岐のカバレッジ情報を生成
                var element = new CoverageElement(
                        relativePath, branchNode, profile.Tagger);
                info.Targets.Add(element);
            }
            info.BranchRanges.Add(
                    Tuple.Create(startBranchIndex, info.Targets.Count));
        }

        private static List<CoverageElement> InsertConditionCoverage(
                CoverageInfo info, ICollection<XElement> condNodeList,
                CoverageProfile profile, string relativePath) {
            return condNodeList
                    .SelectToList(
                            node => {
                                profile.NodeInserter.InsertPredicate(
                                        node, info.Targets.Count,
                                        ElementType.Condition);

                                var covElem = new CoverageElement(
                                        relativePath, node, profile.Tagger);
                                info.Targets.Add(covElem);
                                return covElem;
                            });
        }
    }
}