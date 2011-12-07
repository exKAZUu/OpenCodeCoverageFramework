namespace Occf.Tools.Window
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
			this.label1 = new System.Windows.Forms.Label();
			this.cmbLanguage = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtOutput = new System.Windows.Forms.TextBox();
			this.clbFiles = new System.Windows.Forms.CheckedListBox();
			this.btnStart = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.txtWithParents = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtWithoutParents = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtWithoutChildren = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.txtWithChildren = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.txtBase = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.txtWithoutLittleSiblings = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.txtWithLittleSiblings = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.txtWithoutBigSiblings = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.txtWithBigSiblings = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnReporter = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 12);
			this.label1.TabIndex = 5;
			this.label1.Text = "Language";
			// 
			// cmbLanguage
			// 
			this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbLanguage.FormattingEnabled = true;
			this.cmbLanguage.Items.AddRange(new object[] {
            "Java",
            "Python2",
            "Python3",
            "C"});
			this.cmbLanguage.Location = new System.Drawing.Point(64, 8);
			this.cmbLanguage.Name = "cmbLanguage";
			this.cmbLanguage.Size = new System.Drawing.Size(288, 20);
			this.cmbLanguage.TabIndex = 6;
			this.cmbLanguage.SelectedIndexChanged += new System.EventHandler(this.CmbLanguageSelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 36);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(62, 12);
			this.label2.TabIndex = 8;
			this.label2.Text = "OutputPath";
			// 
			// txtOutput
			// 
			this.txtOutput.Location = new System.Drawing.Point(72, 32);
			this.txtOutput.Name = "txtOutput";
			this.txtOutput.Size = new System.Drawing.Size(280, 19);
			this.txtOutput.TabIndex = 7;
			// 
			// clbFiles
			// 
			this.clbFiles.FormattingEnabled = true;
			this.clbFiles.Location = new System.Drawing.Point(8, 320);
			this.clbFiles.Name = "clbFiles";
			this.clbFiles.Size = new System.Drawing.Size(344, 200);
			this.clbFiles.TabIndex = 10;
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(8, 528);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(168, 32);
			this.btnStart.TabIndex = 13;
			this.btnStart.Text = "Insert Measurement Code";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.BtnStartClick);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(8, 20);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82, 12);
			this.label3.TabIndex = 15;
			this.label3.Text = "Parents(XPath)";
			// 
			// txtWithParents
			// 
			this.txtWithParents.Location = new System.Drawing.Point(120, 16);
			this.txtWithParents.Name = "txtWithParents";
			this.txtWithParents.Size = new System.Drawing.Size(216, 19);
			this.txtWithParents.TabIndex = 14;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(8, 20);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(82, 12);
			this.label4.TabIndex = 17;
			this.label4.Text = "Parents(XPath)";
			// 
			// txtWithoutParents
			// 
			this.txtWithoutParents.Location = new System.Drawing.Point(120, 16);
			this.txtWithoutParents.Name = "txtWithoutParents";
			this.txtWithoutParents.Size = new System.Drawing.Size(216, 19);
			this.txtWithoutParents.TabIndex = 16;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(8, 44);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(85, 12);
			this.label5.TabIndex = 21;
			this.label5.Text = "Children(XPath)";
			// 
			// txtWithoutChildren
			// 
			this.txtWithoutChildren.Location = new System.Drawing.Point(120, 40);
			this.txtWithoutChildren.Name = "txtWithoutChildren";
			this.txtWithoutChildren.Size = new System.Drawing.Size(216, 19);
			this.txtWithoutChildren.TabIndex = 20;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(8, 44);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(85, 12);
			this.label6.TabIndex = 19;
			this.label6.Text = "Children(XPath)";
			// 
			// txtWithChildren
			// 
			this.txtWithChildren.Location = new System.Drawing.Point(120, 40);
			this.txtWithChildren.Name = "txtWithChildren";
			this.txtWithChildren.Size = new System.Drawing.Size(216, 19);
			this.txtWithChildren.TabIndex = 18;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(8, 60);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(54, 12);
			this.label7.TabIndex = 23;
			this.label7.Text = "BasePath";
			// 
			// txtBase
			// 
			this.txtBase.Location = new System.Drawing.Point(72, 56);
			this.txtBase.Name = "txtBase";
			this.txtBase.Size = new System.Drawing.Size(280, 19);
			this.txtBase.TabIndex = 22;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(8, 92);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(109, 12);
			this.label8.TabIndex = 31;
			this.label8.Text = "AfterSiblings(XPath)";
			// 
			// txtWithoutLittleSiblings
			// 
			this.txtWithoutLittleSiblings.Location = new System.Drawing.Point(120, 88);
			this.txtWithoutLittleSiblings.Name = "txtWithoutLittleSiblings";
			this.txtWithoutLittleSiblings.Size = new System.Drawing.Size(216, 19);
			this.txtWithoutLittleSiblings.TabIndex = 30;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(8, 92);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(109, 12);
			this.label9.TabIndex = 29;
			this.label9.Text = "AfterSiblings(XPath)";
			// 
			// txtWithLittleSiblings
			// 
			this.txtWithLittleSiblings.Location = new System.Drawing.Point(120, 88);
			this.txtWithLittleSiblings.Name = "txtWithLittleSiblings";
			this.txtWithLittleSiblings.Size = new System.Drawing.Size(216, 19);
			this.txtWithLittleSiblings.TabIndex = 28;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(8, 68);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(111, 12);
			this.label10.TabIndex = 27;
			this.label10.Text = "BeforeSibling(XPath)";
			// 
			// txtWithoutBigSiblings
			// 
			this.txtWithoutBigSiblings.Location = new System.Drawing.Point(120, 64);
			this.txtWithoutBigSiblings.Name = "txtWithoutBigSiblings";
			this.txtWithoutBigSiblings.Size = new System.Drawing.Size(216, 19);
			this.txtWithoutBigSiblings.TabIndex = 26;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(8, 68);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(111, 12);
			this.label11.TabIndex = 25;
			this.label11.Text = "BeforeSibling(XPath)";
			// 
			// txtWithBigSiblings
			// 
			this.txtWithBigSiblings.Location = new System.Drawing.Point(120, 64);
			this.txtWithBigSiblings.Name = "txtWithBigSiblings";
			this.txtWithBigSiblings.Size = new System.Drawing.Size(216, 19);
			this.txtWithBigSiblings.TabIndex = 24;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtWithParents);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.txtWithChildren);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.txtWithBigSiblings);
			this.groupBox1.Controls.Add(this.txtWithLittleSiblings);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Location = new System.Drawing.Point(8, 80);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(344, 112);
			this.groupBox1.TabIndex = 32;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Include";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.txtWithoutParents);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.txtWithoutChildren);
			this.groupBox2.Controls.Add(this.txtWithoutLittleSiblings);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.txtWithoutBigSiblings);
			this.groupBox2.Location = new System.Drawing.Point(8, 200);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(344, 112);
			this.groupBox2.TabIndex = 33;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Exclude";
			// 
			// btnReporter
			// 
			this.btnReporter.Location = new System.Drawing.Point(184, 528);
			this.btnReporter.Name = "btnReporter";
			this.btnReporter.Size = new System.Drawing.Size(168, 32);
			this.btnReporter.TabIndex = 34;
			this.btnReporter.Text = "Run Coverage Reporter";
			this.btnReporter.UseVisualStyleBackColor = true;
			this.btnReporter.Click += new System.EventHandler(this.btnReporter_Click);
			// 
			// MainForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(361, 566);
			this.Controls.Add(this.btnReporter);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.txtBase);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.clbFiles);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtOutput);
			this.Controls.Add(this.cmbLanguage);
			this.Controls.Add(this.label1);
			this.Name = "MainForm";
			this.Text = "CodeCoverageWeaver";
			this.Load += new System.EventHandler(this.MainFormLoad);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainFormDragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cmbLanguage;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtOutput;
		private System.Windows.Forms.CheckedListBox clbFiles;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtWithParents;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtWithoutParents;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtWithoutChildren;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtWithChildren;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtBase;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtWithoutLittleSiblings;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox txtWithLittleSiblings;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox txtWithoutBigSiblings;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox txtWithBigSiblings;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnReporter;
	}
}

