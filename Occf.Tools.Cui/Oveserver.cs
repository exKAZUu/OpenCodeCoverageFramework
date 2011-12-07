using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Occf.Tools.Cui {
	public class Oveserver {
		private const string S = "  ";
		private const int W = 20;
		private static readonly string usage =
			"Occf 1.0.0" + "\n" +
			"Copyright (C) 2011 Kazunori SAKAMOTO" + "\n" +
			"" + "\n" +
			"Usage: Occf insert <src> [<test>] [options]" + "\n" +
			"" + "\n" +
			S + "<src>".PadRight(W) + "path of source code directory" + "\n" +
			S + "<test>".PadRight(W) + "path of test code directory" + "\n" +
			S + "-l, -lang <name>".PadRight(W) + "language of target source. <name> can be Java(default), C, Python2 or Python3." + "\n" +
			S + "-w, -work <path>".PadRight(W) + "path of working directory used as current directory at testing." + "\n" +
			S + "".PadRight(W) + "library files to measure coverage are copied in specified directory" + "\n" +
			"";

		private void A() {
			
		}
	}
}
