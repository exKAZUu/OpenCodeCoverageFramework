using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NDesk.Options;
using Occf.Core.Modes;
using Occf.Core.Utils;

namespace Occf.Tools.Cui
{
    class KleeMain {
        private const string S = "  ";
        private const int W = 20;

        private static readonly string Usage =
            Program.Header +
            "" + "\n" +
            "Usage: Occf klee_main <root> <main>" + "\n" +
            "" + "\n" +
            S + "<root>".PadRight(W)
            + "path of root directory (including source and test code)" + "\n" +
            S + "<main>".PadRight(W)
            + "path of main file of execute klee tests" + "\n" +
            "";

        public static bool Run(IList<string> args) {

            if (args.Count < 2) {
                return Program.Print(Usage);
            }

            var rootDir = new DirectoryInfo(args[0]);
            if (!rootDir.Exists) {
                return
                        Program.Print(
                                "Root directory doesn't exist.\nroot:" + rootDir.FullName);
            }

            var mainFile = new FileInfo(args[1]);
            if (!mainFile.Exists) {
                return
                        Program.Print(
                                "Error: main file of execute klee tests dosen't exit.\nmain:" + mainFile.FullName);
            }
            
            InsertToMainFail(mainFile, rootDir);

            return true;
        }

        private static void InsertToMainFail(FileInfo defFile, DirectoryInfo rootDir) {
            
            var defFileFullName = defFile.FullName;
            //const string appendExtension = @".occf_klee_back"; //KleeBackUpSuffix
            //const string lineAppendr = @".occf_line_back"; //lineBackUpsuffix
            var backUpFileFullName = defFileFullName + OccfNames.KleeBackUpSuffix;
            var backUpFileName = defFile.Name + OccfNames.KleeBackUpSuffix;

            //先にバックアップファイルが存在していた場合は削除
            if (File.Exists(backUpFileFullName)) {
                Console.WriteLine("delete existing \"" + backUpFileName + "\" and create it newly");
                File.Delete(backUpFileFullName);
            }

            File.Copy(defFileFullName, backUpFileFullName);

            using (var reader = new StreamReader(backUpFileFullName)) {
                using (var writer = new StreamWriter(defFile.FullName, false)) {
                    string line;
                    var lineNum = 1;
                    var mainFlag = false;
                    var bracesCount = 0;
                    var rootFullName = rootDir.FullName.Replace("\\", "/");

                    while ((line = reader.ReadLine()) != null) {
                        if (lineNum == 1) {
                            writer.WriteLine("#include <stdlib.h>");
                            writer.WriteLine("#include <stdio.h>");
                        }

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
                        //return判定:mainflagがONでかつリターンを検出時
                        if (mainFlag && lineLength >= 8) {
                            if (line.Substring(7).Equals("return ") || line.Contains(" return ")) {
                                writer.WriteLine("");
                                writer.WriteLine(@"char *occftmp = getenv(""KTEST_FILE"");");
                                writer.WriteLine(@"FILE *occffile = fopen(""" + rootFullName + @"/" + OccfNames.SuccessfulTests + @""", ""a"");");
                                writer.WriteLine(@"fputs(occftmp, occffile);");
                                writer.WriteLine(@"fputc('\n', occffile);");
                                writer.WriteLine(@"fclose(occffile);");
                                writer.WriteLine("");
                            }
                        }
                        writer.WriteLine(line);
                        lineNum++;
                    }
                    reader.Close();
                    writer.Close();
                }
            }

            //上位のバックアップファイルがある場合はバックアップファイルを残さない
            if(File.Exists(defFileFullName+OccfNames.LineBackUpSuffix)) {
                File.Delete(backUpFileFullName);
            }
        }
    }
}
