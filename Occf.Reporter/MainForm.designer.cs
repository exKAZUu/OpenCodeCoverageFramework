namespace Occf.Reporter
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
			this._tbcCoverage = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this._lvStatement = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this._lbStatement = new System.Windows.Forms.Label();
			this._pgbStatement = new System.Windows.Forms.ProgressBar();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this._lvBranch = new System.Windows.Forms.ListView();
			this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this._lbBranch = new System.Windows.Forms.Label();
			this._pgbBranch = new System.Windows.Forms.ProgressBar();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this._lvCondition = new System.Windows.Forms.ListView();
			this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this._lbCondition = new System.Windows.Forms.Label();
			this._pgbCondition = new System.Windows.Forms.ProgressBar();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this._lvBranchCond = new System.Windows.Forms.ListView();
			this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this._lbBranchCond = new System.Windows.Forms.Label();
			this._pgbBranchCond = new System.Windows.Forms.ProgressBar();
			this.label1 = new System.Windows.Forms.Label();
			this._txtCovInfoPath = new System.Windows.Forms.TextBox();
			this._lbTag = new System.Windows.Forms.ListBox();
			this._btnAnalyze = new System.Windows.Forms.Button();
			this.btnMeasure = new System.Windows.Forms.Button();
			this._tbcCoverage.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tbcCoverage
			// 
			this._tbcCoverage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this._tbcCoverage.Controls.Add(this.tabPage1);
			this._tbcCoverage.Controls.Add(this.tabPage2);
			this._tbcCoverage.Controls.Add(this.tabPage3);
			this._tbcCoverage.Controls.Add(this.tabPage4);
			this._tbcCoverage.Location = new System.Drawing.Point(8, 40);
			this._tbcCoverage.Name = "_tbcCoverage";
			this._tbcCoverage.SelectedIndex = 0;
			this._tbcCoverage.Size = new System.Drawing.Size(312, 370);
			this._tbcCoverage.TabIndex = 1;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this._lvStatement);
			this.tabPage1.Controls.Add(this._lbStatement);
			this.tabPage1.Controls.Add(this._pgbStatement);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(304, 344);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Statement";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// _lvStatement
			// 
			this._lvStatement.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lvStatement.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
			this._lvStatement.FullRowSelect = true;
			this._lvStatement.Location = new System.Drawing.Point(8, 40);
			this._lvStatement.Name = "_lvStatement";
			this._lvStatement.Size = new System.Drawing.Size(288, 296);
			this._lvStatement.TabIndex = 2;
			this._lvStatement.UseCompatibleStateImageBehavior = false;
			this._lvStatement.View = System.Windows.Forms.View.Details;
			this._lvStatement.DoubleClick += new System.EventHandler(this.LvStatementDoubleClick);
			this._lvStatement.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LvStatementMouseDown);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Y/N";
			this.columnHeader1.Width = 36;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "FileName";
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Line";
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Position";
			// 
			// _lbStatement
			// 
			this._lbStatement.AutoSize = true;
			this._lbStatement.Location = new System.Drawing.Point(8, 16);
			this._lbStatement.Name = "_lbStatement";
			this._lbStatement.Size = new System.Drawing.Size(0, 12);
			this._lbStatement.TabIndex = 1;
			// 
			// _pgbStatement
			// 
			this._pgbStatement.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._pgbStatement.Location = new System.Drawing.Point(96, 8);
			this._pgbStatement.Name = "_pgbStatement";
			this._pgbStatement.Size = new System.Drawing.Size(200, 24);
			this._pgbStatement.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this._pgbStatement.TabIndex = 0;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this._lvBranch);
			this.tabPage2.Controls.Add(this._lbBranch);
			this.tabPage2.Controls.Add(this._pgbBranch);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(304, 344);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Decision";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// _lvBranch
			// 
			this._lvBranch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lvBranch.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
			this._lvBranch.Location = new System.Drawing.Point(8, 40);
			this._lvBranch.Name = "_lvBranch";
			this._lvBranch.Size = new System.Drawing.Size(288, 296);
			this._lvBranch.TabIndex = 5;
			this._lvBranch.UseCompatibleStateImageBehavior = false;
			this._lvBranch.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Y/N";
			this.columnHeader5.Width = 36;
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "FileName";
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "Line";
			// 
			// columnHeader8
			// 
			this.columnHeader8.Text = "Position";
			// 
			// _lbBranch
			// 
			this._lbBranch.AutoSize = true;
			this._lbBranch.Location = new System.Drawing.Point(8, 16);
			this._lbBranch.Name = "_lbBranch";
			this._lbBranch.Size = new System.Drawing.Size(0, 12);
			this._lbBranch.TabIndex = 4;
			// 
			// _pgbBranch
			// 
			this._pgbBranch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._pgbBranch.Location = new System.Drawing.Point(96, 8);
			this._pgbBranch.Name = "_pgbBranch";
			this._pgbBranch.Size = new System.Drawing.Size(200, 24);
			this._pgbBranch.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this._pgbBranch.TabIndex = 3;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this._lvCondition);
			this.tabPage3.Controls.Add(this._lbCondition);
			this.tabPage3.Controls.Add(this._pgbCondition);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(304, 344);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Condition";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// _lvCondition
			// 
			this._lvCondition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lvCondition.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12});
			this._lvCondition.Location = new System.Drawing.Point(8, 40);
			this._lvCondition.Name = "_lvCondition";
			this._lvCondition.Size = new System.Drawing.Size(288, 296);
			this._lvCondition.TabIndex = 6;
			this._lvCondition.UseCompatibleStateImageBehavior = false;
			this._lvCondition.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader9
			// 
			this.columnHeader9.Text = "Y/N";
			this.columnHeader9.Width = 36;
			// 
			// columnHeader10
			// 
			this.columnHeader10.Text = "FileName";
			// 
			// columnHeader11
			// 
			this.columnHeader11.Text = "Line";
			// 
			// columnHeader12
			// 
			this.columnHeader12.Text = "Position";
			// 
			// _lbCondition
			// 
			this._lbCondition.AutoSize = true;
			this._lbCondition.Location = new System.Drawing.Point(8, 16);
			this._lbCondition.Name = "_lbCondition";
			this._lbCondition.Size = new System.Drawing.Size(0, 12);
			this._lbCondition.TabIndex = 4;
			// 
			// _pgbCondition
			// 
			this._pgbCondition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._pgbCondition.Location = new System.Drawing.Point(96, 8);
			this._pgbCondition.Name = "_pgbCondition";
			this._pgbCondition.Size = new System.Drawing.Size(200, 24);
			this._pgbCondition.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this._pgbCondition.TabIndex = 3;
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this._lvBranchCond);
			this.tabPage4.Controls.Add(this._lbBranchCond);
			this.tabPage4.Controls.Add(this._pgbBranchCond);
			this.tabPage4.Location = new System.Drawing.Point(4, 22);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage4.Size = new System.Drawing.Size(304, 344);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "Condition/Decision";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// _lvBranchCond
			// 
			this._lvBranchCond.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lvBranchCond.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader13,
            this.columnHeader14,
            this.columnHeader15,
            this.columnHeader16});
			this._lvBranchCond.Location = new System.Drawing.Point(8, 40);
			this._lvBranchCond.Name = "_lvBranchCond";
			this._lvBranchCond.Size = new System.Drawing.Size(288, 296);
			this._lvBranchCond.TabIndex = 7;
			this._lvBranchCond.UseCompatibleStateImageBehavior = false;
			this._lvBranchCond.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader13
			// 
			this.columnHeader13.Text = "Y/N";
			this.columnHeader13.Width = 36;
			// 
			// columnHeader14
			// 
			this.columnHeader14.Text = "FileName";
			// 
			// columnHeader15
			// 
			this.columnHeader15.Text = "Line";
			// 
			// columnHeader16
			// 
			this.columnHeader16.Text = "Position";
			// 
			// _lbBranchCond
			// 
			this._lbBranchCond.AutoSize = true;
			this._lbBranchCond.Location = new System.Drawing.Point(8, 16);
			this._lbBranchCond.Name = "_lbBranchCond";
			this._lbBranchCond.Size = new System.Drawing.Size(0, 12);
			this._lbBranchCond.TabIndex = 4;
			// 
			// _pgbBranchCond
			// 
			this._pgbBranchCond.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._pgbBranchCond.Location = new System.Drawing.Point(96, 8);
			this._pgbBranchCond.Name = "_pgbBranchCond";
			this._pgbBranchCond.Size = new System.Drawing.Size(200, 24);
			this._pgbBranchCond.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this._pgbBranchCond.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(0, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 12);
			this.label1.TabIndex = 6;
			this.label1.Text = "CoverageInfo";
			// 
			// _txtCovInfoPath
			// 
			this._txtCovInfoPath.AllowDrop = true;
			this._txtCovInfoPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtCovInfoPath.Location = new System.Drawing.Point(72, 8);
			this._txtCovInfoPath.Name = "_txtCovInfoPath";
			this._txtCovInfoPath.ReadOnly = true;
			this._txtCovInfoPath.Size = new System.Drawing.Size(592, 19);
			this._txtCovInfoPath.TabIndex = 5;
			// 
			// _lbTag
			// 
			this._lbTag.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lbTag.FormattingEnabled = true;
			this._lbTag.ItemHeight = 12;
			this._lbTag.Location = new System.Drawing.Point(328, 40);
			this._lbTag.Name = "_lbTag";
			this._lbTag.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this._lbTag.Size = new System.Drawing.Size(422, 328);
			this._lbTag.TabIndex = 7;
			// 
			// _btnAnalyze
			// 
			this._btnAnalyze.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._btnAnalyze.Location = new System.Drawing.Point(328, 376);
			this._btnAnalyze.Name = "_btnAnalyze";
			this._btnAnalyze.Size = new System.Drawing.Size(422, 32);
			this._btnAnalyze.TabIndex = 9;
			this._btnAnalyze.Text = "Analyze";
			this._btnAnalyze.UseVisualStyleBackColor = true;
			this._btnAnalyze.Click += new System.EventHandler(this.BtnAnalyzeClick);
			// 
			// btnMeasure
			// 
			this.btnMeasure.Location = new System.Drawing.Point(672, 8);
			this.btnMeasure.Name = "btnMeasure";
			this.btnMeasure.Size = new System.Drawing.Size(80, 23);
			this.btnMeasure.TabIndex = 10;
			this.btnMeasure.Text = "Measure";
			this.btnMeasure.UseVisualStyleBackColor = true;
			this.btnMeasure.Click += new System.EventHandler(this.BtnMeasureClick);
			// 
			// MainForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(759, 415);
			this.Controls.Add(this.btnMeasure);
			this.Controls.Add(this._btnAnalyze);
			this.Controls.Add(this._lbTag);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._txtCovInfoPath);
			this.Controls.Add(this._tbcCoverage);
			this.Name = "MainForm";
			this.Text = "CoverageReport";
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
			this._tbcCoverage.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			this.tabPage4.ResumeLayout(false);
			this.tabPage4.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl _tbcCoverage;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.Label _lbStatement;
		private System.Windows.Forms.ProgressBar _pgbStatement;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label _lbBranch;
		private System.Windows.Forms.ProgressBar _pgbBranch;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.Label _lbCondition;
		private System.Windows.Forms.ProgressBar _pgbCondition;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.Label _lbBranchCond;
		private System.Windows.Forms.ProgressBar _pgbBranchCond;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _txtCovInfoPath;
		private System.Windows.Forms.ListBox _lbTag;
		private System.Windows.Forms.Button _btnAnalyze;
		private System.Windows.Forms.ListView _lvStatement;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ListView _lvBranch;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.ListView _lvCondition;
		private System.Windows.Forms.ColumnHeader columnHeader9;
		private System.Windows.Forms.ColumnHeader columnHeader10;
		private System.Windows.Forms.ColumnHeader columnHeader11;
		private System.Windows.Forms.ColumnHeader columnHeader12;
		private System.Windows.Forms.ListView _lvBranchCond;
		private System.Windows.Forms.ColumnHeader columnHeader13;
		private System.Windows.Forms.ColumnHeader columnHeader14;
		private System.Windows.Forms.ColumnHeader columnHeader15;
		private System.Windows.Forms.ColumnHeader columnHeader16;
		private System.Windows.Forms.Button btnMeasure;
	}
}

