using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OccfLineInsert
{
    class LineInsert
    {
        //引数に入力ファイルのディレクトリのパス.c、.cpp、.cxxのみにフィルタリングして挿入
        public void LineInserts(string dirPath)
        {
            var dirInfo = new DirectoryInfo(dirPath);
            foreach (var defaultFileFullName in from fileInfo in dirInfo.GetFiles()
                                                let defaultFileFullName = fileInfo.FullName
                                                let fileName = fileInfo.Name
                                                let fileNameLength = fileName.Length
                                                let end2 = fileName.Substring(fileNameLength - 2, 2)
                                                let end4 = fileName.Substring(fileNameLength - 4, 4)
                                                where end2.Equals(@".c") || end4.Equals(@".cpp") || end4.Equals(@".cxx")
                                                select defaultFileFullName)
            {
                WriteInsetCode(defaultFileFullName);
            }
        }

        //指定されたファイルのパスを受け取って、指定名のバックアップファイルを作成して挿入
        public void WriteInsetCode(string defaultFileFullName)
        {
            var fileInfo = new FileInfo(defaultFileFullName);
            const string appendExtension = @".back";
            var backUpFileFullName = defaultFileFullName + appendExtension;
            var backUpFileName = fileInfo.Name + appendExtension;

            File.Copy(defaultFileFullName, backUpFileFullName);

            using (var reader = new StreamReader(backUpFileFullName))
            {
                using (var writer = new StreamWriter(fileInfo.FullName, false))
                {
                    string line;
                    var lineNum = 1;
                    while ((line = reader.ReadLine()) != null)
                    {
                        writer.WriteLine("#line " + lineNum);
                        writer.WriteLine(line);
                        lineNum++;
                    }
                    reader.Close();
                    writer.Close();
                }
            }
            //暫定的にバックアップファイルを消す場合は有効にする
            //File.Delete(backUpFileFullName);
        }

        //return検索 移動予定：別プロジェクトへ
        /*
        public bool SerchReturn(string line)
        {
            if (line.Length < 7)
            {
                return false;
            }

            for (int i = 6; i < line.Length; i++)
            {
                if (line.Substring(i - 6, 7).Equals("return "))
                {
                    return true;
                }
            }
            return false;
        }
         */

        //ディレクトリ内のファイルを提示して選択されたコードの行数の元の行数を表示
        /*
        public void OutPutTrueLineNumber(string dirPath)
        {
            var dirInfo = new DirectoryInfo(dirPath);
            var files = Directory.GetFiles(dirPath);
            //Console.WriteLine(dirPath);
            var i = 1;
            foreach (var file in files)
            {
                Console.WriteLine(i + "." + file.Substring(dirPath.Length + 1));
                i++;
            }

            Console.WriteLine("ファイル名を入力");
            var searchedFileName = Console.ReadLine();

            Console.WriteLine("行数を入力");
            var seachedLineNum = int.Parse(s: Console.ReadLine());

            var trueLineNum = SeachTrueLineNumber(dirPath, searchedFileName, seachedLineNum);
            Console.WriteLine(searchedFileName + "の " + seachedLineNum + " 行目は");
            Console.WriteLine("元のファイルでの行数は:　" + trueLineNum + " 行目");

        }

        //指定されたファイル・行数から元の行数を返す
        public int SeachTrueLineNumber(string dirPath, string searchedFileName, int searchedLineNum)
        {
            var truelineNum = 0;
            var fileNameLength = searchedFileName.Length;
            using (var reader = new StreamReader(dirPath + "/" + searchedFileName))
            {
                var lineNum = 1;
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line != null && line.Length >= 6 + fileNameLength)
                    {
                        //Console.WriteLine( lineNum + ","+ line +" , "+ line.Length +" , " + (line.Length - 2 - FileNameLength) + " , " + (FileNameLength+2));
                        var lastSentence = line.Substring(line.Length - 2 - fileNameLength, fileNameLength + 2);
                        if (lastSentence.Equals(@"""" + searchedFileName + @""""))
                        {
                            //truelineNum = int.Parse(line.Substring(2,));
                            var digitNum = line.Length - fileNameLength - 5;
                            truelineNum = int.Parse(line.Substring(2, digitNum));
                        }
                    }
                    //Console.WriteLine(lineNum +" ," + SearchedLineNum);
                    if (lineNum >= searchedLineNum)
                    {
                        break;
                    }
                    lineNum++;
                }
                reader.Close();
            }
            return truelineNum;
        }
         */

        //引数に入力ファイルのパス、出力はString型の出力コード 未使用
        /*
        public string LineInsetr2(string dirPath)
        {
            var insertedCode = "";
            var dirInfo = new DirectoryInfo(dirPath);
            foreach (var fileInfo in dirInfo.GetFiles())
            {
                using (var reader = new StreamReader(fileInfo.FullName))
                {
                    string line;
                    var lineNum = 1;
                    while ((line = reader.ReadLine()) != null)
                    {
                        insertedCode += "#line " + lineNum + "\n" + line + "\n";
                        lineNum++;
                    }
                }
            }
            return insertedCode;
        }
        */
    }
}
