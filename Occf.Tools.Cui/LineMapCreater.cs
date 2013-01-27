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
using System.Diagnostics.Contracts;
using System.Linq;
using System.IO;
using NDesk.Options;
using Occf.Core.Utils;

namespace Occf.Tools.Cui
{
    class LineMapCreater
    {
        private const string S = "  ";
        private const int W = 22;

        private static readonly string Usage =
            Program.Header +
            "" + "\n" +
            "Usage: Occf line_map -r <root_dir> [options] [<file_path>]" + "\n" +
            "" + "\n" +
            S + "-r, -root <root_dir>".PadRight(W)
            + "path of root directory and mapping file place." + "\n" +
            S + "-t, -test <test_dir>".PadRight(W) 
            + "path of test code directory." + "\n" +
            S + "-s, -srcdir <src_dir>".PadRight(W)
            + "root of source directory for project when you want to "
            + "\n" + S + "".PadRight(W) + "separate source directory from root directory." 
            + "\n" +
            S + "-p, -pattern <name>".PadRight(W)
            + "search pattern for exploring target source files "
            + "\n" + S + "".PadRight(W) + "in the root directory." 
            + "\n" +
            S + "<file_path>".PadRight(W)
            + "file path when you want to specify the file directly."
            + "\n" +
            "";

        public static bool Run(IList<string> args) {
            var rootDirPath = "";
            var srcDirPath = "";
            var testDirPath = "";
            var patterns = new List<string>();
            var filePaths = new List<string>();

            // parse options
            var p = new OptionSet {
					{ "r|root=", v => rootDirPath = v },
					{ "t|test=", v => testDirPath = v },
					{ "p|pattern=", patterns.Add },
                    { "s|srcdir=", v => srcDirPath = v },
			};

            // コマンドをパース "-"指定されないのだけargs[]に残る
            try {
                args = p.Parse(args);
            } catch {
                Console.WriteLine("catch");
                return Program.Print(Usage);
            }

            filePaths = args.ToList();

            if (string.IsNullOrEmpty(rootDirPath)) {
                return Program.Print(Usage);
            }

            var rootDir = new DirectoryInfo(rootDirPath);
            if (!rootDir.Exists) {
                return
                        Program.Print(
                                "Root directory doesn't exist.\nroot:" + rootDir.FullName);
            }

            if (string.IsNullOrEmpty(srcDirPath)) {
                srcDirPath = rootDirPath;
            }

            var srcDir = new DirectoryInfo(srcDirPath);
            if (!srcDir.Exists) {
                return
                        Program.Print(
                                "Source directory doesn't exist.\nsrc:" + srcDir.FullName);
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
                                    "Error: file path doesn't exist.\nfile path:" + fileInfo.FullName);
                }
                fileInfos.Add(fileInfo);
            }


            MapFileCreater(rootDir, srcDir, testDir, patterns, fileInfos);

            return true;
        }

        //DirInfoを入力として対象ファイルを選択　対象は.c, .cpp, .cxx 
        public static void MapFileCreater(
                DirectoryInfo rootDir, DirectoryInfo srcDir, DirectoryInfo testDir, 
                List<string> patterns, List<FileInfo> fileInfos ) {

            Contract.Requires<ArgumentException>(rootDir.Exists);
            Contract.Requires<ArgumentException>(srcDir.Exists);
            Contract.Requires<ArgumentException>(testDir == null || testDir.Exists);
            patterns = patterns ?? new List<string>();
            fileInfos = fileInfos ?? new List<FileInfo>();

            var mapFileList = new List<FileInfo>();
            var mappingFileFullName = rootDir.FullName + "/" + OccfNames.LineMapping;

            if (File.Exists(mappingFileFullName)) {
                Console.WriteLine("deleat exiting \"" + OccfNames.LineMapping + "\" and create it newly");
                File.Delete(mappingFileFullName);
            }

           //ファイルリストの生成
            if (!patterns.Any() && !fileInfos.Any()) {
                mapFileList.AddRange(srcDir.GetFiles("*.c", SearchOption.AllDirectories));
                mapFileList.AddRange(srcDir.GetFiles("*.cpp", SearchOption.AllDirectories));
                mapFileList.AddRange(srcDir.GetFiles("*.cxx", SearchOption.AllDirectories));                
            } else if (patterns.Any()) {
                foreach (var pattern in patterns) {
                    mapFileList.AddRange(srcDir.GetFiles(pattern, SearchOption.AllDirectories));
                }

                //パターン合致によるバックアップファイルの排除   
                var backUpFiles = new List<FileInfo>();
                backUpFiles.AddRange(srcDir.GetFiles("*" + OccfNames.LineBackUpSuffix, SearchOption.AllDirectories));
                backUpFiles.AddRange(srcDir.GetFiles("*" + OccfNames.KleeBackUpSuffix, SearchOption.AllDirectories));
                backUpFiles.AddRange(srcDir.GetFiles("*" + OccfNames.BackupSuffix, SearchOption.AllDirectories));

                for (var i = mapFileList.Count - 1; i >= 0; i--) {
                    if (backUpFiles.Any(backUpFile => mapFileList[i].FullName == backUpFile.FullName)) {
                        mapFileList.Remove(mapFileList[i]);
                    }
                }
            }
           
            //テストディレクトリ以下を排除
            if (testDir != null) {
                for (var i = mapFileList.Count - 1; i >= 0; i--) {
                    if (mapFileList[i].FullName.StartsWith(testDir.FullName)) {
                        mapFileList.Remove(mapFileList[i]);
                    }
                }
            }

            //ファイル指定の追加
            var filePlus = mapFileList.Concat(fileInfos).ToList();

            //重複ファイルの削除
            for (var i = filePlus.Count - 1; i >= 0; i--) {
                for (var j = i - 1; j >= 0; j--) {
                    if (filePlus[i].FullName == filePlus[j].FullName) {
                        filePlus.Remove(filePlus[i]);
                        break;
                    }
                }
            }
            
            mapFileList = filePlus;

            foreach (var fileInfo in mapFileList) {
                MapFileWriter(fileInfo, rootDir);
            }
        }

        public static void MapFileWriter(FileInfo readedFile, DirectoryInfo rootDir) {

            var mappingFileFullname = rootDir.FullName + "/" + OccfNames.LineMapping;
            const string header = @"# ";
            const string divider = @" ";

            var trueLineNum = 1;
            var lineDiff = 0;
            var sharpLine = false;
            var illegalSharpOne = false;

            using (var reader = new StreamReader(readedFile.FullName)) {
                using (var writer = new StreamWriter(mappingFileFullname, true)) {

                    var line = reader.ReadLine();
                    var lineAppender = line.Substring(header.Length + divider.Length + 1);
                    var apdLength = lineAppender.Length;
                    var leastLength = header.Length + divider.Length + apdLength;
                    
                    writer.WriteLine(readedFile.FullName);
                    writer.WriteLine(1 + "," + (-1));

                    const int iggLineNum = 3;

                    //最初の2～4行目は見ないようにする
                    for (var i = 0; i < iggLineNum; i++) {
                        line = reader.ReadLine();
                        if (line == null) {
                            reader.Close();
                            writer.Close();
                            return;
                        }
                    }
                    
                    var nowLineNum = iggLineNum + 2;
                    while ((line = reader.ReadLine()) != null) {
                        if (line.Length > leastLength) {
                            var lastSentence = line.Substring(line.Length - apdLength, apdLength);
                            if (lastSentence.Equals(lineAppender)) {
                                var digitNum = line.Length - leastLength;
                                trueLineNum = int.Parse(line.Substring(header.Length, digitNum));
                                sharpLine = true;
                                if (trueLineNum != 1) {
                                    illegalSharpOne = true;
                                }
                            }
                        }

                        if (sharpLine) {
                            var nowLineDiff = nowLineNum - trueLineNum - 1;
                            if (illegalSharpOne && trueLineNum == 1) {
                                writer.WriteLine(nowLineNum + "," + (-2));
                            } else if (lineDiff != nowLineDiff){
                                writer.WriteLine(nowLineNum + "," + nowLineDiff);
                                lineDiff = nowLineDiff;
                            }
                            
                        }

                        nowLineNum++;
                        sharpLine = false;
                    }

                    writer.Close();
                    reader.Close();
                }
            }

            Console.WriteLine("wrote:" + readedFile.FullName);
        }
    }
}
