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
using System.Linq;
using System.Xml.Linq;
using Paraiba.Xml.Linq;

namespace Occf.Languages.C.Operators.Selectors {
	public static class CElements {
		public static IEnumerable<XElement> Loop(XElement root) {
			return root.Descendants("iteration_statement");
		}

		public static IEnumerable<XElement> While(XElement root) {
			return Loop(root)
					.Where(e => e.FirstElement().Value == "while");
		}

		public static IEnumerable<XElement> DoWhile(XElement root) {
			return Loop(root)
					.Where(e => e.FirstElement().Value == "do");
		}

		public static IEnumerable<XElement> For(XElement root) {
			return Loop(root)
					.Where(e => e.FirstElement().Value == "for");
		}

		public static XElement LoopProcess(XElement element) {
			return element.Element("statement");
		}

		public static IEnumerable<XElement> If(XElement root) {
			return root.Descendants("selection_statement")
					.Where(e => e.FirstElement().Value == "if");
		}

		public static IEnumerable<XElement> SelectionProcesses(XElement element) {
			return element.Elements("statement");
		}
	}
}