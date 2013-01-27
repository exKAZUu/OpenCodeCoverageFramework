namespace Occf.Reporter
{
	partial class Editor
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._rtbCode = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// richTextBox1
			// 
			this._rtbCode.Dock = System.Windows.Forms.DockStyle.Fill;
			this._rtbCode.Location = new System.Drawing.Point(0, 0);
			this._rtbCode.Name = "_rtbCode";
			this._rtbCode.Size = new System.Drawing.Size(584, 426);
			this._rtbCode.TabIndex = 1;
			this._rtbCode.Text = "";
			// 
			// Editor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 426);
			this.Controls.Add(this._rtbCode);
			this.Name = "Editor";
			this.Text = "Editor";
			this.Load += new System.EventHandler(this.Editor_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox _rtbCode;

	}
}