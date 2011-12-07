using System.Collections.Generic;
using System.IO;
using System.Linq;
using Occf.Tools.Core;

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
			S + "<path>".PadRight(W) + "path of directory containing files to restore" + "\n" +
			"";

		public static bool Run(IList<string> args) {
			if (args.Count <= 0)
				return Program.Print(Usage);

			var filePaths = args.Where(Directory.Exists)
				.SelectMany(
					dp =>
					Directory.GetFiles(dp, "*" + Names.BackupSuffix,
						SearchOption.AllDirectories));

			foreach (var filePath in filePaths) {
				var destPath = filePath.Substring(0,
					filePath.Length - Names.BackupSuffix.Length);
				File.Delete(destPath);
				File.Move(filePath, destPath);
				System.Console.WriteLine("restored:" + destPath);
			}
			return true;
		}
	}
}