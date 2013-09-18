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
		private CodeToXml _activeCodeToXml;
		private List<CodeFile> _codeFiles;
		private CodeFile _activeCodeFile;
		private ListViewItem _lastSelectedItem;

		public string Code {
			get { return azuki.Text; }
			set { azuki.Text = value; }
		}

		public MainForm() {
			_codeFiles = new List<CodeFile>();

			InitializeComponent();
			Marking.Register(new MarkingInfo(0, "selected"));
			Marking.Register(new MarkingInfo(1, "selected_head"));
			Marking.Register(new MarkingInfo(2, "infered_head"));
			azuki.ColorScheme.SetMarkingDecoration(0, new BgColorTextDecoration(Color.Yellow));
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
					lvFile.Items.Clear();
					lvRule.Items.Clear();
					lvInferedElements.Items.Clear();
					lvSelectedElements.Items.Clear();
				};
				if (codeToXml is JavaScriptCodeToXml) {
					menuItem.PerformClick();
				}
			}
		}

		private void btnInfer_Click(object sender, EventArgs e) {
			lvRule.Items.Clear();

			var datas = _codeFiles.Select(f => new LearningData(f.Ast, f.Range2Elements.Values)).ToList();
			var filters = RuleLearner.Learn(datas).ToList();
			foreach (var filter in filters) {
				var item = new FilterListViewItem(filter);
				lvRule.Items.Add(item);
				item.Checked = filter is NopFilter;
			}
		}

		private void btnApply_Click(object sender, EventArgs e) {
			var elements = ApplyRules();
			foreach (var element in elements) {
				MarkSelectedElement(element, false);
			}
			NormalizeAllRange2Elements();
			azuki.Invalidate();
		}

		private void btnApplyAll_Click(object sender, EventArgs e) {
			btnApply.PerformClick();
			var rules = lvRule.CheckedItems.Cast<FilterListViewItem>().Select(i => i.Filter);
			var rule = new FilteringRule(rules);
			foreach (var codeFile in _codeFiles) {
				var elements = rule.Apply(codeFile.Ast).ToList();
				foreach (var element in elements) {
					var range = CodeRange.Locate(element);
					if (!codeFile.Range2Elements.ContainsKey(range)) {
						codeFile.Range2Elements.Add(range, element);
					}
				}
			}
		}

		private void azuki_Click(object sender, EventArgs e) {}

		private void azuki_MouseClick(object sender, MouseEventArgs e) {}

		private void markToolStripMenuItem_Click(object sender, EventArgs e) {
			int start, end;
			azuki.GetSelection(out start, out end);
			var range = CodeRange.ConvertFromIndicies(Code, ref start, ref end);
			// TODO: Should be use FindInnerElement or FindOuterElement ?
			var element = range.FindOuterElement(_activeCodeFile.Ast);
			Console.WriteLine(element);
			MarkSelectedElement(element, false);
			NormalizeAllRange2Elements();
			azuki.Invalidate();
		}

		private void unmarkToolStripMenuItem_Click(object sender, EventArgs e) {
			int start, end;
			azuki.GetSelection(out start, out end);
			var selectedRange = CodeRange.ConvertFromIndiciesSkippingWhitespaces(Code, ref start, ref end);
			var ranges = _activeCodeFile.Range2Elements.Keys.Where(r => r.Contains(selectedRange));
			var range =
					ranges.OrderBy(r => (r.EndLine - r.StartLine) * 65536 + (r.EndPosition - r.StartPosition))
							.First();
			if (range == default(CodeRange)) {
				return;
			}
			_activeCodeFile.Range2Elements.Remove(range);
			ReflectRange2Elements();
		}

		private void ReflectRange2Elements() {
			lvSelectedElements.Items.Clear();
			azuki.Document.Unmark(0, azuki.TextLength, 0);
			azuki.Document.Unmark(0, azuki.TextLength, 1);
			foreach (var element in _activeCodeFile.Range2Elements.Values) {
				MarkSelectedElement(element, true);
			}
			azuki.Invalidate();
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
			var count = lvFile.Items.Count;
			foreach (var fileName in fileNames) {
				var file = new FileInfo(fileName);
				var item = new ListViewItem(new[] { file.Name, file.FullName });
				lvFile.Items.Add(item);
				_codeFiles.Add(new CodeFile(_activeCodeToXml, file.FullName));
			}
			lvFile.Items[count].Selected = true;
		}

		private void lvFile_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
			if (e.Item != null && e.Item != _lastSelectedItem) {
				_lastSelectedItem = e.Item;
				lvSelectedElements.Items.Clear();

				_activeCodeFile = _codeFiles[e.Item.Index];
				Code = _activeCodeFile.Code;
				foreach (var element in _activeCodeFile.Range2Elements.Values) {
					MarkSelectedElement(element, true);
				}
				ApplyRules();
			}
		}

		private void lvRule_ItemChecked(object sender, ItemCheckedEventArgs e) {
			ApplyRules();
		}

		private List<XElement> ApplyRules() {
			lvInferedElements.Items.Clear();
			azuki.Document.Unmark(0, azuki.TextLength, 2);

			var rules = lvRule.CheckedItems.Cast<FilterListViewItem>().Select(i => i.Filter);
			var rule = new FilteringRule(rules);
			var elements = rule.Apply(_activeCodeFile.Ast).ToList();
			foreach (var element in elements) {
				int inclusiveStart, exclusiveEnd;
				var range = CodeRange.Locate(element);
				range.ConvertToIndicies(azuki.Text, out inclusiveStart, out exclusiveEnd);
				azuki.Document.Mark(inclusiveStart, Math.Min(exclusiveEnd, inclusiveStart + 4), 2);
				lvInferedElements.Items.Add(new ElementListViewItem(range, element));
			}
			azuki.Invalidate();
			return elements;
		}

		private void MarkSelectedElement(XElement element, bool markingOnly) {
			int start, end;
			var range = CodeRange.Locate(element);
			if (!markingOnly && _activeCodeFile.Range2Elements.ContainsKey(range)) {
				return;
			}
			range.ConvertToIndicies(Code, out start, out end);
			azuki.Document.Mark(start, end, 0);
			azuki.Document.Mark(start, Math.Min(end, start + 4), 1);
			if (!markingOnly) {
				_activeCodeFile.Range2Elements.Add(range, element);
			}
			lvSelectedElements.Items.Add(new ElementListViewItem(range, element));
		}

		private void NormalizeAllRange2Elements() {
			var name2Count = new Dictionary<string, int>();
			var elements = _codeFiles.SelectMany(f => f.Range2Elements.Values)
					.SelectMany(e => e.DescendantsOfOnlyChildAndSelf());
			foreach (var element in elements) {
				int count = 0;
				name2Count.TryGetValue(element.Name.LocalName, out count);
				name2Count[element.Name.LocalName] = count + 1;
			}

			foreach (var codeFile in _codeFiles) {
				var newRange2Elements = new Dictionary<CodeRange, XElement>();
				foreach (var nameAndElement in codeFile.Range2Elements) {
					var newElement = nameAndElement.Value.DescendantsOfOnlyChildAndSelf()
							.OrderByDescending(e => name2Count[e.Name.LocalName])
							.First();
					newRange2Elements[nameAndElement.Key] = newElement;
				}
				codeFile.Range2Elements = newRange2Elements;
			}

			lvSelectedElements.Items.Clear();
			foreach (var rangeAndElement in _activeCodeFile.Range2Elements) {
				lvSelectedElements.Items.Add(new ElementListViewItem(rangeAndElement.Key, rangeAndElement.Value));
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

		private void lvSelectedElements_MouseDoubleClick(object sender, MouseEventArgs e) {
			var item = lvSelectedElements.SelectedItems.Cast<ElementListViewItem>().FirstOrDefault();
			if (item != null) {
				var indicies = item.Range.ConvertToIndicies(Code);
				azuki.SetSelection(indicies.Item1, indicies.Item2);
			}
		}

		private void lvInferedElements_MouseDoubleClick(object sender, MouseEventArgs e) {
			var item = lvInferedElements.SelectedItems.Cast<ElementListViewItem>().FirstOrDefault();
			if (item != null) {
				var indicies = item.Range.ConvertToIndicies(Code);
				azuki.SetSelection(indicies.Item1, indicies.Item2);
			}
		}
	}
}