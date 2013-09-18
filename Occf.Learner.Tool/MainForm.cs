using System;
using System.Collections.Generic;
using System.Drawing;
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
		private IDictionary<CodeRange, XElement> _range2Elements;

		public MainForm() {
			_range2Elements = new Dictionary<CodeRange, XElement>();

			InitializeComponent();
			Marking.Register(new MarkingInfo(0, "selected"));
			Marking.Register(new MarkingInfo(1, "assumed"));
			azuki.ColorScheme.SetMarkingDecoration(0, new BgColorTextDecoration(Color.Yellow));
			azuki.ColorScheme.SetMarkingDecoration(1,
					new UnderlineTextDecoration(LineStyle.Solid, Color.Red));
		}

		private void ApplyRules(IEnumerable<IFilter> ruleDict) {
			var rule = new FilteringRule(ruleDict);
			var elements = rule.Apply(_ast);
			foreach (var element in elements) {
				int inclusiveStart, exclusiveEnd;
				CodeRange.Locate(element).ConvertToIndicies(azuki.Text, out inclusiveStart, out exclusiveEnd);
				azuki.Document.Mark(inclusiveStart, Math.Min(exclusiveEnd, inclusiveStart + 4), 1);
			}
			azuki.Invalidate();
		}

		private void button1_Click(object sender, EventArgs e) {
			ruleListBox.Items.Clear();
			var filters = FilteringRuleLearner.Learn(_ast, _range2Elements.Values).ToList();
			foreach (var filter in filters) {
				ruleListBox.Items.Add(filter, true);
			}
			ApplyRules(filters);
		}

		private void button2_Click(object sender, EventArgs e) {
			ApplyRules(ruleListBox.SelectedItems.Cast<IFilter>());
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
			var code = azuki.Text;
			var range = CodeRange.ConvertFromIndicies(code, ref start, ref end);
			var selectedElement = range.FindOuterElement(_ast);
			Console.WriteLine(selectedElement);
			CodeRange.Locate(selectedElement).ConvertToIndicies(code, out start, out end);
			azuki.Document.Mark(start, end, 0);
			_range2Elements.Add(range, selectedElement);
			azuki.Invalidate();
		}

		private void unmarkToolStripMenuItem_Click(object sender, EventArgs e) {
			int start, end;
			azuki.GetSelection(out start, out end);
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
			var fileName = fileNames[0];
			var text = GuessEncoding.ReadAllText(fileName);
			azuki.Text = text;
			_ast = _activeCodeToXml.Generate(text);
			_range2Elements.Clear();
		}

		private void azuki_Click(object sender, EventArgs e) {}
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