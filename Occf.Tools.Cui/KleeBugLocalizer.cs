﻿#region License

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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Code2Xml.Core.Position;
using NDesk.Options;
using Occf.Core.CoverageInformation;
using Occf.Core.TestInformation;
using Occf.Core.Utils;
using Paraiba.IO;

namespace Occf.Tools.Cui {
	public class KleeBugLocalizer {
		private const string S = "  ";
		private const int W = 20;

		private static readonly string Usage =
				Program.Header +
						"" + "\n" +
						"Usage: Occf klee -r <root_dir> -t <test_dir> [options]" + "\n" +
						"" + "\n" +
						S + "-r -root<root_dir>".PadRight(W) 
						+ "a path of a directory containing '.occf_coverage_info'"
						+ "\n" +
						S + "-t -test <test_dir>".PadRight(W) 
						+ "a path of klee test directory" + "\n" +
						S + "-m -metrics <metrics>".PadRight(W)
						+ "type of metrics of fault localization."+ "\n" + 
						S + "".PadRight(W) + "tar[antura], och[iai], jac[card] or rus[sell]"
						+ "\n" +
						S + "-v -csv <output_csv_dir>".PadRight(W)
						+ "path of the directory for csv file for output" 
						+ "\n" +
						S + "-u -success <successfull_csvfile_path>".PadRight(W)
						+ "path of the successfull csv file"
						+ "\n" +
						"";

		public static bool Run(IList<string> args) {

			var rootDirPath = "";
			var testDirPath = "";
			var metricsType = "";
			var csvDirPath = "";
			var passFilePath = "";
			
			// parse options

			var p = new OptionSet {
					{ "r|root=", v => rootDirPath = v},
					{ "t|test=", v=> testDirPath = v },
					{ "m|metrics=", v => metricsType = v },
					{ "v|csv=", v => csvDirPath = v },
					{ "u|success=", v => passFilePath = v },
			};

			// コマンドをパース "-"指定されないのだけargs[]に残る
			try {
				args = p.Parse(args);
			} catch {
				Console.WriteLine("catch");
				return Program.Print(Usage);
			}

			if (args.Count > 0) {
				return Program.Print(Usage);
			}

			if (string.IsNullOrEmpty(rootDirPath)) {
				return Program.Print(Usage);
			}

			var rootDir = new DirectoryInfo(rootDirPath);
			if (!rootDir.Exists) {
				return
						Program.Print(
								"Root directory doesn't exist.\nroot:" + rootDir.FullName);
			}

			if (string.IsNullOrEmpty(testDirPath)) {
				return Program.Print(Usage);
			}

			var testDir = new DirectoryInfo(testDirPath);
			if (!rootDir.Exists) {
				return
						Program.Print(
								"test directory doesn't exist.\ntest:" + testDir.FullName);
			}

			var metricsFileName = "BugLocalization.py";
			if (!string.IsNullOrEmpty(metricsType)) {
				//裏短縮コード
				switch (metricsType){
					case "tar":
						metricsType = "Tarantula.py";
						break;
					case "och":
						metricsType = "Ochiai.py";
						break;
					case "jac":
						metricsType = "Jaccard.py";
						break;
					case "rus":
						metricsType = "Russell.py";
						break;
				}

				if (!metricsType.EndsWith(".py")){
					metricsType += ".py";
				}

				metricsFileName = metricsType;
				var metricsFileInfo = new FileInfo(metricsType);
				if (!metricsFileInfo.Exists){
					Console.WriteLine("Error: not find \"" + metricsFileName + "\"");
					Console.WriteLine("Path: " + metricsFileInfo.FullName);
					Console.WriteLine("chage default file");
					metricsFileName = "BugLocalization.py";
				}
			}

			DirectoryInfo csvDir = null;
			if (!string.IsNullOrEmpty(csvDirPath)) {
				csvDir = new DirectoryInfo(csvDirPath);
				if (!csvDir.Exists) {
					return
							Program.Print(
									"Csv-File directory doesn't exist.\ncsv:" + csvDir.FullName);
				}
			}

			FileInfo passFile = null;
			if (!string.IsNullOrEmpty(passFilePath)) {
				passFile = new FileInfo(passFilePath);
				if (!passFile.Exists) {
					return
							Program.Print(
									"successfull_file does'nt exit .\nsuccesfull_file" + passFile.FullName);
				}
			}

			Localize(rootDir, testDir, metricsFileName, csvDir, passFile);
			return true;
		}

		private static void Localize(DirectoryInfo rootDir, DirectoryInfo testDir, 
									 string metricsFileName, DirectoryInfo csvDir,
									 FileInfo passFile) {

			var formatter = new BinaryFormatter();
			var covInfoFile = FileUtil.GetCoverageInfo(rootDir);
			var covInfo = CoverageInfo.ReadCoverageInfo(covInfoFile, formatter);
			var testInfo = AnalyzeKleeTestFiles(testDir);

			if (passFile != null) {
				AnalyzeTestResultCsv(passFile, testInfo);
			} else {
				AnalyzeTestResult(rootDir, testInfo);    
			}
			
			//Line対応のMapのMapを作成、
			var lineDic = new Dictionary<FileInfo, Dictionary<int, int>>();
			var mapFileInfo = new FileInfo(rootDir.FullName + "/" + OccfNames.LineMapping);
			if (mapFileInfo.Exists) {
				lineDic = MapDicCreater(mapFileInfo);
			} else {
				Console.WriteLine("\"" + OccfNames.LineMapping + "\" file is not found.");
			}
			
			BugLocalizer.LocalizeStatements(testInfo, covInfo, lineDic, metricsFileName);

			if (csvDir != null) {
				BugLocalizer.LocalizeStatementsCsv(csvDir, testInfo, covInfo, lineDic);
			}
		}

		private static TestInfo AnalyzeKleeTestFiles(DirectoryInfo testDirInfo) {
			var files = testDirInfo.EnumerateFiles("*.ktest");
			var testInfo = new TestInfo(testDirInfo.FullName);
			testInfo.InitializeForStoringData(false);
			foreach (var file in files) {
				var relativePath = ParaibaPath.GetRelativePath(file.FullName, testDirInfo.FullName);
				var testCase = new TestCase(relativePath, file.FullName, new CodePosition());
				testInfo.TestCases.Add(testCase);
				testCase.InitializeForStoringData(false);
				var dataPath = file.FullName + OccfNames.CoverageData;
				CoverageDataReader.ReadFile(testInfo, dataPath, testCase);
			}
			return testInfo;
		}

		private static void AnalyzeTestResult(DirectoryInfo rootDirInfo, TestInfo testInfo) {
			var fileInfo = rootDirInfo.GetFile(OccfNames.SuccessfulTests);
			using (var reader = fileInfo.OpenText()) {
				foreach (var line in reader.ReadLines()) {
					var testCase = testInfo.TestCases.FirstOrDefault(t => t.Name.EndsWith(line));
					if (testCase != null) {
						testCase.Passed = true;
					} else {
						Console.Error.WriteLine("[WARNING] the testcase of '" + line + "' is not founded.");
					}
				}
			}
		}

		private static void AnalyzeTestResultCsv(FileInfo csvFileInfo, TestInfo testInfo) {
			
			var passTestLists = new List<string>();
			var spriter = new[] { ',' };

			using (var reader = csvFileInfo.OpenText()) {
				foreach (var line in reader.ReadLines()) {
					passTestLists.AddRange(line.Split(spriter).ToList());
				}
				reader.Close();
			}

			foreach (var passTest in passTestLists) {
				var testCase = testInfo.TestCases.FirstOrDefault(t => t.Name.EndsWith(passTest));
				if (testCase != null) {
					testCase.Passed = true;
				} else {
					Console.Error.WriteLine("[WARNING] the testcase of '" + passTest + "' is not founded.");
				}
			}
		}

		public static Dictionary<FileInfo, Dictionary<int, int>> MapDicCreater(FileInfo mappingFile) {
			// <path, <now, true>>
			var mapDic = new Dictionary<FileInfo, Dictionary<int, int>>();

			using (var reader = new StreamReader(mappingFile.FullName)) {
				var lineDic = new Dictionary<int, int>();
				var lastFileInfo = new FileInfo(reader.ReadLine());
				var csvSpriter = new[] {','};
				var linePare = reader.ReadLine().Split(csvSpriter);
				int nowLine;
				int lineDiff;
				int.TryParse(linePare[0], out nowLine);
				int.TryParse(linePare[1], out lineDiff);

				lineDic.Add(nowLine, lineDiff);

				string line;
				while ((line = reader.ReadLine()) != null) {

					linePare = line.Split(csvSpriter);
					//line or path
					if(int.TryParse(linePare[0], out nowLine) && linePare.Length == 2) {
						//nowline,lineDiff
						int.TryParse(linePare[1], out lineDiff);
						lineDic.Add(nowLine, lineDiff);
					} else {
						//path
						mapDic.Add(lastFileInfo, new Dictionary<int, int>(lineDic));
						lineDic.Clear();
						lastFileInfo = new FileInfo(line);
					}
				}
				mapDic.Add(lastFileInfo, lineDic);
				reader.Close();
			}

			return mapDic;
		}

		
	}
}