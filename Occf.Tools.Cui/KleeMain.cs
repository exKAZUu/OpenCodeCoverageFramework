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
using System.IO;
using System.Linq;
using NDesk.Options;
using Occf.Core.Utils;

namespace Occf.Tools.Cui {
	internal class KleeMain {
		private const string S = "  ";
		private const int W = 20;

		private static readonly string Usage =
				Program.Header +
						"" + "\n" +
						"Usage: Occf klee_main -r <root_dir> <main_file_path> [<main_file_path>]" + "\n" +
						"" + "\n" +
						S + "-r, -root <root>".PadRight(W)
						+ "path of root directory (including source and test code)" + "\n" +
                        S + "-l, -lib <lib>".PadRight(W)
                        + "insert include \"stdlib\" and \"stdio\" " + "\n" +
                        S + "".PadRight(W) + " <lib> : \"lib\" is stdlib, \"io\" is stdio, \"all\" is both." + "\n" +
                        S + "<main_file_path>".PadRight(W)
						+ "path of main file of execute klee tests" + "\n" +
						"";

		public static bool Run(IList<string> args) {
			var mainFilePaths = new List<string>();
			var rootDirPath = "";
		    var libtype = "";

			// parse options
			var p = new OptionSet {
					{ "r|root=", v => rootDirPath = v },
                    { "l|lib=", v => libtype = v },
			};

			// コマンドをパース "-"指定されないのだけargs[]に残る
			try {
				args = p.Parse(args);
			} catch {
				Console.WriteLine("catch");
				return Program.Print(Usage);
			}

			mainFilePaths = args.ToList();

			if (args.Count < 1) {
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
			
			var fileInfos = new List<FileInfo>();
			foreach (var path in mainFilePaths) {
				if (!string.IsNullOrEmpty(path)) {
					var fileInfo = new FileInfo(path);
					if (!fileInfo.Exists) {
						return
								Program.Print(
										"Error: file path doesn't exist.\nfile_path:" + fileInfo.FullName);
					}
					fileInfos.Add(fileInfo);
				}
			}

            if (!string.IsNullOrEmpty(libtype)) {
                if (libtype != "lib" && libtype != "io" && libtype != "all") {
                    return
                            Program.Print(Usage);
                }
            }

			MainFileInsertReader(fileInfos, rootDir, libtype);
			//InsertToMainFail(mainFile, rootDir);

			return true;
		}

		private static void MainFileInsertReader(IEnumerable<FileInfo> fileInfos, 
                                                    DirectoryInfo rootDir, string libType) {
			foreach (var info in fileInfos) {
				InsertToMainFail(info, rootDir, libType);
			}
		}

		private static void InsertToMainFail(FileInfo defFile, DirectoryInfo rootDir, string libType) {
			
			var defFileFullName = defFile.FullName;
			var backUpFileFullName = defFileFullName + OccfNames.KleeBackUpSuffix;
			var backUpFileName = defFile.Name + OccfNames.KleeBackUpSuffix;

			var insertedMethod = false;
		    var insertedAtExit = false;

			//先にバックアップファイルが存在していた場合は削除
			if (File.Exists(backUpFileFullName)) {
				Console.WriteLine("delete existing \"" + backUpFileName + "\" and create it newly");
				File.Delete(backUpFileFullName);
			}
            //バックアップ兼データ読み込み用ファイルの作成
			File.Copy(defFileFullName, backUpFileFullName);

			using (var reader = new StreamReader(backUpFileFullName)) {
				using (var writer = new StreamWriter(defFile.FullName, false)) {
					string line;
					var lineNum = 1;
					var mainFlag = false;
					var bracesCount = 0;
					var rootFullName = rootDir.FullName.Replace("\\", "/");

				    const string lineIndent = "#line 0";
                    const string atexitMarker = "klee_make_symbolic";

                    //include の埋め込み
				    WriteInclude(writer,libType);

					while ((line = reader.ReadLine()) != null) {
						var lineLength = line.Length;
						
						//main 判定　"main" メソッドの抽出と{}による終了判定
						var mainIndex1 = line.IndexOf(" main ", StringComparison.Ordinal);
						var mainIndex2 = line.IndexOf(" main(", StringComparison.Ordinal);

						if (!(mainFlag) && (mainIndex1 >= 0 || mainIndex2 >= 0)) {
							var mainIndex = mainIndex1 > mainIndex2 ? mainIndex1 : mainIndex2;
							for (var i = mainIndex + 5; i < lineLength; i++) {
								var sent1 = line.Substring(i, 1);

								if (sent1.Equals(@"(")) {
									mainFlag = true;
                                    // main直前 OccfExit()の挿入
                                    WriteOccfExit(writer, lineIndent, rootFullName);
								    insertedMethod = true;
									bracesCount = 0;
									break;
								}
								if (sent1.Equals(@" ")) {
									break;
								}
							}
						}

						//main終端判定
						if (mainFlag && lineLength >= 1) {
							for (var i = 0; i < lineLength; i++) {
								var sent1 = line.Substring(i, 1);

								if (sent1.Equals("{")) {
									bracesCount++;
								} else if (sent1.Equals("}")) {
									bracesCount--;
									if (bracesCount <= 0) {
										mainFlag = false;
									}
								}
							}
						}

                        // klee_make_symbolic判定mainflagがONでかつklee_makeを検出時
                        if (mainFlag && line.Contains(atexitMarker)) {
                            var spaceNum = line.IndexOfAny(atexitMarker.ToCharArray());
                            //atexit()の挿入
                            WriteAtExit(writer, lineIndent, spaceNum);
                            insertedAtExit = true;
                        }

						writer.WriteLine(line);
						lineNum++;
					}
					reader.Close();
					writer.Close();
				}
			}

			//上位のバックアップファイル(Line ins)がある場合はバックアップファイルを残さない
			if(File.Exists(defFileFullName+OccfNames.LineBackUpSuffix)) {
				File.Delete(backUpFileFullName);
			}

			if (insertedAtExit) { // exit trueじゃないとmethod trueにならないので片方でOK
				Console.WriteLine("wrote:" + defFile.FullName);
			} else if(insertedMethod){
                Console.WriteLine("failed:" + defFile.FullName + " is only wrote OCCF Method");
			} else {
			    Console.WriteLine("unwrote:" + defFile.FullName);
			}
		}

        private static void WriteInclude(StreamWriter writer, string libType) {
            switch (libType){
                case "lib":
                    writer.WriteLine("#include <stdlib.h>");
                    break;
                case "io":
                    writer.WriteLine("#include <stdio.h>");
                    break;
                case "all":
                    writer.WriteLine("#include <stdlib.h>");
                    writer.WriteLine("#include <stdio.h>");
                    break;
            }
        }

        private static void WriteAtExit(StreamWriter writer,string lineIndent, int spaceNum) {
            const string atexit = "atexit(occf_exit);";
            writer.WriteLine("");
            writer.WriteLine(lineIndent);
            writer.WriteLine(atexit.PadLeft(spaceNum + atexit.Length));
            writer.WriteLine(lineIndent);
            writer.WriteLine("");
        }

        private static void WriteOccfExit(StreamWriter writer, string lineIndent, string rootFullName) {
            writer.WriteLine("");
            writer.WriteLine(lineIndent);
            writer.WriteLine("void occf_exit(){");
            writer.WriteLine(lineIndent);
            writer.WriteLine("\t char *occftmp = getenv(\"KTEST_FILE\");");
            writer.WriteLine(lineIndent);
            writer.WriteLine("\t FILE *occffile = fopen(\"" + rootFullName + "/" + OccfNames.SuccessfulTests + "\", \"a\");");
            writer.WriteLine(lineIndent);
            writer.WriteLine("\t fputs(occftmp, occffile);");
            writer.WriteLine(lineIndent);
            writer.WriteLine("\t fputc('\\n', occffile);");
            writer.WriteLine(lineIndent);
            writer.WriteLine("\t fclose(occffile);");
            writer.WriteLine(lineIndent);
            writer.WriteLine("}");
            writer.WriteLine(lineIndent);
            writer.WriteLine("");
        }
	}
}
