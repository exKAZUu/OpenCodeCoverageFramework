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
			this.lbRule = new System.Windows.Forms.CheckedListBox();
			this.lvSelectedElements = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lvInferedElements = new System.Windows.Forms.ListView();
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.button3 = new System.Windows.Forms.Button();
			this.lvFile = new System.Windows.Forms.ListView();
			this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
			this.azuki.Location = new System.Drawing.Point(242, 34);
			this.azuki.Margin = new System.Windows.Forms.Padding(4);
			this.azuki.Name = "azuki";
			this.azuki.Size = new System.Drawing.Size(437, 653);
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
			this.button1.Location = new System.Drawing.Point(309, 695);
			this.button1.Margin = new System.Windows.Forms.Padding(4);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(100, 29);
			this.button1.TabIndex = 1;
			this.button1.Text = "Infer";
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
			this.menuStrip.Size = new System.Drawing.Size(1312, 31);
			this.menuStrip.TabIndex = 2;
			// 
			// menuItemLanguage
			// 
			this.menuItemLanguage.Name = "menuItemLanguage";
			this.menuItemLanguage.Size = new System.Drawing.Size(93, 27);
			this.menuItemLanguage.Text = "Language";
			// 
			// lbRule
			// 
			this.lbRule.Cursor = System.Windows.Forms.Cursors.Default;
			this.lbRule.FormattingEnabled = true;
			this.lbRule.HorizontalScrollbar = true;
			this.lbRule.Location = new System.Drawing.Point(686, 34);
			this.lbRule.Name = "lbRule";
			this.lbRule.Size = new System.Drawing.Size(614, 225);
			this.lbRule.TabIndex = 4;
			this.lbRule.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lbRule_ItemCheck);
			// 
			// lvSelectedElements
			// 
			this.lvSelectedElements.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.lvSelectedElements.Location = new System.Drawing.Point(686, 262);
			this.lvSelectedElements.Name = "lvSelectedElements";
			this.lvSelectedElements.ShowItemToolTips = true;
			this.lvSelectedElements.Size = new System.Drawing.Size(301, 425);
			this.lvSelectedElements.TabIndex = 8;
			this.lvSelectedElements.UseCompatibleStateImageBehavior = false;
			this.lvSelectedElements.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Name";
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Text";
			this.columnHeader2.Width = 219;
			// 
			// lvInferedElements
			// 
			this.lvInferedElements.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
			this.lvInferedElements.Location = new System.Drawing.Point(993, 262);
			this.lvInferedElements.Name = "lvInferedElements";
			this.lvInferedElements.ShowItemToolTips = true;
			this.lvInferedElements.Size = new System.Drawing.Size(307, 425);
			this.lvInferedElements.TabIndex = 9;
			this.lvInferedElements.UseCompatibleStateImageBehavior = false;
			this.lvInferedElements.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Name";
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Text";
			this.columnHeader4.Width = 219;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(524, 695);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(102, 29);
			this.button3.TabIndex = 10;
			this.button3.Text = "Apply";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// lvFile
			// 
			this.lvFile.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6});
			this.lvFile.HideSelection = false;
			this.lvFile.Location = new System.Drawing.Point(12, 34);
			this.lvFile.MultiSelect = false;
			this.lvFile.Name = "lvFile";
			this.lvFile.ShowItemToolTips = true;
			this.lvFile.Size = new System.Drawing.Size(223, 653);
			this.lvFile.TabIndex = 11;
			this.lvFile.UseCompatibleStateImageBehavior = false;
			this.lvFile.View = System.Windows.Forms.View.Details;
			this.lvFile.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvFile_ItemSelectionChanged);
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Name";
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Path";
			this.columnHeader6.Width = 219;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1312, 751);
			this.Controls.Add(this.lvFile);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.lvInferedElements);
			this.Controls.Add(this.lvSelectedElements);
			this.Controls.Add(this.lbRule);
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
		private System.Windows.Forms.CheckedListBox lbRule;
		private System.Windows.Forms.ListView lvSelectedElements;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ListView lvInferedElements;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.ListView lvFile;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
	}
}

