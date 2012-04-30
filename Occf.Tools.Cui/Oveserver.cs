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
				S + "-l, -lang <name>".PadRight(W)
				+
				"language of target source. <name> can be Java(default), C, Python2 or Python3."
				+ "\n" +
				S + "-w, -work <path>".PadRight(W)
				+ "path of working directory used as current directory at testing." + "\n" +
				S + "".PadRight(W)
				+ "library files to measure coverage are copied in specified directory"
				+ "\n" +
				"";

		private void A() {}
	}
}