﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Lua.Operators.Selectors {
	public static class LuaElements {
		public static IEnumerable<XElement> If(XElement root) {
			return root.Descendants("stat");
		}

		public static IEnumerable<XElement> IfAndElseProcesses(XElement root) {
			return root.Elements("block");
		}

		public static IEnumerable<XElement> While(XElement root) {
			return root.Descendants("whileStatement");
		}

		public static XElement WhileProcess(XElement element) {
			return element.FirstElement("statement");
		}

		public static IEnumerable<XElement> DoWhile(XElement root) {
			return root.Descendants("doWhileStatement");
		}

		public static XElement DoWhileProcess(XElement element) {
			return element.FirstElement("statement");
		}

		public static IEnumerable<XElement> For(XElement root) {
			return root.Descendants("stat")
				.Where(e => e.Elements().First().Value == "for" &&
				            e.Elements().ElementAt(2).Value == "=");
		}

		public static XElement ForProcess(XElement element) {
			return element.FirstElement("block");
		}

		public static IEnumerable<XElement> ForEach(XElement root) {
			return root.Descendants("stat")
				.Where(e => e.Elements().First().Value == "for" &&
				            e.Elements().ElementAt(2).Value == "in");
		}

		public static XElement ForEachProcess(XElement element) {
			return element.FirstElement("block");
		}

		public static IEnumerable<XElement> ForAndForEach(XElement root) {
			return root.Descendants("stat");
		}

		public static XElement ForAndForEachProcess(XElement root) {
			return root.FirstElement("block");
		}

		public static XElement ControlFlowProcess(XElement element) {
			return element.FirstElement("block");
		}
	}
}