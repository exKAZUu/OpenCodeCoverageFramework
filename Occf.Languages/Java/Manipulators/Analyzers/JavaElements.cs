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

namespace Occf.Languages.Java.Manipulators.Analyzers {
	public static class JavaElements {
		public static IEnumerable<XElement> Statement(XElement root) {
			return root.Descendants("statement");
		}

		public static IEnumerable<XElement> If(XElement root) {
			return root.Descendants("statement")
					.Where(e => e.FirstElement().Value == "if");
		}

		public static IEnumerable<XElement> IfAndElseProcesses(XElement root) {
			return root.Elements("statement");
		}

		public static IEnumerable<XElement> While(XElement root) {
			return root.Descendants("statement")
					.Where(e => e.FirstElement().Value == "while");
		}

		public static XElement WhileProcess(XElement element) {
			return element.NthElement(2);
		}

		public static IEnumerable<XElement> DoWhile(XElement root) {
			return root.Descendants("statement")
					.Where(e => e.FirstElement().Value == "do");
		}

		public static XElement DoWhileProcess(XElement element) {
			return element.NthElement(1);
		}

		public static IEnumerable<XElement> For(XElement root) {
			return root.Descendants("forstatement");
		}

		public static XElement ForProcess(XElement element) {
			return element.Element("statement");
		}
	}
}