using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.Plugin;
using Code2Xml.Languages.Java.CodeToXmls;
using Sgry.Azuki;
using Sgry.Azuki.Highlighter;

namespace Occf.Learner.Tool {
	public partial class MainForm : Form {
		private CodeToXml _activeCodeToXml;

		public MainForm() {
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e) {
			int start, end;
			azuki.GetSelection(out start, out end);
			azuki.Document.SetCharClass(0, CharClass.Heading1);
			azuki.Document.SetCharClass(1, CharClass.Heading1);
			var marking = new MarkingInfo(0, "default");
			Marking.Register(marking);
			azuki.ColorScheme.SetMarkingDecoration(0, new BgColorTextDecoration(Color.Yellow));
			azuki.Document.Mark(0, 5, 0);
			azuki.Invalidate();
			Console.WriteLine(azuki.Document.GetMarkedText(7, 0));
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
			int start, end;
			azuki.GetSelection(out start, out end);
			Console.WriteLine(start + ", " + end);
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