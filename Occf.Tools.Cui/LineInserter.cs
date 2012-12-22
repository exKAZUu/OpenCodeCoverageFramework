<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
=======
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

using System.Collections.Generic;
>>>>>>> 3587ca0c216605ba8970ea2d1825980021828d64
using System.IO;
using Occf.Core.Utils;

//using Occf.Tools.Cui;

namespace Occf.Tools.Cui {
<<<<<<< HEAD
    public class LineInserter {
        private const string S = "  ";
        private const int W = 22;

        private static readonly string Usage = 
            Program.Header +
            "" + "\n" +
            "Usage: Occf line_insert [options] [<file_path>]" 
            + "\n" +
            "" + "\n" +
            S + "-s, -srcdir <src_dir>".PadRight(W)
            + "path of the root directory of the sou rce that have to " 
            + "\n" + S + "".PadRight(W) + "line inserted."
            + "\n" +
            S + "-t, -test <test_dir>".PadRight(W) 
            + "path of test code directory" + "\n" +
            S + "-p, -pattern <name>".PadRight(W)
            + "search pattern for exploring target source files "
            + "\n" + S + "".PadRight(W) + "in the src_dir" 
            + "\n" +
            S + "<file_path>".PadRight(W)
            + "file path when you want to specify the file directly." 
            + "\n\n" +
            S + "you have to specify at least one of src_dir or file_path." 
            + "\n" +
            "";

        public static bool Run (IList<string> args) {
            var srcDirPath = "";
            var testDirPath = "";

            var patterns = new List<string>();
            var filePaths = new List<string>();

            if (args.Count <= 0) {
                return Program.Print(Usage);
            }

            // parse oputions
            var p = new OptionSet {
                    { "s|srcdir=", v => srcDirPath = v },
                    { "t|rest=", v => testDirPath = v},
                    { "p|pattern=", patterns.Add },
            };

            try {
                args = p.Parse(args);
            } catch {
                Console.WriteLine("catch");
                return Program.Print(Usage);
            }

            filePaths = args.ToList();
            
            DirectoryInfo srcDir = null;
            if (!string.IsNullOrEmpty(srcDirPath)) {
                srcDir = new DirectoryInfo(srcDirPath);
                if (!srcDir.Exists) {
                    return
                            Program.Print(
                                    "root directory of source doesn't exist.\nsrcdir:"  + srcDir.FullName);
                }
            }

            DirectoryInfo testDir = null;
            if (!string.IsNullOrEmpty(testDirPath)) {
                testDir = new DirectoryInfo(testDirPath);
                if (!testDir.Exists) {
                    return
                            Program.Print(
                                    "Error: test code directory doesn't exist.\ntest:" + testDir.FullName);
                }
            }

            var fileInfos = new List<FileInfo>();
            foreach (var path in filePaths) {

                if (string.IsNullOrEmpty(path)) {
                    return
                            Program.Print(
                                    "Error: there is file path that is null or empty.");
                }
                
                var fileInfo = new FileInfo(path);

                if (!fileInfo.Exists) {
                    return
                            Program.Print(
                                    "Error: file path doesn't exist.\n file path:" + fileInfo.FullName);
                }
                fileInfos.Add(fileInfo);
            }

            if (patterns.Any() && srcDir==null) {
                return
                        Program.Print(
                                "when you need -pattern option, you have to use -srcdir option.");
            }

            LineInserts(srcDir, testDir, fileInfos, patterns);
            
            return true;
        }

        //引数に入力ファイルのディレクトリのパス.c、.cpp、.cxxのみにフィルタリングして挿入 
        //テストディレクトリ以下は除外
        private static void LineInserts(
                 DirectoryInfo rootDir, DirectoryInfo testDir, 
                 IEnumerable<FileInfo> fileInfos  ,IEnumerable<string> patterns) {
            
            Contract.Requires<ArgumentException>(rootDir == null || rootDir.Exists);
            Contract.Requires<ArgumentException>(testDir == null || testDir.Exists);
            fileInfos = fileInfos ?? new List<FileInfo>();
            patterns = patterns ?? new List<string>();

            var insertList = new List<FileInfo>();
            
            
            // パス指定時は拡張子検索を行わない
            if(!patterns.Any() && !fileInfos.Any() && rootDir!=null){
                insertList.AddRange(rootDir.GetFiles("*.c", SearchOption.AllDirectories));
                insertList.AddRange(rootDir.GetFiles("*.cpp", SearchOption.AllDirectories));
                insertList.AddRange(rootDir.GetFiles("*.cxx", SearchOption.AllDirectories));
            } else if (patterns.Any() && rootDir != null) {
                foreach (var pattern in patterns) {
                    insertList.AddRange(rootDir.GetFiles(pattern, SearchOption.AllDirectories));
                }

                //パターン合致によるバックアップファイルの排除
                var backUpFiles = new List<FileInfo>();
                backUpFiles.AddRange(rootDir.GetFiles("*" + OccfNames.LineBackUpSuffix, SearchOption.AllDirectories));
                backUpFiles.AddRange(rootDir.GetFiles("*" + OccfNames.KleeBackUpSuffix, SearchOption.AllDirectories));
                backUpFiles.AddRange(rootDir.GetFiles("*" + OccfNames.BackupSuffix, SearchOption.AllDirectories));

                for (var i = insertList.Count - 1; i >= 0; i--) {
                    foreach (var backUpFile in backUpFiles) {
                        if (insertList[i].FullName == backUpFile.FullName) {
                            insertList.Remove(insertList[i]);
                            break;
                        }
                    }
                }
            }

            //テストディレクトリ以下を排除
            if(testDir != null) {
                //注意:　i>0⇒i>=0　に変更　要確認
                for (var i = insertList.Count - 1; i >= 0; i--) {
                    if (insertList[i].FullName.StartsWith(testDir.FullName)) {
                        insertList.Remove(insertList[i]);
                    }
                }
            }

            //ファイル指定の追加
            var filePlus = insertList.Concat(fileInfos).ToList();

            //重複ファイルの削除
            for (var i = filePlus.Count - 1; i >= 0; i--) {
                for (var j = i - 1; j >= 0; j--) {
                    if (filePlus[i].FullName == filePlus[j].FullName) {
                        filePlus.Remove(filePlus[i]);
                        break;
                    }
                }
            }
            
            insertList = filePlus;
            
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

            File.Copy(defaultFileFullName, backUpFileFullName, true);

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

            Console.WriteLine("wrote:" + fileInfo.FullName);
        }

    }
}
=======
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
>>>>>>> 3587ca0c216605ba8970ea2d1825980021828d64
