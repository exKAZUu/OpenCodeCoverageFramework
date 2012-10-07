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
using System.Linq;
using Occf.Core.Modes;

namespace Occf.Tools.Cui
{
    public class Program
    {
        private const string S = "  ";
        private const int W = 12;

        public static readonly string Header =
                "Occf 1.1.0" + "\n" +
                "Copyright (C) 2011-2012 SAKAMOTO Kazunori" + "\n";

        private static readonly string Usage =
                Header +
                "" + "\n" +
                "Usage: Occf <command> [<args>]" + "\n" +
                "" + "\n" +
                "The occf commands are:" + "\n" +
                S + "ins[ert]".PadRight(W)
                + "Insert measurement code in source files and test files" + "\n" +
                S + "res[tore]".PadRight(W) + "Restore inserted files from backup files"
                + "\n" +
                S + "path".PadRight(W) + "Show the execution path of each test case" + "\n" +
                S + "cov[erage]".PadRight(W) + "Show the measurement result of coverage"
                + "\n" +
                S + "dup[licate]".PadRight(W)
                + "Show the duplicated test cases using coverage" + "\n" +
                S + "loc[alize]".PadRight(W) + "Show the result of bug localization" + "\n" +
                S + "klee".PadRight(W) + "Analyze klee test files for localizing bugs" + "\n" +
                S + "line_insert".PadRight(W) 
                + "Insert line number code in source files for C" + "\n" +
                S + "klee_main".PadRight(W) 
                + "Insert output .successful file code in main files " +"\n" +
                "";

        public static bool Print(string message)
        {
            Console.WriteLine(message);
            return false;
        }

        private static bool Run(string[] args)
        {
            if (args.Length < 1)
            {
                return Print(Usage);
            }

            var newArgs = args.Skip(1).ToArray();
            switch (args[0])
            {
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
                case "klee":
                    return KleeBugLocalizer.Run(newArgs);
                case "line_insert":
                    return LineInserter.Run(newArgs);
                case "klee_main":
                    return KleeMain.Run(newArgs);
            }
            return Print(Usage);
        }

        private static void Main(string[] args)
        {
            if (Run(args))
            {
                Environment.Exit(1);
            }
            Environment.Exit(0);
        }
    }
}