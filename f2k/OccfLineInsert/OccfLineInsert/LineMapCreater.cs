using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OccfLineInsert
{
    class LineMapCreater
    {
        //引数にDirPathとTestPathをとってDirInfoに変換して呼び出し
        public void LineMapper(string rootPath, string testPath)
        {
            var rootDir = new DirectoryInfo(rootPath);
            var testDir = new DirectoryInfo(testPath);



        }

        //DirInfoを入力として対象ファイルを選択　対象は.c, .cpp, .cxx 
        public void MapFileCreater(DirectoryInfo rootDir, DirectoryInfo testDir)
        {
            const string MappingFileName = ".occf_map_file";
            var mappingFileFullName = rootDir.FullName + "/" + MappingFileName; 

            if (File.Exists(mappingFileFullName))
            {
                Console.WriteLine("deleat exiting \"" + MappingFileName + "\" and create it newly");
                File.Delete(mappingFileFullName);
            }
                                              
            var mapFileList = new List<FileInfo>();

            mapFileList.AddRange(rootDir.GetFiles("*.c", SearchOption.AllDirectories));
            mapFileList.AddRange(rootDir.GetFiles("*.cpp", SearchOption.AllDirectories));
            mapFileList.AddRange(rootDir.GetFiles("*.cxx", SearchOption.AllDirectories));

            if (testDir != null)
            {
                for (var i = mapFileList.Count - 1; i > 0; i--)
                {
                    if (mapFileList[i].FullName.StartsWith(testDir.FullName))
                    {
                        mapFileList.Remove(mapFileList[i]);
                    }
                }
            }

            foreach (var fileInfo in mapFileList)
            {
                MapFileWriter(fileInfo, rootDir);
            }
        }

        public void MapFileWriter(FileInfo readedFile, DirectoryInfo rootDir)
        {
            var mappingFileFullname = rootDir.FullName + "/.occf_map_file";
            const string header = @"# ";
            const string divider = @" ";
            var trueLineNum = 0;

            using (var reader = new StreamReader(readedFile.FullName)) {
                using (var writer = new StreamWriter(mappingFileFullname, true)) {

                    var line = reader.ReadLine();
                    var lineAppender = line.Substring(header.Length + divider.Length + 1);
                    var apdLength = lineAppender.Length;
                    var leastLength = header.Length + divider.Length + apdLength;

                    writer.WriteLine(readedFile.FullName);
                    writer.WriteLine(1);
                    writer.WriteLine(1);


                    var nowLineNum = 2;

                    while ((line = reader.ReadLine()) != null) {
                        if (line.Length > leastLength) {
                            var lastSentence = line.Substring(line.Length - apdLength, apdLength);
                            if (lastSentence.Equals(lineAppender)) {
                                var digitNum = line.Length - leastLength;
                                trueLineNum = int.Parse(line.Substring(header.Length, digitNum));
                            }
                        }
                        writer.WriteLine(readedFile.FullName);
                        writer.WriteLine(nowLineNum);
                        writer.WriteLine(trueLineNum);
                        nowLineNum++;
                    }

                    writer.Close();
                    reader.Close();
                }
            }
        }


        public void LineMapKakunin(FileInfo mappingFile, DirectoryInfo rootdir)
        {
            var mapDic = mapDicCreater(mappingFile);
            var kakuninFileFullname = rootdir.FullName + "/.kakuninfile"; 
            using (var writer = new StreamWriter(kakuninFileFullname, false))
            {
                for (var i = 0; i < mapDic.Values.Count; i++)
                {
                    for (var j = 0; j < mapDic.Values.ElementAt(i).Count(); j++ )
                    {
                        writer.WriteLine(mapDic.ElementAt(i).Key.FullName);
                        writer.WriteLine(mapDic.Values.ElementAt(i).ElementAt(j).Key);
                        writer.WriteLine(mapDic.Values.ElementAt(i).ElementAt(j).Value);
                    }
                }
                writer.Close();
            }
            var kakunin = new FileInfo(kakuninFileFullname);

            //Console.WriteLine(mappingFile.Equals(kakunin));
            DirectoryInfo testDir = new DirectoryInfo(rootdir.FullName + "/test");
            var kyuu = LineDicCreater(rootdir, testDir);
            Console.WriteLine("f1: " + kyuu.Keys.ElementAt(0).FullName);
            Console.WriteLine("ct1: " + kyuu.Values.ElementAt(0).Count + " , " + kyuu.Values.ElementAt(0).Keys.ElementAt(0));
            Console.WriteLine("f2: " + mapDic.Keys.ElementAt(0).FullName);
            Console.WriteLine("ct2: " + mapDic.Values.ElementAt(0).Count + " , " + mapDic.Values.ElementAt(0).Keys.ElementAt(0));

            Console.WriteLine("f1: " + kyuu.Keys.ElementAt(1).FullName);
            Console.WriteLine("ct1: " + kyuu.Values.ElementAt(1).Count + " , " + kyuu.Values.ElementAt(0).Keys.ElementAt(0));
            Console.WriteLine("f2: " + mapDic.Keys.ElementAt(1).FullName);
            Console.WriteLine("ct2: " + mapDic.Values.ElementAt(1).Count + " , " + mapDic.Values.ElementAt(0).Keys.ElementAt(0));

            Console.WriteLine(mapDic.Equals(kyuu));
            var fl = true;
            for (var i = 0; i < kyuu.Count; i++)
            {
                for (var j = 0; j < kyuu.Values.ElementAt(i).Count; j++)
                {
                    var kyuu1 =  kyuu.Values.ElementAt(i).Keys.ElementAt(j);
                    var kyuu2 = kyuu.Values.ElementAt(i).Values.ElementAt(j);
                    var map1 = mapDic.Values.ElementAt(i).Keys.ElementAt(j);
                    var map2 = mapDic.Values.ElementAt(i).Values.ElementAt(j);

                    if (kyuu1 != map1 || kyuu2 != map2)
                    {
                        Console.WriteLine("k1:"+ kyuu1 + ", k2:"+ kyuu2 + ", m1:"+ map1 + ", m2:" + map2);
                        fl = false;
                    }
                }
            }
            Console.WriteLine("fl: "+ fl);
            
            Console.WriteLine(file_compare(mappingFile.FullName, kakunin.FullName));
            Console.WriteLine(file_compare2(mappingFile, kakunin));
        }

        public Dictionary<FileInfo, Dictionary<int, int>> mapDicCreater(FileInfo mappingFile)
        {
            var mapDic = new Dictionary<FileInfo, Dictionary<int, int>>();

            using (var reader = new StreamReader(mappingFile.FullName)) {
                var lineDic = new Dictionary<int, int>();
                var lastFileInfo = new FileInfo(reader.ReadLine());
                var nowLine = int.Parse(reader.ReadLine());
                var trueLine = int.Parse(reader.ReadLine());
                lineDic.Add(nowLine, trueLine);
                
                string line;
                while ((line = reader.ReadLine()) != null) {
                    var fileInfo = new FileInfo(line);
                    nowLine = int.Parse(reader.ReadLine());
                    trueLine = int.Parse(reader.ReadLine());
                    
                    if (!(fileInfo.FullName.Equals(lastFileInfo.FullName))) {
                        mapDic.Add(lastFileInfo, new Dictionary<int, int>(lineDic));
                        lineDic.Clear();
                        lastFileInfo = fileInfo;
                    }

                    lineDic.Add(nowLine, trueLine);                   
                }
                mapDic.Add(lastFileInfo, lineDic);
                reader.Close();
            }

            return mapDic;
        }

        int file_compare2(FileInfo f1, FileInfo f2)
        {
            int retval = 0;
            using (var reader1 = new StreamReader(f1.FullName)) {
                using (var reader2 = new StreamReader(f2.FullName))
                {
                    string line1;
                    string line2;
                    int line = 1;
                    while ((line1 = reader1.ReadLine()) != null)
                    {
                        line2 = reader2.ReadLine();
                        if (!(line1.Equals(line2)))
                        {
                            retval = line;
                            break;
                        }
                        line++;
                    }

                    reader1.Close();
                    reader2.Close();
                }
            }
            
            return retval;

        }

        int file_compare(string file_name1, string file_name2)
        {
            FileStream reader1 = new FileStream(file_name1,
                                                FileMode.Open,
                                                FileAccess.Read);

            FileStream reader2 = new FileStream(file_name2,
                                                FileMode.Open,
                                                FileAccess.Read);

            int X1, X2;
            int ret_val = 1;
            int line = 1;
            while (true)
            {
                X1 = reader1.ReadByte();
                X2 = reader2.ReadByte();
                
                if (X1 != X2) {
                    ret_val = line;
                    break; 
                }    // 比較
                
                if (X1 == -1) { 
                    ret_val = 0; break; 
                }
                
                line++;
            }
            reader1.Close();
            reader2.Close();

            return ret_val;
        }

        private Dictionary<FileInfo, Dictionary<int, int>>
            LineDicCreater(DirectoryInfo rootDirInfo, DirectoryInfo testDirTnfo)
        {

            var fileList = new List<FileInfo>();
            fileList.AddRange(rootDirInfo.GetFiles("*.c", SearchOption.AllDirectories));
            fileList.AddRange(rootDirInfo.GetFiles("*.cpp", SearchOption.AllDirectories));
            fileList.AddRange(rootDirInfo.GetFiles("*.cxx", SearchOption.AllDirectories));

            if (testDirTnfo != null && testDirTnfo.Exists)
            {
                for (var i = fileList.Count - 1; i > 0; i--)
                {
                    if (fileList[i].FullName.StartsWith(testDirTnfo.FullName))
                    {
                        fileList.Remove(fileList[i]);
                    }
                }
            }

            var lineDic = fileList.ToDictionary(fileInfo => fileInfo, LineSymmetyCreater);

            return lineDic;
        }

        private Dictionary<int, int> LineSymmetyCreater(FileInfo fileInfo)
        {
            var lineDic = new Dictionary<int, int>();
            var fileName = fileInfo.Name;
            //var lineAppender = @"""" + fileName + @"""";
            //var apdLength = lineAppender.Length;
            const string header = @"# ";
            const string divider = @" ";
            //var leastLength = header.Length + divider.Length + apdLength;
            var trueLineNum = 0;

            using (var reader = new StreamReader(fileInfo.FullName))
            {

                var line = reader.ReadLine();
                var lineAppender = line.Substring(4);
                var apdLength = lineAppender.Length;
                var leastLength = header.Length + divider.Length + apdLength;
                lineDic.Add(1, 1);
                var nowLineNum = 2;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length > leastLength)
                    {
                        var lastSentence = line.Substring(line.Length - apdLength, apdLength);
                        if (lastSentence.Equals(lineAppender))
                        {
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

    }
}
