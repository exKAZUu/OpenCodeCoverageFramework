﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OccfLineInsert;

namespace OccfLineInsert
{
    class Program
    {
        static void Main(string[] args)
        {
            var lineInsert = new LineInsert();
            var returnBackUp = new ReturnBackUp();
            var lineAnalysis = new LineAnalysis();
            var lineMapCreater = new LineMapCreater();

            Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
            const string absolutePath = @"C:\Documents and Settings\Administrator\My Documents\Visual Studio 2010\Projects\ConsoleApplication1\ConsoleApplication1\sample";
            const string relativePath = @"../../sample";
            const string relativePath2 = @"../../sample/test";
            const string relativePath3 = @"../../sample/test/line";
            const string relativePath4 = @"../../sample/insert";
            const string relativePath5 = @"../../sample/return";
            const string relativePath6 = @"../../sample/revert";
            const string rerativePath7 = @"../../sample/analysis";
            //var basicInput = Console.ReadLine();

            //Console.WriteLine(relativePath.Substring(0, relativePath.Length-5));
            //Console.WriteLine(relativePath);

            //FileInfo fileInfo = new FileInfo(relativePath + "/filetest.txt");
            //Console.WriteLine(fileInfo.FullName);
            //Console.WriteLine(@""""+ @"main" +@"""");
            //fileInfo.Replace(relativePath + "/filetest2.txt", relativePath + "/filetest3.txt");

            //lineInsert.LineInserts(absolutePath);

            //lineInsert.LineInserts(relativePath);

            //埋め込み
            //lineInsert.LineInserts(relativePath4);
            //その復活
            //returnBackUp.RevertBackUp(relativePath4);

            //復活＆埋め込み
            //returnBackUp.RevertBackUp(relativePath4);
            //lineInsert.LineInserts(relativePath4);

            //元の行数
            //lineAnalysis.OutPutTrueLineNumber(relativePath3);

            //復活
            //returnBackUp.RevertBackUp(relativePath5);
            //returnBackUp.RevertBackUp(relativePath6);
            
            var rootDir = new DirectoryInfo(rerativePath7);
            var testDir = new DirectoryInfo(rerativePath7 + "/test");
            //lineAnalysis.CreateLineDic(rootDir, testDir);
            //lineMapCreater.MapFileCreater(rootDir, testDir);
            var mappingFile = new FileInfo(rerativePath7 + "/.occf_map_file");
            lineMapCreater.LineMapKakunin(mappingFile, rootDir);

            //埋め込み、停止、復活
            //lineInsert.LineInserts(relativePath4);
            //var basicInput = Console.ReadLine();
            //returnBackUp.RevertBackUp(relativePath4);

            //埋め込み、解析、停止、復活
            //lineInsert.LineInserts(relativePath4);
            //lineAnalysis.OutPutTrueLineNumber(relativePath4);
            //Console.WriteLine("Please input Enter");
            //var basicInput = Console.ReadLine();
            //returnBackUp.RevertBackUp(relativePath4);

            
            //lineInsert.LineInserts(basicInput);
            //Console.WriteLine(LineInsert.LineInsetr2(absolutePath));
            //Console.WriteLine(LineInsert.LineInsetr2(relativePath));
            //Console.WriteLine(LineInsert.LineInsetr2(basicInput));

            /*
            IEnumerable<string> ie1 = new[] { "aa", "bb", "cc" };
            IEnumerable<string> ie1b = new[] { "aa", "bb", "cc" };
            IEnumerable<string> ie2 = new[] { "dd", "ee", "ff", "gg" };

            var ie3 = ie1.Except(ie1b);

            ie1 = ie1.Concat(ie3);

            Console.WriteLine(ie3.Count());

            foreach (var ie in ie3) {
                Console.WriteLine("zz");
            }


            for (var i = 0; i < ie1.Count(); i++ )
            {
                Console.WriteLine(ie1.ElementAt(i));
            }*/



        }
    }
}
