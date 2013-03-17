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

using System.Collections.Generic;
using System.Xml.Linq;

namespace Occf.Languages.Python2.Manipulators.Analyzers {
	public static class Python2Elements {
		// withはusingのようなもの．Loneパターン．

		public static IEnumerable<XElement> If(XElement root) {
			return root.Descendants("if_stmt");
		}

		public static IEnumerable<XElement> IfAndElseProcesses(XElement root) {
			return root.Elements("suite");
		}

		public static IEnumerable<XElement> While(XElement root) {
			return root.Descendants("while_stmt");
		}

		public static IEnumerable<XElement> WhileAndElseProcesses(XElement root) {
			return root.Elements("suite");
		}

		public static IEnumerable<XElement> For(XElement root) {
			return root.Descendants("for_stmt");
		}

		public static IEnumerable<XElement> ForAndElseProcesses(XElement root) {
			return root.Elements("suite");
		}
	}
}