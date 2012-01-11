using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Occf.Core.CoverageInformation;
using Paraiba.Windows.Forms;

namespace Occf.Reporter {
	public partial class Editor : Form {
		public Editor(string path, IEnumerable<ICoverageElement> elements) {
			InitializeComponent();

			using (var fs = new FileStream(path, FileMode.Open)) {
				using (var reader = new StreamReader(fs)) {
					_rtbCode.Text = reader.ReadToEnd();
				}
			}
			foreach (var element in elements) {
				_rtbCode.Select(element.Position.StartLine, element.Position.EndLine,
					element.Position.StartPos, element.Position.EndPos);
				_rtbCode.SelectionBackColor = Color.Red;
			}

			_rtbCode.SelectionStart = 0;
			_rtbCode.SelectionLength = 0;
			_rtbCode.ScrollToCaret();
			_rtbCode.Focus();
		}

		private void Editor_Load(object sender, EventArgs e) {}
	}
}