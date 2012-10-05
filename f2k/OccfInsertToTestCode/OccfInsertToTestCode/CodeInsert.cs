using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OccfInsertToTestCode
{
    class CodeInsert
    {
        //引数に入力ファイルのディレクトリのパス .c .cpp .cxxのみにフィルタリングして挿入
        public void CodeInserts(string dirPath)
        {
            var dirInfo = new DirectoryInfo(dirPath);
            foreach (var fileInfo in dirInfo.GetFiles())
            {
                var defaultFileFullName = fileInfo.FullName;
                var fileName = fileInfo.Name;
                var nameLength = fileName.Length;

                if (nameLength >= 4)
                {
                    var end2 = fileName.Substring(nameLength - 2, 2);
                    var end4 = fileName.Substring(nameLength - 4, 4);

                    if (end2.Equals(@".c") || end4.Equals(@".cpp")
                        || end4.Equals(@".cxx"))
                    {
                        WriteCodeInsert(defaultFileFullName);
                    }

                }
                else if (nameLength >= 2)
                {
                    var end2 = fileName.Substring(nameLength - 2, 2);

                    if (end2.Equals(@".c"))
                    {
                        WriteCodeInsert(defaultFileFullName);
                    }

                }
            }
        }

        //指定されたファイルのパスを受け取って、指定名のバックアップファイルを作成して挿入
        public void WriteCodeInsert(string defaultFileFullName)
        {
            var fileInfo = new FileInfo(defaultFileFullName);
            const string appendExtension = @".backt";
            var backUpFileFullName = defaultFileFullName + appendExtension;
            var backUpFileName = fileInfo.Name + appendExtension;

            //先にバックアップファイルが存在していた場合は削除
            if (File.Exists(backUpFileFullName))
            {
                Console.WriteLine("元の" + backUpFileName + "を削除して新しいファイルを作成します");
                File.Delete(backUpFileFullName);
            }

            File.Copy(defaultFileFullName, backUpFileFullName);

            using (var reader = new StreamReader(backUpFileFullName))
            {
                using (var writer = new StreamWriter(fileInfo.FullName, false))
                {
                    string line;
                    var lineNum = 1;
                    var mainFlag = false;
                    var bracesCount = 0;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if(lineNum == 1) {
                            writer.WriteLine("#include <stdlib.h>");
                            writer.WriteLine("#include <stdio.h>");
                        }

                        var lineLength = line.Length;

                        //main 判定　"main" メソッドの抽出と{}による終了判定
                        if(!(mainFlag) && lineLength >=6) {
                            for(var i=5; i<lineLength; i++) {
                                var sent6 = line.Substring(i - 5, 6);
                                if(sent6.Equals(@" main ") || sent6.Equals(@" main(")) {
                                    for (var j = i; j < lineLength; j++) {
                                        var sent1 = line.Substring(j, 1);
                                        if(sent1.Equals(@"(")) {
                                            mainFlag = true;
                                            bracesCount = 0;
                                            break;
                                        }
                                        else if(!(sent1.Equals(@" "))) {
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        //main終端判定
                        if (mainFlag && lineLength >= 1)
                        {
                            for(var i=0; i<lineLength; i++) {
                                var sent1 = line.Substring(i, 1);
                                if (sent1.Equals("{"))
                                {
                                    bracesCount++;
                                }
                                else if (sent1.Equals("}"))
                                {
                                    bracesCount--;
                                    if (bracesCount <= 0)
                                    {
                                        mainFlag = false;
                                    }
                                }
                            }
                        }

                        //return判定:mainflagがONでかつリターンを検出時
                        if(mainFlag && lineLength>=8) {
                            for(var i=8; i<lineLength; i++) {
                                if (line.Substring(7).Equals("return ") || line.Substring(i - 8, 8).Equals(" return ")) {
                                    writer.WriteLine("");
                                    writer.WriteLine(@"char *tmp = getenv(""KTEST_FILE"");");
                                    writer.WriteLine(@"FILE *file = fopen("".successful_test"", ""a"");");
                                    writer.WriteLine(@"fputs(tmp, file);");
                                    writer.WriteLine(@"fputc('\n', file);");
                                    writer.WriteLine(@"fclose(file);");
                                    writer.WriteLine("");
                                }
                            }
                        }

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
