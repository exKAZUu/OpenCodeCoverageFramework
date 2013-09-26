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
using Code2Xml.Languages.Java.CodeToXmls;
using Occf.Learner.Core;
using Sgry.Azuki;
using Sgry.Azuki.Highlighter;

namespace Occf.Learner.Tool {
	public partial class MainForm : Form {
		private static readonly int[] MaxLengths = { int.MaxValue / 2, 4, 8 };
		private CodeToXml _activeCodeToXml;
		private CodeFile _activeFile;
		private ListViewItem _lastSelectedItem;
		private bool _eventEnabled;

		public string Code {
			get { return azuki.Text; }
			set { azuki.Text = value; }
		}

		private IEnumerable<CodeFile> Files {
			get {
				return lvModifiableFile.Items.Cast<FileListViewItem>()
						.Concat(lvFreezedFile.Items.Cast<FileListViewItem>())
						.Select(item => item.File);
			}
		}

		private IEnumerable<CodeFile> FreezingFiles {
			get {
				return lvFreezedFile.Items.Cast<FileListViewItem>()
						.Select(item => item.File);
			}
		}

		public MainForm() {
			_eventEnabled = true;

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
					lvModifiableFile.Items.Clear();
					lvFreezedFile.Items.Clear();
					lvModifiableRule.Items.Clear();
					lvFreezedRule.Items.Clear();
					lvWillBeMarkedElements.Items.Clear();
					lvMarkedElements.Items.Clear();
				};
				if (codeToXml is JavaCodeToXml) {
					menuItem.PerformClick();
				}
			}
		}

		private void btnInfer_Click(object sender, EventArgs e) {
			NormalizeAllRange2Element(); // To infer good rules
			var learningRecrods = Files
					.Select(f => new LearningRecord(f.Ast, f.Range2Elements.Values))
					.ToList();
			var filters = RuleLearner.Learn(learningRecrods).ToList();
			_eventEnabled = false;
			var asts = lvModifiableFile.Items.Cast<FileListViewItem>()
					.Concat(lvFreezedFile.Items.Cast<FileListViewItem>())
					.Select(item => item.File.Ast)
					.ToList();
			var newModifiableItems = new List<RuleListViewItem>();
			foreach (var filter in filters) {
				var modifiableRule = lvModifiableRule.Items.Cast<RuleListViewItem>()
						.FirstOrDefault(i => i.Filter.TryUpdateProperties(filter));
				var freezedRule = lvFreezedRule.Items.Cast<RuleListViewItem>()
						.FirstOrDefault(i => i.Filter.TryUpdateProperties(filter));
				if (modifiableRule == null && freezedRule == null) {
					var item = new RuleListViewItem(filter, Files);
					if (filter is NopFilter) {
						item.Checked = true; //TODO: should use "filter is NopFilter;" ?
					}
					newModifiableItems.Add(item);
				} else {
					if (modifiableRule != null) {
						modifiableRule.Update(Files);
					newModifiableItems.Add(modifiableRule);
					}
					if (freezedRule != null) {
						freezedRule.Update(Files);
					}
				}
			}
			lvModifiableRule.Items.Clear();
			foreach (var item in newModifiableItems) {
				lvModifiableRule.Items.Add(item);
			}
			_eventEnabled = true;
			lvRule_ItemChecked(null, null);
		}

		private void btnApply_Click(object sender, EventArgs e) {
			var rule = InferRule();
			_activeFile.Range2Elements = rule.ExtractRange2Elements(_activeFile.Ast);
			NormalizeAllRange2Element(); // To show good element lists
			RedrawActiveFile();
		}

		private void btnApplyAll_Click(object sender, EventArgs e) {
			var rule = InferRule();
			foreach (var file in lvModifiableFile.Items.Cast<FileListViewItem>().Select(i => i.File)) {
				file.Range2Elements = rule.ExtractRange2Elements(file.Ast);
			}
			NormalizeAllRange2Element(); // To show good element lists
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
				NormalizeAllRange2Element(); // To show good element lists
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
			var paths = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			var count = lvModifiableFile.Items.Count;
			var filePaths = paths.SelectMany(path =>
					Directory.Exists(path)
							? Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
									.Where(filePath => _activeCodeToXml.TargetExtensions.Contains(Path.GetExtension(filePath)))
							: Enumerable.Repeat(path, 1));
			foreach (var filePath in filePaths) {
				var file = new FileInfo(filePath);
				var codeFile = new CodeFile(_activeCodeToXml, file);
				var item = new FileListViewItem(codeFile);
				lvModifiableFile.Items.Add(item);
			}
			lvModifiableFile.Items[count].Selected = true;
		}

		private void lvFile_ItemSelectionChanged(
				object sender, ListViewItemSelectionChangedEventArgs e) {
			var item = e.Item as FileListViewItem;
			if (item != null && item != _lastSelectedItem) {
				_lastSelectedItem = item;
				ChangeActiveFile(item.File);
			}
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

		private void lvFile_MouseClick(object sender, MouseEventArgs e) {
			var src = (ListView)sender;
			var dst = src == lvModifiableFile ? lvFreezedFile : lvModifiableFile;
			var readOnly = src == lvModifiableFile;
			if (e.Button == MouseButtons.Right) {
				foreach (FileListViewItem item in src.SelectedItems) {
					item.Remove();
					dst.Items.Add(item);
					item.File.ReadOnly = readOnly;
					azuki.ContextMenuStrip = _activeFile.ReadOnly ? null : contextMenuStrip;
				}
				btnApplyAll.Enabled = CanApplyAll();
			}
		}

		private void lvRule_ItemChecked(object sender, ItemCheckedEventArgs e) {
			if (!_eventEnabled) {
				return;
			}
			var rule = InferRule();
			var range2Elements = rule.ExtractRange2Elements(_activeFile.Ast);
			btnApply.Enabled = CanApplyThis(range2Elements);
			btnApplyAll.Enabled = CanApplyAll(rule);
			DrawMarkers(range2Elements, 2);
			ListElements(range2Elements, lvWillBeMarkedElements);
		}

		private void lvRule_MouseClick(object sender, MouseEventArgs e) {
			var src = (ListView)sender;
			var dst = src == lvModifiableRule ? lvFreezedRule : lvModifiableRule;
			if (e.Button == MouseButtons.Right) {
				foreach (RuleListViewItem item in src.SelectedItems) {
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

		#region Helper Methods

		private ExtractingRule InferRule() {
			var filters = lvModifiableRule.CheckedItems.Cast<RuleListViewItem>()
					.Concat(lvFreezedRule.CheckedItems.Cast<RuleListViewItem>())
					.Select(item => item.Filter);
			return new ExtractingRule(filters);
		}

		private void NormalizeAllRange2Element() {
			var name2Count = new Dictionary<string, int>();
			var elements = Files
					.SelectMany(file => file.Range2Elements.Values)
					.SelectMany(e => e.DescendantsOfOnlyChildAndSelf());
			foreach (var element in elements) {
				int count = 0;
				name2Count.TryGetValue(element.Name.LocalName, out count);
				name2Count[element.Name.LocalName] = count + 1;
			}

			foreach (var codeFile in Files) {
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
		private void DrawMarkers(
				IEnumerable<KeyValuePair<CodeRange, XElement>> range2Elements, params int[] markingIds) {
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
		private void ListElements(
				IEnumerable<KeyValuePair<CodeRange, XElement>> range2Elements, ListView listView) {
			listView.Items.Clear();
			foreach (var rangeAndElement in range2Elements) {
				var item = new ElementListViewItem(rangeAndElement.Key, rangeAndElement.Value);
				listView.Items.Add(item);
			}
		}

		private void RedrawActiveFile() {
			DrawMarkers(_activeFile.Range2Elements, 0, 1, 2);
			ListElements(_activeFile.Range2Elements, lvMarkedElements);
			ListElements(_activeFile.Range2Elements, lvWillBeMarkedElements);
		}

		private void ChangeActiveFile(CodeFile activeFile) {
			_activeFile = activeFile;
			Code = _activeFile.Code;
			RedrawActiveFile();
			var rule = InferRule();
			var range2Elements = rule.ExtractRange2Elements(_activeFile.Ast);
			DrawMarkers(range2Elements, 2);
			ListElements(range2Elements, lvWillBeMarkedElements);
			btnApply.Enabled = CanApplyThis(range2Elements);
			azuki.ContextMenuStrip = _activeFile.ReadOnly ? null : contextMenuStrip;
		}

		private bool CanApplyThis(Dictionary<CodeRange, XElement> range2Elements) {
			var equal = _activeFile.RangesEquals(range2Elements);
			return !_activeFile.ReadOnly || equal;
		}

		private bool CanApplyAll(ExtractingRule rule = null) {
			rule = rule ?? InferRule();
			return btnApply.Enabled && FreezingFiles.All(
					f => f.RangesEquals(rule.ExtractRange2Elements(f.Ast)));
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

		public class RuleListViewItem : ListViewItem {
			public IFilter Filter { get; private set; }

			public RuleListViewItem(IFilter filter, IEnumerable<CodeFile> files)
					: base(GetName(filter, files)) {
				Filter = filter;
			}

			private static string GetName(IFilter filter, IEnumerable<CodeFile> files) {
				var rule = new ExtractingRule(Enumerable.Repeat(filter, 1));
				var exactMatch = files.Where(f => f.ReadOnly).All(
						f => f.RangesEquals(filter.ElementName, rule.ExtractRange2Elements(f.Ast)));
				return (exactMatch ? "* " : "") + files.Select(f => f.Ast).Select(filter.CountRemovableTargets).Sum() + ": " + filter;
			}

			public void Update(IEnumerable<CodeFile> files) {
				Text = GetName(Filter, files);
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