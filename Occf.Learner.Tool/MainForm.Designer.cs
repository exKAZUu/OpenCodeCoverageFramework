namespace Occf.Learner.Tool
{
	partial class MainForm
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
			if (disposing && (components != null))
			{
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
			Sgry.Azuki.FontInfo fontInfo1 = new Sgry.Azuki.FontInfo();
			this.azuki = new Sgry.Azuki.WinForms.AzukiControl();
			this.button1 = new System.Windows.Forms.Button();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.menuItemLanguage = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// azuki
			// 
			this.azuki.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(250)))), ((int)(((byte)(240)))));
			this.azuki.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.azuki.DrawingOption = ((Sgry.Azuki.DrawingOption)(((((((Sgry.Azuki.DrawingOption.DrawsFullWidthSpace | Sgry.Azuki.DrawingOption.DrawsTab) 
            | Sgry.Azuki.DrawingOption.DrawsEol) 
            | Sgry.Azuki.DrawingOption.HighlightCurrentLine) 
            | Sgry.Azuki.DrawingOption.ShowsLineNumber) 
            | Sgry.Azuki.DrawingOption.ShowsDirtBar) 
            | Sgry.Azuki.DrawingOption.HighlightsMatchedBracket)));
			this.azuki.FirstVisibleLine = 0;
			this.azuki.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			fontInfo1.Name = "MS UI Gothic";
			fontInfo1.Size = 9;
			fontInfo1.Style = System.Drawing.FontStyle.Regular;
			this.azuki.FontInfo = fontInfo1;
			this.azuki.ForeColor = System.Drawing.Color.Black;
			this.azuki.IsReadOnly = true;
			this.azuki.Location = new System.Drawing.Point(224, 42);
			this.azuki.Name = "azuki";
			this.azuki.Size = new System.Drawing.Size(560, 349);
			this.azuki.TabIndex = 0;
			this.azuki.Text = "azukiControl1";
			this.azuki.ViewWidth = 4129;
			this.azuki.MouseClick += new System.Windows.Forms.MouseEventHandler(this.azuki_MouseClick);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(139, 408);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 1;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemLanguage});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(812, 26);
			this.menuStrip.TabIndex = 2;
			// 
			// menuItemLanguage
			// 
			this.menuItemLanguage.Name = "menuItemLanguage";
			this.menuItemLanguage.Size = new System.Drawing.Size(76, 22);
			this.menuItemLanguage.Text = "Language";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(812, 487);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.azuki);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Name = "MainForm";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Sgry.Azuki.WinForms.AzukiControl azuki;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem menuItemLanguage;
	}
}

