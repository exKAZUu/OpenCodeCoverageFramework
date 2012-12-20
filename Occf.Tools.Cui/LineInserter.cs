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

using System.Collections.Generic;
using System.IO;
using Occf.Core.Utils;

//using Occf.Tools.Cui;

namespace Occf.Tools.Cui {
	public class LineInserter {
		private const string S = "  ";
		private const int W = 20;

		private static readonly string Usage =
				Program.Header +
						"" + "\n" +
						"Usage: Occf line_insert <root> [<test>]" + "\n" +
						"" + "\n" +
						S + "<root>".PadRight(W)
						+ "path of root directory" + "\n" +
						S + "<test>".PadRight(W) + "path of test code directory" + "\n" +
						"";

		public static bool Run(IList<string> args) {
			if (args.Count < 1) {
				return Program.Print(Usage);
			}

			var rootDir = new DirectoryInfo(args[0]);
			if (!rootDir.Exists) {
				return
						Program.Print(
								"Root directory doesn't exist.\nroot:" + rootDir.FullName);
			}

			DirectoryInfo testDir = null;
			if (args.Count >= 2) {
				testDir = new DirectoryInfo(args[1]);
				if (!testDir.Exists) {
					return
							Program.Print(
									"Error: test code directory doesn't exist.\ntest:" + testDir.FullName);
				}
			}

			LineInserts(rootDir, testDir);

			return true;
		}

		//引数に入力ファイルのディレクトリのパス.c、.cpp、.cxxのみにフィルタリングして挿入 
		//テストディレクトリ以下は除外
		private static void LineInserts(DirectoryInfo rootDir, DirectoryInfo testDir) {
			var insertList = new List<FileInfo>();

			insertList.AddRange(rootDir.GetFiles("*.c", SearchOption.AllDirectories));
			insertList.AddRange(rootDir.GetFiles("*.cpp", SearchOption.AllDirectories));
			insertList.AddRange(rootDir.GetFiles("*.cxx", SearchOption.AllDirectories));

			if (testDir != null) {
				//注意:　i>0⇒i>=0　に変更　要確認
				for (var i = insertList.Count - 1; i >= 0; i--) {
					if (insertList[i].FullName.StartsWith(testDir.FullName)) {
						insertList.Remove(insertList[i]);
					}
				}
			}

			foreach (var fileInfo in insertList) {
				WriteInsetLine(fileInfo.FullName);
			}
		}

		//指定されたファイルのパスを受け取って、指定名のバックアップファイルを作成して挿入
		private static void WriteInsetLine(string defaultFileFullName) {
			var fileInfo = new FileInfo(defaultFileFullName);
			const string appendExtension = OccfNames.LineBackUpSuffix;
			;
			var backUpFileFullName = defaultFileFullName + appendExtension;

			File.Copy(defaultFileFullName, backUpFileFullName);

			using (var reader = new StreamReader(backUpFileFullName)) {
				using (var writer = new StreamWriter(fileInfo.FullName, false)) {
					string line;
					var lineNum = 1;

					while ((line = reader.ReadLine()) != null) {
						writer.WriteLine("#line " + lineNum);
						writer.WriteLine(line);
						lineNum++;
					}
					reader.Close();
					writer.Close();
				}
			}
		}
	}
}