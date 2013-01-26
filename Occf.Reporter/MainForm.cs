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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Occf.Core.CoverageInformation;
using Occf.Core.CoverageInformation.Elements;
using Occf.Reader.SharedMemory;
using Paraiba.IO;
using Paraiba.Linq;
using Paraiba.Windows.Forms;

namespace Occf.Reporter {
	public partial class MainForm : Form {
		private CoverageInfo _info;
		private Point _lastMouseLocation;

		public MainForm()
				: this(null) {}

		public MainForm(string[] args) {
			InitializeComponent();
			if (args != null && args.Length > 1) {
				LoadCoverageInfomation(args[1]);
			}
		}

		private void BtnAnalyzeClick(object sender, EventArgs e) {
			var robustTags = _lbTag.SelectedItems
					.Cast<object>()
					.Select(item => _lbTag.GetItemText(item))
					.ToList();
			// 包含関係にあるタグの最適化
			var tags = robustTags
					.Where(t1 => robustTags.All(t2 => t1 == t2 || !t1.StartsWith(t2)))
					.ToList();

			// 命令網羅
			AnalyzeCoverage(
					_info.StatementTargets, tags, _lvStatement, _pgbStatement,
					_lbStatement);

			// 分岐網羅
			AnalyzeCoverage(_info.BranchTargets, tags, _lvBranch, _pgbBranch, _lbBranch);

			// 条件網羅 & 判定条件網羅
			AnalyzeBranchConditionCoverage(_info.BranchConditionTargets, tags);
		}

		private static void AnalyzeCoverage(
				IEnumerable<ICoverageElement> coverageTargets, IEnumerable<string> tags,
				ListView listView, ProgressBar pgbResult, Label lbResult) {
			listView.Items.Clear();
			int nAll = 0, nCoveraged = 0;
			var targets = coverageTargets
					.Where(target => tags.Any(tag => target.Tag.StartsWith(tag)));
			foreach (var target in targets) {
				nAll++;
				if (target.State == CoverageState.Done) {
					nCoveraged++;
					listView.Items.Add(new CoverageListViewItem("Y", target));
				} else {
					listView.Items.Add(new CoverageListViewItem("N", target));
				}
			}
			if (nAll == 0) {
				pgbResult.Value = 100;
			} else {
				pgbResult.Value = 100 * nCoveraged / nAll;
			}
			lbResult.Text = pgbResult.Value + "% : " + nCoveraged + " / " + nAll;
		}

		private void AnalyzeBranchConditionCoverage(
				IEnumerable<CoverageElementGroup> coverageTargets, IEnumerable<string> tags) {
			_lvCondition.Items.Clear();
			_lvBranchCond.Items.Clear();

			int nAll = 0, nCoveragedCondition = 0, nCoveragedBranchCond = 0;
			var targets = coverageTargets
					.Where(target => tags.Any(tag => target.Tag.StartsWith(tag)));
			foreach (var target in targets) {
				nAll++;
				if (target.StateChildrenOrParent == CoverageState.Done) {
					nCoveragedCondition++;
					_lvCondition.Items.Add(new CoverageListViewItem("Y", target));
					if (target.ParentElement.State == CoverageState.Done) {
						nCoveragedBranchCond++;
						_lvBranchCond.Items.Add(new CoverageListViewItem("Y", target));
						continue;
					}
				} else {
					_lvCondition.Items.Add(new CoverageListViewItem("N", target));
				}
				_lvBranchCond.Items.Add(new CoverageListViewItem("N", target));
			}
			if (nAll == 0) {
				_pgbBranchCond.Value = _pgbCondition.Value = 100;
			} else {
				_pgbCondition.Value = 100 * nCoveragedCondition / nAll;
				_pgbBranchCond.Value = 100 * nCoveragedBranchCond / nAll;
			}
			_lbCondition.Text = _pgbCondition.Value + "% : " + nCoveragedCondition +
					" / " + nAll;
			_lbBranchCond.Text = _pgbBranchCond.Value + "% : " + nCoveragedBranchCond +
					" / " + nAll;
		}

		private void LoadCoverageInfomation(string filePath) {
			_txtCovInfoPath.Text = filePath;

			// カバレッジ情報（母数）の取得
			_info = CoverageInfo.ReadCoverageInfo(new FileInfo(filePath));

			// タグを構成要素に分解して再構成する
			var tagSet = _info.Targets.Select(t => t.Tag).ToHashSet();
			var newTagSet = new SortedSet<string>();

			foreach (var tag in tagSet) {
				var tagElements = tag.Split(
						new[] { '>' },
						StringSplitOptions.RemoveEmptyEntries);
				var newTag = string.Empty;
				foreach (var tagEelment in tagElements) {
					newTag += tagEelment + '>';
					newTagSet.Add(newTag);
				}
			}

			_lbTag.Items.Clear();
			foreach (var tag in newTagSet) {
				_lbTag.Items.Add(tag);
			}

			switch (_info.SharingMethod) {
			case SharingMethod.SharedMemory:
				SharedMemoryReporter.Initialize(_info.Targets.Count);
				break;
			case SharingMethod.TcpIp:
				break;
			case SharingMethod.File:
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void MainForm_DragEnter(object sender, DragEventArgs e) {
			e.Effect = DragDropEffects.All;
		}

		private void MainForm_DragDrop(object sender, DragEventArgs e) {
			if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
				return;
			}
			foreach (var fileName in (string[])e.Data.GetData(DataFormats.FileDrop)) {
				LoadCoverageInfomation(fileName);
				break;
			}
		}

		private void BtnMeasureClick(object sender, EventArgs e) {
			var coverageElements = _info.Targets;
			var count = coverageElements.Count;
			for (int i = 0; i < count; i++) {
				coverageElements[i].UpdateState(SharedMemoryReporter.Read(i));
			}

			_lbTag.SelectAll();
			_btnAnalyze.PerformClick();
		}

		private void LvStatementDoubleClick(object sender, EventArgs e) {
			var item =
					(CoverageListViewItem)
							_lvStatement.GetItemAt(_lastMouseLocation.X, _lastMouseLocation.Y);
			var element = item.Element;
			var path = ParaibaPath.GetFullPath(element.RelativePath, _info.BasePath);
			new Editor(
					path, _info.StatementTargets
							      .Where(
									      elm => elm.RelativePath == element.RelativePath
											      && elm.State != CoverageState.Done)).Show();
		}

		private void LvStatementMouseDown(object sender, MouseEventArgs e) {
			_lastMouseLocation = e.Location;
		}

		#region Nested type: CoverageListViewItem

		public class CoverageListViewItem : ListViewItem {
			private readonly ICoverageElement _element;

			public CoverageListViewItem(string coveragedText, ICoverageElement element)
					: base(
							new[] {
									coveragedText, element.RelativePath, element.Position.SmartLineString,
									element.Position.SmartPositionString
							}) {
				_element = element;
			}

			public ICoverageElement Element {
				get { return _element; }
			}
		}

		#endregion
	}
}