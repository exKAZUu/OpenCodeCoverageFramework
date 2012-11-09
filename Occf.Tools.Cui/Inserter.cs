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
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Code2Xml.Core.Position;
using IronPython.Runtime;
using NDesk.Options;
using Occf.Core.CoverageCode;
using Occf.Core.CoverageInformation;
using Occf.Core.Modes;
using Occf.Core.TestInfos;
using Occf.Core.Utils;
using Paraiba.IO;

namespace Occf.Tools.Cui {
	public class Inserter {
		private const string S = "  ";
		private const int W = 20;

		private static readonly string Usage =
				Program.Header +
				"" + "\n" +
				"Usage: Occf insert -r <root_dir> [options] <src_dir>"
				+ "\n" +
				"" + "\n" +
				S + "-r, -root <root_dir>".PadRight(W)
				+ "root directory for writing project information and library files to measure coverage"
				+ "\n" +
				S + "-t, -test <test>".PadRight(W) +
				"test code directory for execluding files as measurement targets and for localizing faults"
				+ "\n" +
				S + "-l, -lang <name>".PadRight(W) +
				"language of target source. <name> can be Java(default), C, Python2 or Python3."
				+ "\n" +
				S + "-p, -pattern <name>".PadRight(W)
				+ "search pattern for exploring target source files in the root directory."
				+ "\n" +
				S + "-i, -lib <path>".PadRight(W)
				+ "path of library directory where library files for measuring coverage are copied."
				+ "\n" +
				"";

		public static bool Run(IList<string> args) {
		    var rootDirPath = "";
		    var srcDirPath = "";
			var testDirPath = "";
			var languageName = "Java";
			var libDirPath = "";
			var patterns = new List<string>();

			// parse options
			var p = new OptionSet {
					{ "r|root=", v => rootDirPath = v },
					{ "t|test=", v => testDirPath = v },
					{ "l|lang=", v => languageName = v },
					{ "p|pattern=", patterns.Add },
					{ "i|lib=", v => libDirPath = v },
			};

            //確認用 ** 
            /*
            for (int i = 0; i < args.Count; i++ ) {
                Console.WriteLine("args"+ i + " = " + args[i]);
            }*/// ** 
            
            // コマンドをパース "-"指定されないのだけargs[]に残る
            try {
                args = p.Parse(args);
            } catch {
                Console.WriteLine("catch");
                return Program.Print(Usage);
            }

            //確認用追加 **
            /*
            for (int i = 0; i < args.Count; i++)
            {
                Console.WriteLine("argsB" + i + " = " + args[i]);
            }*/// **

            //Console.WriteLine("before args count");// 確認
			
            if (string.IsNullOrEmpty(rootDirPath)) {
			    //Console.WriteLine("count < 1");//確認
				return Program.Print(Usage);
			}
		    //Console.WriteLine("before mode:");//確認
			
            if(args.Count > 1) {
                for(var i = 1; i<args.Count; i++) {
                    Console.WriteLine("Path: " + args[i] + " is a wrong designated method");
                }
                Console.WriteLine("you can designate only one srcDir now.");
                Console.WriteLine("please read how to use Occf inserter .");
                Console.WriteLine("continue.");
            }

            srcDirPath = args.Count < 1 ? rootDirPath : args[0];
            

            CoverageMode mode;
			try {
				mode = CoverageModes.GetCoverageModeByClassName(languageName);
			} catch {
				return
						Program.Print(
								"error: cant't load script file for programming language of "
								+ languageName);
			}
		    //Console.WriteLine("before rootDir:");//確認
			var rootDir = new DirectoryInfo(rootDirPath);
			if (!rootDir.Exists) {
				return
						Program.Print(
								"Root directory doesn't exist.\nroot:" + rootDir.FullName);
			}
            //Console.WriteLine("before srcDir:");//確認
            var srcDir = new DirectoryInfo(srcDirPath);
            if (!srcDir.Exists)
            {
                return
                        Program.Print(
                                "Source directory doesn't exist.\nsrc:" + srcDir.FullName);
            }


		    //Console.WriteLine("before testDir:");//確認
			DirectoryInfo testDir = null;
			if (!string.IsNullOrEmpty(testDirPath)) {
				testDir = new DirectoryInfo(testDirPath);
				if (!testDir.Exists) {
					return
							Program.Print(
									"Error: test code directory doesn't exist.\ntest:" + testDir.FullName);
				}
			}
		    //Console.WriteLine( "before libDir");//確認
			var libDir = rootDir;
			if (!string.IsNullOrEmpty(libDirPath)) {
				libDir = new DirectoryInfo(libDirPath);
				if (!libDir.Exists) {
					return
							Program.Print(
									"Error: working directory doesn't exist.\nwork:" + libDir.FullName);
				}
			}

            //確認　**
            /*
		    Console.WriteLine("call insertMeasurementCode:");
		    Console.WriteLine("rootDir: " + rootDir.Name);
		    Console.WriteLine("srcDir: " + srcDir.Name);
		    string pat = patterns.Count > 0 ? patterns[0] : "null";
		    Console.WriteLine("patterns: " + pat);
		    string tDir = testDir != null ? testDir.Name : "null";
		    Console.WriteLine("testDir:" + tDir);
		    Console.WriteLine("libDir: " + libDir.Name);
            *///　**
			InsertMeasurementCode(rootDir, patterns, testDir, libDir, mode);
			return true;
		}

		public static void InsertMeasurementCode(
				DirectoryInfo rootDir, List<string> patterns, DirectoryInfo testDir,
				DirectoryInfo libDir,
				CoverageMode mode) {
			Contract.Requires<ArgumentException>(rootDir.Exists);
			Contract.Requires<ArgumentException>(
					testDir == null || testDir.Exists);
			Contract.Requires<ArgumentException>(libDir.Exists);
			Contract.Requires<ArgumentNullException>(mode != null);
			patterns = patterns ?? new List<string>();
		    
            //root
			var covInfo = new CoverageInfo(
					rootDir.FullName, mode.Name, SharingMethod.File);
			//(o)root or src?
            var testInfo = testDir != null
					               ? new TestInfo(rootDir.FullName)
					               : null;
            //root
			RemoveExistingCoverageDataFiles(rootDir, libDir);

			mode.RemoveLibraries(libDir);
            //src
			WriteProductionCodeFiles(rootDir, patterns, testDir, mode, covInfo);
			if (testInfo != null) {
                //(o)root or src
				WriteTestCodeFiles(rootDir, testDir, mode, testInfo);
			} else {
				// Initialize test information with empty contents
				testInfo = new TestInfo(rootDir.FullName);
				testInfo.TestCases.Add(
						new TestCase("nothing", "nothing", new CodePosition()));
			}
            //root
			WriteInfoFiles(rootDir, covInfo, testInfo);

			CopyLibraries(mode, libDir);
		}

		private static void CopyLibraries(
				CoverageMode mode, DirectoryInfo dirInfo) {
			mode.CopyLibraries(dirInfo);
		}

		private static void RemoveExistingCoverageDataFiles(
				DirectoryInfo rootDir, DirectoryInfo workDir) {
			const string filePattern = "*" + OccfNames.CoverageData;
			var targets = rootDir.EnumerateFiles(
					filePattern, SearchOption.AllDirectories);
			if (workDir != null) {
				targets = targets.Concat(workDir.EnumerateFiles(filePattern));
			}
			foreach (var target in targets.ToList()) {
				target.Delete();
			}
		}

		private static void WriteProductionCodeFiles(
				DirectoryInfo rootDir, IEnumerable<string> patterns, DirectoryInfo testDir,
				CoverageMode mode, CoverageInfo info) {

		    IEnumerable<FileInfo> paths;

		    IEnumerable<string> bakcupPatterns = new List<string>(){"*"+OccfNames.BackupSuffix, "*"+OccfNames.LineBackUpSuffix, "*"+OccfNames.KleeBackUpSuffix}; 
		    IEnumerable<FileInfo> backups;

            //　**
            /*
		    for (int i = 0; i < patterns.Count(); i++) {
                Console.WriteLine("patterns "+ i + " : " + patterns.ElementAt(i));
		        
		    }
            for (int i = 0; i < bakcupPatterns.Count(); i++)
            {
                Console.WriteLine("bkpats " + i + " : " + bakcupPatterns.ElementAt(i));

            }
            */
		    //　**
            /*
		    IEnumerable<FileInfo> pths = rootDir.EnumerateFiles("*", SearchOption.AllDirectories);
		    IEnumerable<FileInfo> pths3;
		    var lpth3 = pths.ToList();
            var lpth3b = new List<FileInfo>();
            for(var i=lpth3.Count-1; i>=0; i--) {
                var fi = lpth3.ElementAt(i);
                foreach (var stpt in mode.FilePatterns) {
                    if(fi.FullName.EndsWith(stpt.Substring(1))) {
                        lpth3b.Add(fi);
                        break;
                    }
                }
            }
            Console.WriteLine("lpth3.co :" + lpth3b.Count());
		    pths3 = lpth3b.CopyTo();
            foreach (var fileInfo in pths3) {
		        Console.WriteLine("lp3fi : " + fileInfo.FullName);
		    }
		    var pat3 = new List<FileInfo>();
            for(int i=0; i<lpth3.Count; i++) {
                var fi = lpth3.ElementAt(i);
                
                
            }
		    Console.WriteLine("pat3 Count : " + pat3.Count());
            //
		    var ppp = new List<FileInfo>();
		    var fdpp = new List<FileInfo>();
		    var all = lpth3;
            foreach (var pattern in patterns)
            {
                Console.WriteLine("pattaaaa : " + pattern);
                var newp = rootDir.GetFiles(
                        pattern, SearchOption.AllDirectories);
                var s = newp.ToList();
                Console.WriteLine("newp Co : " + newp.Count());
                foreach (var fileInfo in s) {
                    Console.WriteLine("fionfo SS : " + fileInfo.FullName);
                    ppp.Add(fileInfo);
                }
                
            }
		    Console.WriteLine("ppp Co : " + ppp.Count());

		    for(int i=all.Count-1; i>=0; i--) {
		        foreach (var fileInfo in ppp) {
		            if(fileInfo.FullName == all[i].FullName) {
		                fdpp.Add(all[i]);
		                all.Remove(all[i]);
                        break;
		            }
		        }
		    }
            Console.WriteLine("fddd : " + fdpp.Count);
		    Console.WriteLine("lpth3 : "+ lpth3.Count);
		    Console.WriteLine("all   : "+ all.Count);
            */

            //Console.WriteLine("psize:" + patterns.Count());//確認
		    //var allFileList = rootDir.EnumerateFiles("*", SearchOption.AllDirectories).ToList();
		    //var pathsFilteredList = new List<FileInfo>();
            if(!patterns.Any()) { // -p　指定が無い場合：拡張子判定
                paths = mode.FilePatterns.SelectMany(
                    pat => rootDir.GetFiles(
                            pat, SearchOption.AllDirectories));
                /*
                for(var i=0; i<allFileList.Count; i++) {
                    var fileInfo = allFileList.ElementAt(i);
                    foreach (var filePattern in mode.FilePatterns) {
                        if(fileInfo.FullName.EndsWith(filePattern.Substring(1))) {
                            pathsFilteredList.Add(fileInfo);
                            break;
                        }
                    }
                }
                paths = pathsFilteredList;*/
            } else {// -p指定があった時はそれを格納
                //var allFiles = allFileList;
                paths = patterns.Where(pattern => !mode.FilePatterns.Contains(pattern))
                                    .SelectMany(
                                            pat => rootDir.GetFiles(pat, SearchOption.AllDirectories));
                /*
                var patFileLists = new List<FileInfo>();
                foreach (var pattern in patterns) {
                    var singlePatLists = rootDir.GetFiles(pattern, SearchOption.AllDirectories).ToList();
                    foreach (var fileInfo in singlePatLists) {
                        patFileLists.Add(fileInfo);
                    }
                }
               */

            }

            backups = bakcupPatterns.Where(bakcupPattern => !mode.FilePatterns.Contains(bakcupPattern))
                                    .SelectMany(
                                            bpat=> rootDir.GetFiles(bpat, SearchOption.AllDirectories));


		   

		    //Console.WriteLine("pathsNum : " + paths.Count());
		    foreach (var fileInfo in paths) {
		        //Console.WriteLine("fileinfo : " + fileInfo.FullName);
		    }

            //Console.WriteLine("bkcount : " + backups.Count());//確認

            //確認　**
            /*
            for (int i = 0; i < paths.Count(); i++)            {
                Console.WriteLine("paths2AAA :" + paths.ElementAt(i).FullName);
            }*/

            //確認　***
            /*
            for (int i = 0; i < paths.Count(); i++){
                Console.WriteLine("paths1 :" + paths.ElementAt(i));
            }
            for (int i = 0; i < backups.Count(); i++){
                Console.WriteLine("pathsBK :" + backups.ElementAt(i));
            }
            */ 
            //***

		    var pathList = paths.ToList();
		    var bkpathList = backups.ToList();

		    for (var i = pathList.Count - 1; i > 0; i-- ) {
                if (bkpathList.Any(bkpath => pathList[i].FullName == bkpath.FullName)) {
                    pathList.Remove(pathList[i]);
                }
            }
            /* 上の元コード
            for (var i = pathList.Count - 1; i > 0; i--){
                foreach (var bkpath in bkpathList){
                    if (pathList[i].FullName == bkpath.FullName){
                        pathList.Remove(pathList[i]);
                        break;
                    }
                }
            }*/
		    paths = pathList;　//バックアップファイルを除いたリストに置き換え
            
            
            /*
            paths = mode.FilePatterns.SelectMany(
                    pat => rootDir.EnumerateFiles(
                            pat, SearchOption.AllDirectories));
           
            paths =
                    paths.Concat(
                            patterns.Where(pattern => !mode.FilePatterns.Contains(pattern))
                                    .SelectMany(
                                            pat => rootDir.EnumerateFiles(pat, SearchOption.AllDirectories)));
            */

            //確認　**
            /*
            for (int i = 0; i < paths.Count(); i++){
                Console.WriteLine("paths2 :" + paths.ElementAt(i).FullName);
            }*/
            
            //**
            
            // ignore test code in the directory of production code
            if (testDir != null){
                paths = paths.Where(f => !f.FullName.StartsWith(testDir.FullName));
            }

            //確認　**
            /*
            for (int i = 0; i < paths.Count(); i++){
                Console.WriteLine("paths2BB :" + paths.ElementAt(i).FullName);
            }*/
            
			foreach (var path in paths.ToList()) {
				var bakPath = rootDir.GetFile(path + OccfNames.BackupSuffix).FullName;
                //対象ファイルに対してKlee_backやLine_backがあるときは作成しない
                if(!(File.Exists(path.FullName+OccfNames.LineBackUpSuffix)) 
                    && !(File.Exists(path.FullName+OccfNames.KleeBackUpSuffix))) {
                    path.CopyTo(bakPath, true);
                    
                }
				var outPath = CoverageCodeGenerator.WriteCoveragedCode(
						mode, info, path, rootDir);
				Console.WriteLine("wrote:" + outPath);
			}
		}

		private static void WriteTestCodeFiles(
				DirectoryInfo rootDir, DirectoryInfo testDir, CoverageMode prof,
				TestInfo info) {
			var paths = prof.FilePatterns.SelectMany(
					pattern => testDir.EnumerateFiles(
							pattern, SearchOption.AllDirectories));
			foreach (var path in paths.ToList()) {
				var bakPath = rootDir.GetFile(path + OccfNames.BackupSuffix).FullName;
                //path.CopyTo(bakPath, false); //バックファイルの上書き不可　元
                path.CopyTo(bakPath, true);
				var outPath = CoverageCodeGenerator.AnalyzeAndWriteIdentifiedTest(
						prof, info, path, rootDir);
				Console.WriteLine("wrote:" + outPath);
			}
		}

		private static void WriteInfoFiles(
				DirectoryInfo rootDir, CoverageInfo covInfo, TestInfo testInfo) {
			var formatter = new BinaryFormatter();
			WriteCoverageInfo(rootDir, covInfo, formatter);
			if (testInfo == null) {
				return;
			}
			WriteTestInfo(rootDir, testInfo, formatter);
		}

		private static void WriteCoverageInfo(
				DirectoryInfo rootDir, CoverageInfo covInfo, BinaryFormatter formatter) {
			var covPath = Path.Combine(rootDir.FullName, OccfNames.CoverageInfo);
			using (var fs = new FileStream(covPath, FileMode.Create)) {
				formatter.Serialize(fs, covInfo);
			}
		}

		private static void WriteTestInfo(
				DirectoryInfo rootDir, TestInfo testInfo, BinaryFormatter formatter) {
			var testPath = Path.Combine(rootDir.FullName, OccfNames.TestInfo);
			using (var fs = new FileStream(testPath, FileMode.Create)) {
				formatter.Serialize(fs, testInfo);
			}
		}
	}
   

}