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
using System.Windows.Forms;

namespace Occf.Tools.Window {
	public partial class ProgressForm : Form {
		public ProgressForm() {
			InitializeComponent();
		}

		private void ProgressForm_Load(object sender, EventArgs e) {}

		public void Start(IWin32Window owner, int fileCount, Action action) {
			progressBar.Maximum = fileCount;
			Show(owner);

			action.BeginInvoke(
					ar => {
						action.EndInvoke(ar);
						Action hideAction = EndProgress;
						Invoke(hideAction);
					}, null);
		}

		public void Progress(string filePath) {
			Action action = () => {
				progressBar.Value++;
				lbFilePath.Text = filePath;
			};
			Invoke(action);
		}

		public void EndProgress() {
			Hide();
			progressBar.Minimum = 0;
			progressBar.Value = 0;
			lbFilePath.Text = "";
		}

		private void btnCancel_Click(object sender, EventArgs e) {}
	}
}