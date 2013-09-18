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
			this.btnInfer = new System.Windows.Forms.Button();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.menuItemLanguage = new System.Windows.Forms.ToolStripMenuItem();
			this.lvSelectedElements = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lvInferedElements = new System.Windows.Forms.ListView();
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.btnApply = new System.Windows.Forms.Button();
			this.lvFile = new System.Windows.Forms.ListView();
			this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.lvRule = new System.Windows.Forms.ListView();
			this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.btnApplyAll = new System.Windows.Forms.Button();
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
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
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
			this.azuki.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			fontInfo1.Name = "MS UI Gothic";
			fontInfo1.Size = 9;
			fontInfo1.Style = System.Drawing.FontStyle.Regular;
			this.azuki.FontInfo = fontInfo1;
			this.azuki.ForeColor = System.Drawing.Color.Black;
			this.azuki.IsReadOnly = true;
			this.azuki.Location = new System.Drawing.Point(0, 0);
			this.azuki.Name = "azuki";
			this.azuki.Size = new System.Drawing.Size(508, 621);
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
			this.contextMenuStrip.Size = new System.Drawing.Size(124, 48);
			// 
			// markToolStripMenuItem
			// 
			this.markToolStripMenuItem.Name = "markToolStripMenuItem";
			this.markToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.markToolStripMenuItem.Text = "Mark";
			this.markToolStripMenuItem.Click += new System.EventHandler(this.markToolStripMenuItem_Click);
			// 
			// unmarkToolStripMenuItem
			// 
			this.unmarkToolStripMenuItem.Name = "unmarkToolStripMenuItem";
			this.unmarkToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			this.unmarkToolStripMenuItem.Text = "Unmark";
			this.unmarkToolStripMenuItem.Click += new System.EventHandler(this.unmarkToolStripMenuItem_Click);
			// 
			// btnInfer
			// 
			this.btnInfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnInfer.Location = new System.Drawing.Point(9, 625);
			this.btnInfer.Name = "btnInfer";
			this.btnInfer.Size = new System.Drawing.Size(61, 23);
			this.btnInfer.TabIndex = 1;
			this.btnInfer.Text = "Infer";
			this.btnInfer.UseVisualStyleBackColor = true;
			this.btnInfer.Click += new System.EventHandler(this.btnInfer_Click);
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemLanguage});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(1092, 26);
			this.menuStrip.TabIndex = 2;
			// 
			// menuItemLanguage
			// 
			this.menuItemLanguage.Name = "menuItemLanguage";
			this.menuItemLanguage.Size = new System.Drawing.Size(76, 22);
			this.menuItemLanguage.Text = "Language";
			// 
			// lvSelectedElements
			// 
			this.lvSelectedElements.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.lvSelectedElements.ContextMenuStrip = this.contextMenuStrip;
			this.lvSelectedElements.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvSelectedElements.Location = new System.Drawing.Point(0, 0);
			this.lvSelectedElements.Margin = new System.Windows.Forms.Padding(2);
			this.lvSelectedElements.Name = "lvSelectedElements";
			this.lvSelectedElements.ShowItemToolTips = true;
			this.lvSelectedElements.Size = new System.Drawing.Size(165, 326);
			this.lvSelectedElements.TabIndex = 8;
			this.lvSelectedElements.UseCompatibleStateImageBehavior = false;
			this.lvSelectedElements.View = System.Windows.Forms.View.Details;
			this.lvSelectedElements.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvSelectedElements_MouseDoubleClick);
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
			this.lvInferedElements.ContextMenuStrip = this.contextMenuStrip;
			this.lvInferedElements.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvInferedElements.ForeColor = System.Drawing.SystemColors.WindowText;
			this.lvInferedElements.Location = new System.Drawing.Point(0, 0);
			this.lvInferedElements.Margin = new System.Windows.Forms.Padding(2);
			this.lvInferedElements.Name = "lvInferedElements";
			this.lvInferedElements.ShowItemToolTips = true;
			this.lvInferedElements.Size = new System.Drawing.Size(162, 326);
			this.lvInferedElements.TabIndex = 9;
			this.lvInferedElements.UseCompatibleStateImageBehavior = false;
			this.lvInferedElements.View = System.Windows.Forms.View.Details;
			this.lvInferedElements.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvInferedElements_MouseDoubleClick);
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
			this.btnApply.Location = new System.Drawing.Point(75, 625);
			this.btnApply.Margin = new System.Windows.Forms.Padding(2);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(59, 23);
			this.btnApply.TabIndex = 10;
			this.btnApply.Text = "Apply";
			this.btnApply.UseVisualStyleBackColor = true;
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			// 
			// lvFile
			// 
			this.lvFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.lvFile.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6});
			this.lvFile.HideSelection = false;
			this.lvFile.Location = new System.Drawing.Point(9, 27);
			this.lvFile.Margin = new System.Windows.Forms.Padding(2);
			this.lvFile.MultiSelect = false;
			this.lvFile.Name = "lvFile";
			this.lvFile.ShowItemToolTips = true;
			this.lvFile.Size = new System.Drawing.Size(223, 593);
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
			this.columnHeader6.Width = 159;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(237, 27);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.azuki);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(843, 621);
			this.splitContainer1.SplitterDistance = 508;
			this.splitContainer1.TabIndex = 12;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.lvRule);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
			this.splitContainer2.Size = new System.Drawing.Size(331, 621);
			this.splitContainer2.SplitterDistance = 291;
			this.splitContainer2.TabIndex = 0;
			// 
			// lvRule
			// 
			this.lvRule.CheckBoxes = true;
			this.lvRule.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7});
			this.lvRule.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvRule.Location = new System.Drawing.Point(0, 0);
			this.lvRule.Name = "lvRule";
			this.lvRule.Size = new System.Drawing.Size(331, 291);
			this.lvRule.TabIndex = 0;
			this.lvRule.UseCompatibleStateImageBehavior = false;
			this.lvRule.View = System.Windows.Forms.View.Details;
			this.lvRule.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvRule_ItemChecked);
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "Filter";
			this.columnHeader7.Width = 326;
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.lvSelectedElements);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.lvInferedElements);
			this.splitContainer3.Size = new System.Drawing.Size(331, 326);
			this.splitContainer3.SplitterDistance = 165;
			this.splitContainer3.TabIndex = 0;
			// 
			// btnApplyAll
			// 
			this.btnApplyAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnApplyAll.Location = new System.Drawing.Point(138, 625);
			this.btnApplyAll.Margin = new System.Windows.Forms.Padding(2);
			this.btnApplyAll.Name = "btnApplyAll";
			this.btnApplyAll.Size = new System.Drawing.Size(94, 23);
			this.btnApplyAll.TabIndex = 13;
			this.btnApplyAll.Text = "Apply All";
			this.btnApplyAll.UseVisualStyleBackColor = true;
			this.btnApplyAll.Click += new System.EventHandler(this.btnApplyAll_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1092, 659);
			this.Controls.Add(this.btnApplyAll);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.lvFile);
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.btnInfer);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
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
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
			this.splitContainer3.ResumeLayout(false);
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
		private System.Windows.Forms.ListView lvSelectedElements;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ListView lvInferedElements;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.ListView lvFile;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.ListView lvRule;
		private System.Windows.Forms.Button btnApplyAll;
		private System.Windows.Forms.ColumnHeader columnHeader7;
	}
}

