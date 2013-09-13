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

			var dirs = args.Select(path => new DirectoryInfo(path))
					.Where(dir => dir.Exists)
					.ToList();
			Restore(dirs.SelectMany(dir =>
					dir.EnumerateFiles("*" + OccfNames.KleeBackUpSuffix, SearchOption.AllDirectories)));
			Restore(dirs.SelectMany(dir =>
					dir.EnumerateFiles("*" + OccfNames.LineBackUpSuffix, SearchOption.AllDirectories)));
			Restore(dirs.SelectMany(dir =>
					dir.EnumerateFiles("*" + OccfNames.BackupSuffix, SearchOption.AllDirectories)));
			return true;
		}

		private static void Restore(IEnumerable<FileInfo> files) {
			foreach (var file in files) {
				var destPath = Path.GetFileNameWithoutExtension(file.FullName);
				File.Delete(destPath);
				file.MoveTo(destPath);
				File.Delete(destPath + OccfNames.LineBackUpSuffix);
				File.Delete(destPath + OccfNames.BackupSuffix);
				Console.WriteLine("restored:" + destPath);
			}
		}
	}
}