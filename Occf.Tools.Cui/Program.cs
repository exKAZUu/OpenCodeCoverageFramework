using System;
using System.Linq;

namespace Occf.Tools.Cui {
	public class Program {
		private const string S = "  ";
		private const int W = 12;
		private static readonly string Usage =
			"Occf 1.0.0" + "\n" +
			"Copyright (C) 2011 Kazunori SAKAMOTO" + "\n" +
			"" + "\n" +
			"Usage: Occf <command> [<args>]" + "\n" +
			"" + "\n" +
			"The occf commands are:" + "\n" +
			S + "ins[ert]".PadRight(W) + "Insert measurement code in source files and test files" + "\n" +
			S + "res[tore]".PadRight(W) + "Restore inserted files from backup files" + "\n" +
			S + "path".PadRight(W) + "Show the execution path of each test case" + "\n" +
			S + "cov[erage]".PadRight(W) + "Show the measurement result of coverage" + "\n" +
			S + "dup[licate]".PadRight(W) + "Show the duplicated test cases using coverage" + "\n" +
			S + "loc[alize]".PadRight(W) + "Show the result of bug localization" + "\n" +
			"";

		public static bool Print(string message) {
			Console.WriteLine(message);
			return false;
		}
		
		private static bool Run(string[] args) {
			if (args.Length < 1) {
				args = new[] {
						"ins",
						@"C:\Users\exKAZUu\Dropbox\Linux\Coverage\BTree-Python",
						@"-l",
						@"Python2",
				};
			}
			//args = new[] {
			//        "ins",
			//        @"C:\Users\exKAZUu\Dropbox\Linux\Coverage\QuickSort-Java",
			//        @"C:\Users\exKAZUu\Dropbox\Linux\Coverage\QuickSort-Java\src\test",
			//};
			if (args.Length < 1)
				return Print(Usage);

			var newArgs = args.Skip(1).ToArray();
			switch (args[0]) {
			case "ins":
			case "insert":
				return Inserter.Run(newArgs);
			case "res":
			case "restore":
				return Restorer.Run(newArgs);
			case "cov":
			case "coverage":
				return CoverageDisplay.Run(newArgs);
			case "dup":
			case "duplicate":
				return DuplicationDetector.Run(newArgs);
			case "path":
				return PathAnalyzer.Run(newArgs);
			case "loc":
			case "localize":
				return BugLocalizer.Run(newArgs);
			}
			return Print(Usage);
		}

		private static void Main(string[] args) {
			if (Run(args))
				Environment.Exit(1);
			Environment.Exit(0);
		}
	}
}