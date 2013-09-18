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
			this.components = new System.ComponentModel.Container();
			Sgry.Azuki.FontInfo fontInfo1 = new Sgry.Azuki.FontInfo();
			this.azuki = new Sgry.Azuki.WinForms.AzukiControl();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.markToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.unmarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.button1 = new System.Windows.Forms.Button();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.menuItemLanguage = new System.Windows.Forms.ToolStripMenuItem();
			this.ruleListBox = new System.Windows.Forms.CheckedListBox();
			this.button2 = new System.Windows.Forms.Button();
			this.contextMenuStrip.SuspendLayout();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// azuki
			// 
			this.azuki.AllowDrop = true;
			this.azuki.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(250)))), ((int)(((byte)(240)))));
			this.azuki.ContextMenuStrip = this.contextMenuStrip;
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
			this.azuki.Location = new System.Drawing.Point(13, 48);
			this.azuki.Margin = new System.Windows.Forms.Padding(4);
			this.azuki.Name = "azuki";
			this.azuki.Size = new System.Drawing.Size(437, 436);
			this.azuki.TabIndex = 0;
			this.azuki.Text = "azukiControl1";
			this.azuki.ViewWidth = 4129;
			this.azuki.Click += new System.EventHandler(this.azuki_Click);
			this.azuki.DragDrop += new System.Windows.Forms.DragEventHandler(this.azuki_DragDrop);
			this.azuki.DragEnter += new System.Windows.Forms.DragEventHandler(this.azuki_DragEnter);
			this.azuki.MouseClick += new System.Windows.Forms.MouseEventHandler(this.azuki_MouseClick);
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.markToolStripMenuItem,
            this.unmarkToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(138, 60);
			// 
			// markToolStripMenuItem
			// 
			this.markToolStripMenuItem.Name = "markToolStripMenuItem";
			this.markToolStripMenuItem.Size = new System.Drawing.Size(137, 28);
			this.markToolStripMenuItem.Text = "Mark";
			this.markToolStripMenuItem.Click += new System.EventHandler(this.markToolStripMenuItem_Click);
			// 
			// unmarkToolStripMenuItem
			// 
			this.unmarkToolStripMenuItem.Name = "unmarkToolStripMenuItem";
			this.unmarkToolStripMenuItem.Size = new System.Drawing.Size(137, 28);
			this.unmarkToolStripMenuItem.Text = "Unmark";
			this.unmarkToolStripMenuItem.Click += new System.EventHandler(this.unmarkToolStripMenuItem_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(185, 510);
			this.button1.Margin = new System.Windows.Forms.Padding(4);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(100, 29);
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
			this.menuStrip.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
			this.menuStrip.Size = new System.Drawing.Size(1083, 31);
			this.menuStrip.TabIndex = 2;
			// 
			// menuItemLanguage
			// 
			this.menuItemLanguage.Name = "menuItemLanguage";
			this.menuItemLanguage.Size = new System.Drawing.Size(93, 27);
			this.menuItemLanguage.Text = "Language";
			// 
			// ruleListBox
			// 
			this.ruleListBox.Cursor = System.Windows.Forms.Cursors.Default;
			this.ruleListBox.FormattingEnabled = true;
			this.ruleListBox.HorizontalScrollbar = true;
			this.ruleListBox.Location = new System.Drawing.Point(457, 48);
			this.ruleListBox.Name = "ruleListBox";
			this.ruleListBox.Size = new System.Drawing.Size(614, 225);
			this.ruleListBox.TabIndex = 4;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(438, 510);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 5;
			this.button2.Text = "button2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1083, 609);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.ruleListBox);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.azuki);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "MainForm";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.contextMenuStrip.ResumeLayout(false);
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
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem markToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem unmarkToolStripMenuItem;
		private System.Windows.Forms.CheckedListBox ruleListBox;
		private System.Windows.Forms.Button button2;
	}
}

