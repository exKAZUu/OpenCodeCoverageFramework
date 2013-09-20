using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.Location;
using Code2Xml.Core.Plugin;
using Code2Xml.Languages.JavaScript.CodeToXmls;
using Occf.Learner.Core;
using Sgry.Azuki;
using Sgry.Azuki.Highlighter;

namespace Occf.Learner.Tool {
	public partial class MainForm : Form {
		private static readonly int[] MaxLengths = { int.MaxValue / 2, 4, 8 };
		private CodeToXml _activeCodeToXml;
		private CodeFile _activeFile;
		private ListViewItem _lastSelectedItem;

		public string Code {
			get { return azuki.Text; }
			set { azuki.Text = value; }
		}

		public MainForm() {
			InitializeComponent();
			Marking.Register(new MarkingInfo(0, "selected"));
			Marking.Register(new MarkingInfo(1, "selected_head"));
			Marking.Register(new MarkingInfo(2, "infered_head"));
			azuki.ColorScheme.SetMarkingDecoration(0,
					new BgColorTextDecoration(Color.Yellow));
			azuki.ColorScheme.SetMarkingDecoration(1,
					new UnderlineTextDecoration(LineStyle.Solid, Color.Red));
			azuki.ColorScheme.SetMarkingDecoration(2,
					new UnderlineTextDecoration(LineStyle.Waved, Color.Blue));
			Activate();
		}

		private void MainForm_Load(object sender, EventArgs e) {
			var menuItems = new List<ToolStripMenuItem>();
			foreach (var codeToXml in PluginManager.CodeToXmls) {
				var menuItem =
						menuItemLanguage.DropDownItems.Add(codeToXml.ParserName) as ToolStripMenuItem;
				menuItems.Add(menuItem);
				var codeToXmlForClosure = codeToXml;
				menuItem.Click += (o, args) => {
					_activeCodeToXml = codeToXmlForClosure;
					foreach (var item in menuItems) {
						item.Checked = false;
					}
					menuItem.Checked = true;

					Code = "";
					_activeFile = null;
					lvModifyingFile.Items.Clear();
					lvFreezingFile.Items.Clear();
					lvRule.Items.Clear();
					lvFreezingRule.Items.Clear();
					lvWillBeMarkedElements.Items.Clear();
					lvMarkedElements.Items.Clear();
				};
				if (codeToXml is JavaScriptCodeToXml) {
					menuItem.PerformClick();
				}
			}
		}

		private void btnInfer_Click(object sender, EventArgs e) {
			NormalizeAllRange2Element();
			var datas = lvModifyingFile.Items.Cast<FileListViewItem>()
					.Concat(lvFreezingFile.Items.Cast<FileListViewItem>())
					.Select(item => item.File)
					.Select(f => new LearningData(f.Ast, f.Range2Elements.Values))
					.ToList();
			var filters = RuleLearner.Learn(datas).ToList();
			lvRule.Items.Clear();
			foreach (var filter in filters) {
				var item = new FilterListViewItem(filter);
				lvRule.Items.Add(item);
				item.Checked = filter is NopFilter;
			}
		}

		private void btnApply_Click(object sender, EventArgs e) {
			var rule = InferRule();
			_activeFile.Range2Elements = ExtractRange2Elements(_activeFile, rule);
			NormalizeAllRange2Element();
			RedrawActiveFile();
		}

		private void btnApplyAll_Click(object sender, EventArgs e) {
			var rule = InferRule();
			foreach (var file in lvModifyingFile.Items.Cast<FileListViewItem>().Select(i => i.File)) {
				file.Range2Elements = ExtractRange2Elements(file, rule);
			}
			NormalizeAllRange2Element();
			RedrawActiveFile();
		}

		private void markToolStripMenuItem_Click(object sender, EventArgs e) {
			// Calculate the selected range
			var selectedRange = GetSelectedRange();
			// Get the selected element
			// TODO: Should be use FindInnerElement or FindOuterElement ?
			var element = selectedRange.FindOuterElement(_activeFile.Ast);
			var range = CodeRange.Locate(element);
			if (!_activeFile.Range2Elements.ContainsKey(range)) {
				_activeFile.Range2Elements.Add(range, element);
				DrawMarkers(_activeFile.Range2Elements, 0, 1);
				ListElements(_activeFile.Range2Elements, lvMarkedElements);
			}
		}

		private void unmarkToolStripMenuItem_Click(object sender, EventArgs e) {
			var selectedRange = GetSelectedRange();
			var range = _activeFile.Range2Elements.Keys.Where(r => r.Contains(selectedRange))
					.OrderBy(r => (r.EndLine - r.StartLine) * 65536 + (r.EndPosition - r.StartPosition))
					.FirstOrDefault();
			if (range == default(CodeRange)) {
				return;
			}
			_activeFile.Range2Elements.Remove(range);
			DrawMarkers(_activeFile.Range2Elements, 0, 1);
			ListElements(_activeFile.Range2Elements, lvMarkedElements);
		}

		private void azuki_DragEnter(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
				e.Effect = DragDropEffects.Copy;
			} else {
				e.Effect = DragDropEffects.None;
			}
		}

		private void azuki_DragDrop(object sender, DragEventArgs e) {
			var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			var count = lvModifyingFile.Items.Count;
			foreach (var fileName in fileNames) {
				var file = new FileInfo(fileName);
				var codeFile = new CodeFile(_activeCodeToXml, file);
				var item = new FileListViewItem(codeFile);
				lvModifyingFile.Items.Add(item);
			}
			lvModifyingFile.Items[count].Selected = true;
		}

		private void lvModifyingFile_ItemSelectionChanged(
				object sender, ListViewItemSelectionChangedEventArgs e) {
			var item = e.Item as FileListViewItem;
			if (item != null && item != _lastSelectedItem) {
				_lastSelectedItem = item;
				ChangeActiveFile(item.File);
			}
		}

		private void ChangeActiveFile(CodeFile activeFile) {
			_activeFile = activeFile;
			Code = _activeFile.Code;
			RedrawActiveFile();
			var rule = InferRule();
			var range2Elements = ExtractRange2Elements(_activeFile, rule);
			DrawMarkers(range2Elements, 2);
			ListElements(range2Elements, lvWillBeMarkedElements);
		}

		private void PairedListView_MouseClick(object sender, MouseEventArgs e) {
			var src = (ListView)sender;
			ListView dst;
			if (src == lvModifyingFile) {
				dst = lvFreezingFile;
			} else if (src == lvFreezingFile) {
				dst = lvModifyingFile;
			} else if (src == lvRule) {
				dst = lvFreezingRule;
			} else {
				dst = lvRule;
			}

			if (e.Button == MouseButtons.Right) {
				foreach (ListViewItem item in src.SelectedItems) {
					item.Remove();
					dst.Items.Add(item);
				}
			}
		}

		private void lvElements_MouseDoubleClick(object sender, MouseEventArgs e) {
			var listView = (ListView)sender;
			var item = listView.SelectedItems.Cast<ElementListViewItem>().FirstOrDefault();
			if (item != null) {
				var indicies = item.Range.ConvertToIndicies(Code);
				azuki.SetSelection(indicies.Item1, indicies.Item2);
			}
		}

		private void lvRule_ItemChecked(object sender, ItemCheckedEventArgs e) {
			var rule = InferRule();
			var range2Elements = ExtractRange2Elements(_activeFile, rule);
			DrawMarkers(range2Elements, 2);
			ListElements(range2Elements, lvWillBeMarkedElements);
		}

		private void lvFile_Click(object sender, EventArgs e) {
			var listView = (ListView)sender;
			if (listView.SelectedItems.Count == 0) {
				return;
			}
			var item = listView.SelectedItems.Cast<FileListViewItem>()
				.FirstOrDefault();
			if (item != null && item != _lastSelectedItem) {
				_lastSelectedItem = item;
				ChangeActiveFile(item.File);
			}
		}

		#region Helper Methods

		private ExtractingRule InferRule() {
			var filters = lvRule.CheckedItems.Cast<FilterListViewItem>()
					.Concat(lvFreezingRule.CheckedItems.Cast<FilterListViewItem>())
					.Select(item => item.Filter);
			return new ExtractingRule(filters);
		}

		private Dictionary<CodeRange, XElement> ExtractRange2Elements(CodeFile file, ExtractingRule rule) {
			var elements = rule.Extract(file.Ast);
			// Elements having the same range can appear so can't use ToDictiornary
			var ret = new Dictionary<CodeRange, XElement>();
			foreach (var element in elements) {
				var range = CodeRange.Locate(element);
				ret[range] = element;
			}
			return ret;
		}

		private void NormalizeAllRange2Element() {
			var name2Count = new Dictionary<string, int>();
			var elements = _modifyingFiles.SelectMany(f => f.Range2Elements.Values)
					.SelectMany(e => e.DescendantsOfOnlyChildAndSelf());
			foreach (var element in elements) {
				int count = 0;
				name2Count.TryGetValue(element.Name.LocalName, out count);
				name2Count[element.Name.LocalName] = count + 1;
			}

			foreach (var codeFile in _modifyingFiles) {
				var newRange2Elements = new Dictionary<CodeRange, XElement>();
				foreach (var nameAndElement in codeFile.Range2Elements) {
					var newElement = nameAndElement.Value.DescendantsOfOnlyChildAndSelf()
							.OrderByDescending(e => name2Count[e.Name.LocalName])
							.First();
					newRange2Elements[nameAndElement.Key] = newElement;
				}
				codeFile.Range2Elements = newRange2Elements;
			}
		}

		private CodeRange GetSelectedRange() {
			int start, end;
			azuki.GetSelection(out start, out end);
			return CodeRange.ConvertFromIndiciesSkippingWhitespaces(Code, ref start, ref end);
		}

		/// <summary>
		/// Draws markers on the editor with the specified dictionary of ranges and elements and the specified marking ids.
		/// </summary>
		/// <param name="range2Elements"></param>
		/// <param name="markingIds"></param>
		private void DrawMarkers(IDictionary<CodeRange, XElement> range2Elements, params int[] markingIds) {
			foreach (var markingId in markingIds) {
				azuki.Document.Unmark(0, azuki.TextLength, markingId);
			}
			foreach (var rangeAndElement in range2Elements) {
				var range = rangeAndElement.Key;
				int inclusiveStart, exclusiveEnd;
				range.ConvertToIndicies(azuki.Text, out inclusiveStart, out exclusiveEnd);
				foreach (var markingId in markingIds) {
					var end = Math.Min(exclusiveEnd, inclusiveStart + MaxLengths[markingId]);
					azuki.Document.Mark(inclusiveStart, end, markingId);
				}
			}
			azuki.Invalidate();
		}

		/// <summary>
		/// Shows the specified dictionary of ranges and elements on the list.
		/// </summary>
		private void ListElements(IDictionary<CodeRange, XElement> range2Elements, ListView listView) {
			listView.Items.Clear();
			foreach (var rangeAndElement in _activeFile.Range2Elements) {
				var item = new ElementListViewItem(rangeAndElement.Key, rangeAndElement.Value);
				listView.Items.Add(item);
			}
		}

		private void RedrawActiveFile() {
			DrawMarkers(_activeFile.Range2Elements, 0, 1, 2);
			ListElements(_activeFile.Range2Elements, lvMarkedElements);
			ListElements(_activeFile.Range2Elements, lvWillBeMarkedElements);
		}

		#endregion

		#region Inner Classes

		public class FileListViewItem : ListViewItem {
			public CodeFile File { get; private set; }

			public FileListViewItem(CodeFile file)
					: base(file.Info.Name, file.Info.FullName) {
				File = file;
			}
		}

		public class FilterListViewItem : ListViewItem {
			public IFilter Filter { get; private set; }

			public FilterListViewItem(IFilter filter)
					: base(filter.ToString()) {
				Filter = filter;
			}
		}

		public class ElementListViewItem : ListViewItem {
			public CodeRange Range { get; private set; }
			public XElement Element { get; private set; }

			public ElementListViewItem(CodeRange range, XElement element)
					: base(new[] { element.Name.LocalName, element.Value }) {
				Range = range;
				Element = element;
				ToolTipText = element.ToString();
			}
		}

		public class Highlighter : IHighlighter {
			public void Highlight(Document doc) {}

			public void Highlight(Document doc, ref int dirtyBegin, ref int dirtyEnd) {
				throw new NotImplementedException();
			}

			public bool CanUseHook { get; private set; }
			public HighlightHook HookProc { get; set; }
		}

		#endregion
	}
}