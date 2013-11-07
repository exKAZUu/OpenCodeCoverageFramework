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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Languages.C.CodeToXmls;
using Code2Xml.Languages.Java.CodeToXmls;
using Code2Xml.Languages.Python2.CodeToXmls;
using Code2Xml.Languages.Python3.CodeToXmls;
using Occf.Core.CoverageInformation;
using Occf.Core.Manipulators;
using Occf.Core.Utils;
using Paraiba.Core;
using Paraiba.IO;

namespace Occf.Tools.Window {
	public partial class MainForm : Form {
		private ProgressForm _progressForm;

		public MainForm() {
			InitializeComponent();
		}

		private ProgressForm ProgressForm {
			get { return _progressForm ?? (_progressForm = new ProgressForm()); }
		}

		private void MainFormLoad(object sender, EventArgs e) {
			var exeDir = Path.GetDirectoryName(Application.ExecutablePath);
			txtOutput.Text =
					Path.GetFullPath(Path.Combine(exeDir, "..", "codes", "output"));
			cmbLanguage.SelectedIndex = 0;
		}

		private void MainForm_DragEnter(object sender, DragEventArgs e) {
			e.Effect = DragDropEffects.All;
		}

		private void MainFormDragDrop(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
				var filePathes = ((string[])e.Data.GetData(DataFormats.FileDrop));

				txtBase.Text = Path.GetDirectoryName(filePathes[0]);
				var basePath = txtBase.Text.AddIfNotEndsWith('\\');

				var files = filePathes.SelectMany(path => EnumerateFiles(path, "*"))
						.Select(path => ParaibaPath.GetRelativePath(path, basePath));

				clbFiles.Items.Clear();
				foreach (var file in files) {
					clbFiles.Items.Add(file);
				}
				SetFileCheckedWithCombBox();
			}
		}

		private static IEnumerable<string> EnumerateFiles(string path, string pattern) {
			if (Directory.Exists(path)) {
				var subPathes = Directory.GetFiles(
						path, pattern,
						SearchOption.AllDirectories);
				foreach (var subPath in subPathes) {
					yield return subPath;
				}
			} else if (File.Exists(path)) {
				yield return path;
			}
		}

		private void BtnStartClick(object sender, EventArgs e) {
			var files = clbFiles.CheckedItems.Cast<string>();
			var basePath = txtBase.Text.AddIfNotEndsWith('\\');
			var outDirPath = txtOutput.Text.AddIfNotEndsWith('\\');

			var filePathList = files.ToList();
			var langName = cmbLanguage.Text;

			Action action = () => {
				var profile = LanguageSupports.GetCoverageModeByClassName(langName);
				var info = new CoverageInfo(basePath, profile.Name, SharingMethod.File);
				var outDir = new DirectoryInfo(outDirPath);
				foreach (var filePath in filePathList) {
					OccfCodeGenerator.WriteCoveragedCode(
							profile, info, new FileInfo(filePath), outDir);
				}
				info.Write(new DirectoryInfo(outDirPath));
			};
			ProgressForm.Start(this, filePathList.Count, action);
		}

		private void CmbLanguageSelectedIndexChanged(object sender, EventArgs e) {
			SetFileCheckedWithCombBox();
		}

		private void SetFileCheckedWithCombBox() {
			switch (cmbLanguage.Text) {
			case "C":
				SetItemCheckedWithAstGenerator(CCodeToXml.Instance);
				break;
			case "Java":
				SetItemCheckedWithAstGenerator(JavaCodeToXml.Instance);
				break;
			case "Python2":
				SetItemCheckedWithAstGenerator(Python2CodeToXml.Instance);
				break;
			case "Python3":
				SetItemCheckedWithAstGenerator(Python3CodeToXml.Instance);
				break;
			}
		}

		private void SetItemCheckedWithAstGenerator(CodeToXml xmlGenerator) {
			var extensions = xmlGenerator.TargetExtensions;
			var count = clbFiles.Items.Count;
			for (int i = 0; i < count; i++) {
				var relativePath = clbFiles.Items[i] as string;
				var isTargetFile =
						extensions.Contains(Path.GetExtension(relativePath).ToLower());
				clbFiles.SetItemChecked(i, isTargetFile);
			}
		}

		private void btnReporter_Click(object sender, EventArgs e) {
			var info = new ProcessStartInfo {
					FileName = "OpenCodeCoverageFramework.Reporter.exe",
					Arguments = Path.Combine(txtOutput.Text, "coverageinfo"),
			};
			Process.Start(info);
		}
	}
}