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
				_rtbCode.Select(
						element.Position.StartLine, element.Position.EndLine,
						element.Position.StartPosition, element.Position.EndPosition);
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