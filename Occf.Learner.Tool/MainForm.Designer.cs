﻿namespace Occf.Learner.Tool
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
			this.btnInfer = new System.Windows.Forms.Button();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.menuItemLanguage = new System.Windows.Forms.ToolStripMenuItem();
			this.lvMarkedElements = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lvWillBeMarkedElements = new System.Windows.Forms.ListView();
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.btnApply = new System.Windows.Forms.Button();
			this.lvModifiableFile = new System.Windows.Forms.ListView();
			this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.splitContainer5 = new System.Windows.Forms.SplitContainer();
			this.lvModifiableRule = new System.Windows.Forms.ListView();
			this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lvFreezedRule = new System.Windows.Forms.ListView();
			this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.btnApplyAll = new System.Windows.Forms.Button();
			this.splitContainer4 = new System.Windows.Forms.SplitContainer();
			this.lvFreezedFile = new System.Windows.Forms.ListView();
			this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.contextMenuStrip.SuspendLayout();
			this.menuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
			this.splitContainer5.Panel1.SuspendLayout();
			this.splitContainer5.Panel2.SuspendLayout();
			this.splitContainer5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
			this.splitContainer4.Panel1.SuspendLayout();
			this.splitContainer4.Panel2.SuspendLayout();
			this.splitContainer4.SuspendLayout();
			this.SuspendLayout();
			// 
			// azuki
			// 
			this.azuki.AllowDrop = true;
			this.azuki.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(250)))), ((int)(((byte)(240)))));
			this.azuki.ContextMenuStrip = this.contextMenuStrip;
			this.azuki.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.azuki.Dock = System.Windows.Forms.DockStyle.Fill;
			this.azuki.DrawingOption = ((Sgry.Azuki.DrawingOption)(((((((Sgry.Azuki.DrawingOption.DrawsFullWidthSpace | Sgry.Azuki.DrawingOption.DrawsTab) 
            | Sgry.Azuki.DrawingOption.DrawsEol) 
            | Sgry.Azuki.DrawingOption.HighlightCurrentLine) 
            | Sgry.Azuki.DrawingOption.ShowsLineNumber) 
            | Sgry.Azuki.DrawingOption.ShowsDirtBar) 
            | Sgry.Azuki.DrawingOption.HighlightsMatchedBracket)));
			this.azuki.FirstVisibleLine = 0;
			this.azuki.Font = new System.Drawing.Font("MS UI Gothic", 12F);
			fontInfo1.Name = "MS UI Gothic";
			fontInfo1.Size = 12;
			fontInfo1.Style = System.Drawing.FontStyle.Regular;
			this.azuki.FontInfo = fontInfo1;
			this.azuki.ForeColor = System.Drawing.Color.Black;
			this.azuki.IsReadOnly = true;
			this.azuki.Location = new System.Drawing.Point(0, 0);
			this.azuki.Margin = new System.Windows.Forms.Padding(4);
			this.azuki.Name = "azuki";
			this.azuki.Size = new System.Drawing.Size(677, 776);
			this.azuki.TabIndex = 0;
			this.azuki.Text = "azukiControl1";
			this.azuki.ViewWidth = 4138;
			this.azuki.DragDrop += new System.Windows.Forms.DragEventHandler(this.azuki_DragDrop);
			this.azuki.DragEnter += new System.Windows.Forms.DragEventHandler(this.azuki_DragEnter);
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
			// btnInfer
			// 
			this.btnInfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnInfer.Location = new System.Drawing.Point(12, 781);
			this.btnInfer.Margin = new System.Windows.Forms.Padding(4);
			this.btnInfer.Name = "btnInfer";
			this.btnInfer.Size = new System.Drawing.Size(97, 29);
			this.btnInfer.TabIndex = 1;
			this.btnInfer.Text = "Infer Rule";
			this.btnInfer.UseVisualStyleBackColor = true;
			this.btnInfer.Click += new System.EventHandler(this.btnInfer_Click);
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemLanguage});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
			this.menuStrip.Size = new System.Drawing.Size(1456, 31);
			this.menuStrip.TabIndex = 2;
			// 
			// menuItemLanguage
			// 
			this.menuItemLanguage.Name = "menuItemLanguage";
			this.menuItemLanguage.Size = new System.Drawing.Size(93, 27);
			this.menuItemLanguage.Text = "Language";
			// 
			// lvMarkedElements
			// 
			this.lvMarkedElements.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.lvMarkedElements.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvMarkedElements.Location = new System.Drawing.Point(0, 0);
			this.lvMarkedElements.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.lvMarkedElements.Name = "lvMarkedElements";
			this.lvMarkedElements.ShowItemToolTips = true;
			this.lvMarkedElements.Size = new System.Drawing.Size(218, 265);
			this.lvMarkedElements.TabIndex = 8;
			this.lvMarkedElements.UseCompatibleStateImageBehavior = false;
			this.lvMarkedElements.View = System.Windows.Forms.View.Details;
			this.lvMarkedElements.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvElements_MouseDoubleClick);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Name";
			this.columnHeader1.Width = 77;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Text";
			this.columnHeader2.Width = 219;
			// 
			// lvWillBeMarkedElements
			// 
			this.lvWillBeMarkedElements.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
			this.lvWillBeMarkedElements.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvWillBeMarkedElements.ForeColor = System.Drawing.SystemColors.WindowText;
			this.lvWillBeMarkedElements.Location = new System.Drawing.Point(0, 0);
			this.lvWillBeMarkedElements.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.lvWillBeMarkedElements.Name = "lvWillBeMarkedElements";
			this.lvWillBeMarkedElements.ShowItemToolTips = true;
			this.lvWillBeMarkedElements.Size = new System.Drawing.Size(219, 265);
			this.lvWillBeMarkedElements.TabIndex = 9;
			this.lvWillBeMarkedElements.UseCompatibleStateImageBehavior = false;
			this.lvWillBeMarkedElements.View = System.Windows.Forms.View.Details;
			this.lvWillBeMarkedElements.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvElements_MouseDoubleClick);
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
			// btnApply
			// 
			this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnApply.Location = new System.Drawing.Point(116, 781);
			this.btnApply.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(90, 29);
			this.btnApply.TabIndex = 10;
			this.btnApply.Text = "Apply This";
			this.btnApply.UseVisualStyleBackColor = true;
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			// 
			// lvModifiableFile
			// 
			this.lvModifiableFile.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6});
			this.lvModifiableFile.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvModifiableFile.HideSelection = false;
			this.lvModifiableFile.Location = new System.Drawing.Point(0, 0);
			this.lvModifiableFile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.lvModifiableFile.MultiSelect = false;
			this.lvModifiableFile.Name = "lvModifiableFile";
			this.lvModifiableFile.ShowItemToolTips = true;
			this.lvModifiableFile.Size = new System.Drawing.Size(297, 370);
			this.lvModifiableFile.TabIndex = 11;
			this.lvModifiableFile.UseCompatibleStateImageBehavior = false;
			this.lvModifiableFile.View = System.Windows.Forms.View.Details;
			this.lvModifiableFile.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvFile_ItemSelectionChanged);
			this.lvModifiableFile.Click += new System.EventHandler(this.lvFile_Click);
			this.lvModifiableFile.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvFile_MouseClick);
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Name";
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Path";
			this.columnHeader6.Width = 159;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(316, 34);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.azuki);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(1124, 776);
			this.splitContainer1.SplitterDistance = 677;
			this.splitContainer1.SplitterWidth = 5;
			this.splitContainer1.TabIndex = 12;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Margin = new System.Windows.Forms.Padding(4);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.splitContainer5);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
			this.splitContainer2.Size = new System.Drawing.Size(442, 776);
			this.splitContainer2.SplitterDistance = 506;
			this.splitContainer2.SplitterWidth = 5;
			this.splitContainer2.TabIndex = 0;
			// 
			// splitContainer5
			// 
			this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer5.Location = new System.Drawing.Point(0, 0);
			this.splitContainer5.Name = "splitContainer5";
			this.splitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer5.Panel1
			// 
			this.splitContainer5.Panel1.Controls.Add(this.lvModifiableRule);
			// 
			// splitContainer5.Panel2
			// 
			this.splitContainer5.Panel2.Controls.Add(this.lvFreezedRule);
			this.splitContainer5.Size = new System.Drawing.Size(442, 506);
			this.splitContainer5.SplitterDistance = 351;
			this.splitContainer5.TabIndex = 1;
			// 
			// lvModifiableRule
			// 
			this.lvModifiableRule.CheckBoxes = true;
			this.lvModifiableRule.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7});
			this.lvModifiableRule.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvModifiableRule.Location = new System.Drawing.Point(0, 0);
			this.lvModifiableRule.Margin = new System.Windows.Forms.Padding(4);
			this.lvModifiableRule.Name = "lvModifiableRule";
			this.lvModifiableRule.Size = new System.Drawing.Size(442, 351);
			this.lvModifiableRule.TabIndex = 0;
			this.lvModifiableRule.UseCompatibleStateImageBehavior = false;
			this.lvModifiableRule.View = System.Windows.Forms.View.Details;
			this.lvModifiableRule.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvRule_ItemChecked);
			this.lvModifiableRule.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvRule_MouseClick);
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "Filter";
			this.columnHeader7.Width = 326;
			// 
			// lvFreezedRule
			// 
			this.lvFreezedRule.CheckBoxes = true;
			this.lvFreezedRule.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10});
			this.lvFreezedRule.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvFreezedRule.Location = new System.Drawing.Point(0, 0);
			this.lvFreezedRule.Margin = new System.Windows.Forms.Padding(4);
			this.lvFreezedRule.Name = "lvFreezedRule";
			this.lvFreezedRule.Size = new System.Drawing.Size(442, 151);
			this.lvFreezedRule.TabIndex = 1;
			this.lvFreezedRule.UseCompatibleStateImageBehavior = false;
			this.lvFreezedRule.View = System.Windows.Forms.View.Details;
			this.lvFreezedRule.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvRule_ItemChecked);
			this.lvFreezedRule.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvRule_MouseClick);
			// 
			// columnHeader10
			// 
			this.columnHeader10.Text = "Filter";
			this.columnHeader10.Width = 326;
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Margin = new System.Windows.Forms.Padding(4);
			this.splitContainer3.Name = "splitContainer3";
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.lvMarkedElements);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.lvWillBeMarkedElements);
			this.splitContainer3.Size = new System.Drawing.Size(442, 265);
			this.splitContainer3.SplitterDistance = 218;
			this.splitContainer3.SplitterWidth = 5;
			this.splitContainer3.TabIndex = 0;
			// 
			// btnApplyAll
			// 
			this.btnApplyAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnApplyAll.Location = new System.Drawing.Point(212, 781);
			this.btnApplyAll.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.btnApplyAll.Name = "btnApplyAll";
			this.btnApplyAll.Size = new System.Drawing.Size(97, 29);
			this.btnApplyAll.TabIndex = 13;
			this.btnApplyAll.Text = "Apply All";
			this.btnApplyAll.UseVisualStyleBackColor = true;
			this.btnApplyAll.Click += new System.EventHandler(this.btnApplyAll_Click);
			// 
			// splitContainer4
			// 
			this.splitContainer4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.splitContainer4.Location = new System.Drawing.Point(12, 34);
			this.splitContainer4.Name = "splitContainer4";
			this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer4.Panel1
			// 
			this.splitContainer4.Panel1.Controls.Add(this.lvModifiableFile);
			// 
			// splitContainer4.Panel2
			// 
			this.splitContainer4.Panel2.Controls.Add(this.lvFreezedFile);
			this.splitContainer4.Size = new System.Drawing.Size(297, 740);
			this.splitContainer4.SplitterDistance = 370;
			this.splitContainer4.TabIndex = 14;
			// 
			// lvFreezedFile
			// 
			this.lvFreezedFile.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader8,
            this.columnHeader9});
			this.lvFreezedFile.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvFreezedFile.HideSelection = false;
			this.lvFreezedFile.Location = new System.Drawing.Point(0, 0);
			this.lvFreezedFile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.lvFreezedFile.MultiSelect = false;
			this.lvFreezedFile.Name = "lvFreezedFile";
			this.lvFreezedFile.ShowItemToolTips = true;
			this.lvFreezedFile.Size = new System.Drawing.Size(297, 366);
			this.lvFreezedFile.TabIndex = 12;
			this.lvFreezedFile.UseCompatibleStateImageBehavior = false;
			this.lvFreezedFile.View = System.Windows.Forms.View.Details;
			this.lvFreezedFile.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvFile_ItemSelectionChanged);
			this.lvFreezedFile.Click += new System.EventHandler(this.lvFile_Click);
			this.lvFreezedFile.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvFile_MouseClick);
			// 
			// columnHeader8
			// 
			this.columnHeader8.Text = "Name";
			// 
			// columnHeader9
			// 
			this.columnHeader9.Text = "Path";
			this.columnHeader9.Width = 159;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1456, 824);
			this.Controls.Add(this.splitContainer4);
			this.Controls.Add(this.btnApplyAll);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.btnInfer);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "MainForm";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.contextMenuStrip.ResumeLayout(false);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer5.Panel1.ResumeLayout(false);
			this.splitContainer5.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
			this.splitContainer5.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
			this.splitContainer3.ResumeLayout(false);
			this.splitContainer4.Panel1.ResumeLayout(false);
			this.splitContainer4.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
			this.splitContainer4.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Sgry.Azuki.WinForms.AzukiControl azuki;
		private System.Windows.Forms.Button btnInfer;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem menuItemLanguage;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem markToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem unmarkToolStripMenuItem;
		private System.Windows.Forms.ListView lvMarkedElements;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ListView lvWillBeMarkedElements;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.ListView lvModifiableFile;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.ListView lvModifiableRule;
		private System.Windows.Forms.Button btnApplyAll;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.SplitContainer splitContainer4;
		private System.Windows.Forms.ListView lvFreezedFile;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.ColumnHeader columnHeader9;
		private System.Windows.Forms.SplitContainer splitContainer5;
		private System.Windows.Forms.ListView lvFreezedRule;
		private System.Windows.Forms.ColumnHeader columnHeader10;
	}
}

