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
using System.IO;
using System.Linq;
using Occf.Core.Utils;

namespace Occf.Tools.Cui {
	public static class Restorer {
		private const string S = "  ";
		private const int W = 12;

		private static readonly string Usage =
				"Occf 1.0.0" + "\n" +
				"Copyright (C) 2011 Kazunori SAKAMOTO" + "\n" +
				"" + "\n" +
				"Usage: Occf restore <path1> <path2> ..." + "\n" +
				"" + "\n" +
				S + "<path>".PadRight(W) + "path of directory containing files to restore"
				+ "\n" +
				"";

		public static bool Run(IList<string> args) {
			if (args.Count <= 0) {
				return Program.Print(Usage);
			}

			var filePaths = args.Where(Directory.Exists)
					.SelectMany(
							dp =>
							Directory.GetFiles(
									dp, "*" + OccfNames.BackupSuffix,
									SearchOption.AllDirectories));

		    var lineFilePaths = args.Where(Directory.Exists)
		            .SelectMany(
		                    dp =>
		                    Directory.GetFiles(
		                            dp, "*" + OccfNames.LineBackUpSuffix,
		                            SearchOption.AllDirectories));

            var kleeFilePaths = args.Where(Directory.Exists)
                    .SelectMany(
                            dp =>
                            Directory.GetFiles(
                                    dp, "*" + OccfNames.KleeBackUpSuffix,
                                    SearchOption.AllDirectories));
            //nullエラー出ないよね？
		    filePaths = filePaths.Concat(lineFilePaths.Concat(kleeFilePaths));
            
            for(var i=0; i<filePaths.Count(); i++) {
                Console.WriteLine("filepaths :" + filePaths.ElementAt(i));
            }

			foreach (var filePath in filePaths) {
				var destPath = filePath.Substring(
						0,
						filePath.Length - OccfNames.BackupSuffix.Length);
				File.Delete(destPath);
				File.Move(filePath, destPath);
				Console.WriteLine("restored:" + destPath);
			}
			return true;
		}
	}
}