using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Occf.Core.Utils;

namespace Occf.Tools.Cui
{
    class LineMapCreater
    {
        private const string S = "  ";
        private const int W = 20;

        private static readonly string Usage =
            Program.Header +
            "" + "\n" +
            "Usage: Occf line_map <root> [<test>]" + "\n" +
            "" + "\n" +
            S + "<root>".PadRight(W)
            + "path of root directory" + "\n" +
            S + "<test>".PadRight(W) + "path of test code directory" + "\n" +
            "";

        public static bool Run(IList<string> args)
        {

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

            MapFileCreater(rootDir, testDir);

            return true;
        }

        //DirInfoを入力として対象ファイルを選択　対象は.c, .cpp, .cxx 
        public static void MapFileCreater(DirectoryInfo rootDir, DirectoryInfo testDir) {
            
            var mappingFileFullName = rootDir.FullName + "/" + OccfNames.LineMapping;

            if (File.Exists(mappingFileFullName)) {
                Console.WriteLine("deleat exiting \"" + OccfNames.LineMapping + "\" and create it newly");
                File.Delete(mappingFileFullName);
            }

            var mapFileList = new List<FileInfo>();

            mapFileList.AddRange(rootDir.GetFiles("*.c", SearchOption.AllDirectories));
            mapFileList.AddRange(rootDir.GetFiles("*.cpp", SearchOption.AllDirectories));
            mapFileList.AddRange(rootDir.GetFiles("*.cxx", SearchOption.AllDirectories));

            if (testDir != null) {
                //注意：i>0　⇒　i>=0に変更
                for (var i = mapFileList.Count - 1; i >= 0; i--) {
                    if (mapFileList[i].FullName.StartsWith(testDir.FullName)) {
                        mapFileList.Remove(mapFileList[i]);
                    }
                }
            }

            foreach (var fileInfo in mapFileList) {
                MapFileWriter(fileInfo, rootDir);
            }
        }

        public static void MapFileWriter(FileInfo readedFile, DirectoryInfo rootDir)
        {
            var mappingFileFullname = rootDir.FullName + "/" + OccfNames.LineMapping;
            const string header = @"# ";
            const string divider = @" ";
            //const string occfLineMarker = "# 1 OccfLineMarker";
            var trueLineNum = 0;

            using (var reader = new StreamReader(readedFile.FullName)) {
                using (var writer = new StreamWriter(mappingFileFullname, true)) {

                    var line = reader.ReadLine();
                    var lineAppender = line.Substring(header.Length + divider.Length + 1);
                    var apdLength = lineAppender.Length;
                    var leastLength = header.Length + divider.Length + apdLength;

                    writer.WriteLine(readedFile.FullName);
                    writer.WriteLine(1);
                    writer.WriteLine(0);
                    
                    var nowLineNum = 2;
                    while ((line = reader.ReadLine()) != null) {
                        if (line.Length > leastLength) {
                            var lastSentence = line.Substring(line.Length - apdLength, apdLength);
                            if (lastSentence.Equals(lineAppender)) {
                                var digitNum = line.Length - leastLength;
                                trueLineNum = int.Parse(line.Substring(header.Length, digitNum));
                            }
                        }
                        //if (line == occfLineMarker){
                            trueLineNum = 0;
                        //}
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
    }
}
