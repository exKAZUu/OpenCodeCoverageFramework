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
            foreach (var fileInfo in dirInfo.GetFiles())
            {
                var defaultFileFullName = fileInfo.FullName;
                var fileName = fileInfo.Name;
                var nameLength = fileName.Length;

                if (nameLength >= 4)
                {
                    var end2 = fileName.Substring(nameLength - 2, 2);
                    var end4 = fileName.Substring(nameLength - 4, 4);

                    if (end2.Equals(@".c") || end4.Equals(@".cpp") || end4.Equals(@".cxx"))
                    {
                        WriteInsetLine(defaultFileFullName);
                    }
                }
                else if (nameLength >= 2)
                {
                    var end2 = fileName.Substring(nameLength - 2, 2);

                    if (end2.Equals(@".c"))
                    {
                        WriteInsetLine(defaultFileFullName);
                    }
                }

            }
        }

        //指定されたファイルのパスを受け取って、指定名のバックアップファイルを作成して挿入
        public void WriteInsetLine(string defaultFileFullName)
        {
            var fileInfo = new FileInfo(defaultFileFullName);
            const string appendExtension = @".back";
            var backUpFileFullName = defaultFileFullName + appendExtension;

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
        }

    }
}
