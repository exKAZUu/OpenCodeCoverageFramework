using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OccfLineInsert
{
    class LineAnalysis
    {
        //ディレクトリ内のファイルを提示して選択されたコードの行数の元の行数を表示
        public void OutPutTrueLineNumber(string dirPath)
        {
            var dirInfo = new DirectoryInfo(dirPath);
            var files = Directory.GetFiles(dirPath);

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

            //Mapの受け取り
            new Dictionary<int, int>();
            //var lineDictionary = CreateLineMap(dirPath + @"/" + searchedFileName);
            var lineDictionary = LineSymmetyCreater(new FileInfo(dirPath + @"/" + searchedFileName));

            var trueLineNum = lineDictionary[seachedLineNum];
            
            Console.WriteLine(searchedFileName + "の " + seachedLineNum + " 行目は");
            Console.WriteLine("元のファイルでの行数は:　" + trueLineNum + " 行目");

        }

        //指定されたファイルの現行数と元行数のマップを作成
        public Dictionary<int, int> CreateLineMap(string fileFullName)
        {
            var lineDictionary = new Dictionary<int, int>();
            var fileInfo = new FileInfo(fileFullName);
            var lineApender = @"""" + fileInfo.Name + @"""";
            var apendNum = lineApender.Length;
            var truelineNum = 0;
            var fileNameLength = fileInfo.Name.Length;

            using (var reader = new StreamReader(fileFullName))
            {
                var lineNum = 1;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length >= (4 + apendNum))
                    {
                        var lastSentence = line.Substring(line.Length - apendNum, apendNum);
                        if (lastSentence.Equals(lineApender))
                        {
                            var digitNum = line.Length - fileNameLength - 5;
                            truelineNum = int.Parse(line.Substring(2, digitNum));
                        }
                    }
                    lineDictionary.Add(lineNum, truelineNum);
                    lineNum++;
                }
                reader.Close();
            }

            return lineDictionary;
        }
        

        public void CreateLineDic(DirectoryInfo rootDir, DirectoryInfo testDir) {
            // rootとtestを受け取ってroot以下でtest以外の　.c、.cxx、.cppのファイルに対して作成
            
            var fileList = new List<FileInfo>();
            fileList.AddRange(rootDir.GetFiles("*.c", SearchOption.AllDirectories));
            fileList.AddRange(rootDir.GetFiles("*.cpp", SearchOption.AllDirectories));
            fileList.AddRange(rootDir.GetFiles("*.cxx", SearchOption.AllDirectories));

            if(testDir !=null && testDir.Exists) {
                for(var i=fileList.Count-1; i>0; i--) {
                    if (fileList[i].FullName.StartsWith(testDir.FullName)) {
                        fileList.Remove(fileList[i]);
                    }
                }
            }

            var dic = fileList.ToDictionary(fileInfo => fileInfo, LineSymmetyCreater);

            foreach (var dics in dic) {
                Console.WriteLine(dics.Key.FullName + " , " + dics.Value.Last().Key + " , " + dics.Value.Last().Value);
            }
        }

        public Dictionary<int, int> LineSymmetyCreater(FileInfo fileInfo) {

            var lineDic = new Dictionary<int, int>();
            var fileName = fileInfo.Name;
            var lineAppender = @"""" + fileName + @"""";
            var apdLength = lineAppender.Length;
            const string header = @"# ";
            const string divider = @" ";
            var leastLength = header.Length + divider.Length + apdLength;

            var trueLineNum = 0;

            using (var reader = new StreamReader(fileInfo.FullName)) {
                var nowLineNum = 1;
                string line;
                while ((line = reader.ReadLine()) != null) {
                    if(line.Length > leastLength) {
                        var lastSentence = line.Substring(line.Length - apdLength, apdLength); 
                        if(lastSentence.Equals(lineAppender)) {
                            var digitNum = line.Length - leastLength;
                            trueLineNum = int.Parse(line.Substring(header.Length, digitNum));
                        }
                    }
                    lineDic.Add(nowLineNum, trueLineNum);
                    nowLineNum++;
                }
                reader.Close();
            }

            return lineDic;
        }

        /*
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
                       var lastSentence = line.Substring(line.Length - 2 - fileNameLength, fileNameLength + 2);
                       if (lastSentence.Equals(@"""" + searchedFileName + @""""))
                       {
                           var digitNum = line.Length - fileNameLength - 5;
                           truelineNum = int.Parse(line.Substring(2, digitNum));
                       }
                   }
                   if (lineNum >= searchedLineNum)
                   {
                       break;
                   }
                   lineNum++;
               }
               reader.Close();
           }
           return truelineNum;
       }*/
    }
}
