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
using Paraiba.Xml.Linq;

namespace Occf.Languages.CSharp.Operators.Selectors {
	public static class CSharpElements {
		public static IEnumerable<XElement> If(XElement root) {
			return root.Descendants("if_statement");
		}

		public static XElement IfProcess(XElement element) {
			return element.NthElement(4);
		}

		public static IEnumerable<XElement> Else(XElement root) {
			return root.Descendants("else_statement");
		}

		public static XElement ElseProcess(XElement element) {
			return element.NthElement(1);
		}

		public static IEnumerable<XElement> While(XElement root) {
			return root.Descendants("while_statement");
		}

		public static XElement WhileProcess(XElement element) {
			return element.NthElement(4);
		}

		public static IEnumerable<XElement> DoWhile(XElement root) {
			return root.Descendants("do_statement");
		}

		public static XElement DoWhileProcess(XElement element) {
			return element.NthElement(1);
		}

		public static IEnumerable<XElement> For(XElement root) {
			return root.Descendants("for_statement");
		}

		public static XElement ForProcess(XElement element) {
			return ControlFlowProcess(element);
		}

		public static IEnumerable<XElement> ForEach(XElement root) {
			return root.Descendants("foreach_statement");
		}

		public static XElement ForEachProcess(XElement element) {
			return element.NthElement(7);
		}

		public static XElement ControlFlowProcess(XElement element) {
			return element.Element("embedded_statement");
		}
	}
}