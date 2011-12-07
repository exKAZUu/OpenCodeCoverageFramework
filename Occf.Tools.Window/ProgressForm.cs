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

			action.BeginInvoke(ar => {
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