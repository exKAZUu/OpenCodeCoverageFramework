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
            var insertList = new List<FileInfo>();
            
            insertList.AddRange(dirInfo.GetFiles("*.c", SearchOption.AllDirectories));
            insertList.AddRange(dirInfo.GetFiles("*.cpp", SearchOption.AllDirectories));
            insertList.AddRange(dirInfo.GetFiles("*.cxx", SearchOption.AllDirectories));

            /*
            DirectoryInfo test = null;

            if (test != null)
            {
                for(var i=insertList.Count-1; i>0; i--) {
                    if(insertList[i].FullName.StartsWith(test.FullName)) {
                        insertList.Remove(insertList[i]);
                    }
                }
            }
            */
            

            foreach (var fileInfo in insertList) {
                WriteInsetLine(fileInfo.FullName);
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
