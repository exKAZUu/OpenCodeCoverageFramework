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
using Paraiba.Text;
using Sgry.Azuki;
using Sgry.Azuki.Highlighter;

namespace Occf.Learner.Tool {
	public partial class MainForm : Form {
		private CodeToXml _activeCodeToXml;
		private XElement _ast;
		private readonly IDictionary<CodeRange, XElement> _range2Elements;
		private ListViewItem _lastSelectedItem;

		public string Code {
			get { return azuki.Text; }
			set { azuki.Text = value; }
		}

		public MainForm() {
			_range2Elements = new Dictionary<CodeRange, XElement>();

			InitializeComponent();
			Marking.Register(new MarkingInfo(0, "selected"));
			Marking.Register(new MarkingInfo(1, "selected_head"));
			Marking.Register(new MarkingInfo(2, "infered_head"));
			azuki.ColorScheme.SetMarkingDecoration(0, new BgColorTextDecoration(Color.Yellow));
			azuki.ColorScheme.SetMarkingDecoration(1,
					new UnderlineTextDecoration(LineStyle.Solid, Color.Red));
			azuki.ColorScheme.SetMarkingDecoration(2,
					new UnderlineTextDecoration(LineStyle.Waved, Color.Blue));
		}

		private List<XElement> ApplyRules(IEnumerable<IFilter> ruleDict) {
			lvInferedElements.Items.Clear();
			azuki.Document.Unmark(0, azuki.TextLength, 2);

			var rule = new FilteringRule(ruleDict);
			var elements = rule.Apply(_ast).ToList();
			foreach (var element in elements) {
				int inclusiveStart, exclusiveEnd;
				CodeRange.Locate(element).ConvertToIndicies(azuki.Text, out inclusiveStart, out exclusiveEnd);
				azuki.Document.Mark(inclusiveStart, Math.Min(exclusiveEnd, inclusiveStart + 4), 2);

				var item = new ListViewItem(new[] { element.Name.LocalName, element.Value });
				item.ToolTipText = element.ToString();
				lvInferedElements.Items.Add(item);
			}
			azuki.Invalidate();
			return elements;
		}

		private void button1_Click(object sender, EventArgs e) {
			lbRule.Items.Clear();
			var filters = RuleLearner.Learn(_ast, _range2Elements.Values).ToList();
			foreach (var filter in filters) {
				lbRule.Items.Add(filter, true);
			}
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
				};
				if (codeToXml is JavaCodeToXml) {
					menuItem.PerformClick();
				}
			}
		}

		private void azuki_MouseClick(object sender, MouseEventArgs e) {
			var me = e as IMouseEventArgs;

			int inclusiveStart, exclusiveEnd;
			azuki.GetSelection(out inclusiveStart, out exclusiveEnd);
			Console.WriteLine(inclusiveStart + ", " + exclusiveEnd);
		}

		private void markToolStripMenuItem_Click(object sender, EventArgs e) {
			int start, end;
			azuki.GetSelection(out start, out end);
			var range = CodeRange.ConvertFromIndicies(Code, ref start, ref end);
			var element = range.FindOuterElement(_ast);
			Console.WriteLine(element);
			MarkSelectedElement(element);
			azuki.Invalidate();
		}

		private void unmarkToolStripMenuItem_Click(object sender, EventArgs e) {
			int start, end;
			azuki.GetSelection(out start, out end);
			var range = _range2Elements.Keys.FirstOrDefault(r => r.Contains(new CodeLocation(start, end)));
			if (range == default(CodeRange)) {
				return;
			}
			range.ConvertToIndicies(Code, out start, out end);
			azuki.Document.Unmark(start, end, 0);
			azuki.Document.Unmark(start, Math.Min(end, start + 4), 1);
			_range2Elements.Remove(range);
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
			}
			lvFile.Items[count].Selected = true;
		}

		private void OpenCode(string path) {
			Code = GuessEncoding.ReadAllText(path);
			_ast = _activeCodeToXml.Generate(Code);

			_range2Elements.Clear();
			lvSelectedElements.Items.Clear();
			lvInferedElements.Items.Clear();
		}

		private void azuki_Click(object sender, EventArgs e) {}

		private void button3_Click(object sender, EventArgs e) {
			ApplyAndMark();
		}

		private void ApplyAndMark() {
			var elements = ApplyRules(lbRule.CheckedItems.Cast<IFilter>());
			foreach (var element in elements) {
				MarkSelectedElement(element);
			}
			azuki.Invalidate();
		}

		private void MarkSelectedElement(XElement element) {
			int start, end;
			var range = CodeRange.Locate(element);
			if (_range2Elements.ContainsKey(range)) {
				return;
			}
			range.ConvertToIndicies(Code, out start, out end);
			azuki.Document.Mark(start, end, 0);
			azuki.Document.Mark(start, Math.Min(end, start + 4), 1);
			_range2Elements.Add(range, element);
			var item = new ListViewItem(new[] { element.Name.LocalName, element.Value });
			item.ToolTipText = element.ToString();
			lvSelectedElements.Items.Add(item);
		}

		private void lbRule_ItemCheck(object sender, ItemCheckEventArgs e) {
			ApplyRules(lbRule.CheckedItems.Cast<IFilter>());
		}

		private void lvFile_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
			if (e.Item != null && e.Item != _lastSelectedItem) {
				OpenCode(e.Item.SubItems[1].Text);
				ApplyAndMark();
				_lastSelectedItem = e.Item;
			}
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
}