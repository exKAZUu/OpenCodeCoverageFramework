using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Paraiba.Core;

namespace Occf.Core.CodeInformation {
	public static class CodePositionParser {
		public static CodePosition Create(XElement element) {
			return Create(element.DescendantsAndSelf(),
				element.Value.Length);
		}

		public static CodePosition Create(IEnumerable<XElement> element) {
			return Create(element.DescendantsAndSelf(),
				element.Sum(e_ => e_.Value.Length));
		}

		private static CodePosition Create(
			IEnumerable<XElement> descendants, int length) {
			int startLine = 0, startPos = 0;
			var firstElement =
				descendants.FirstOrDefault(e => e.Attribute("startline") != null);
			if (firstElement != null) {
				var startLineAttr = firstElement.Attribute("startline");
				var startPosAttr = firstElement.Attribute("startpos");

				startLine = startLineAttr.Value.ToIntOrDefault(startLine);
				if (startPosAttr != null) {
					startPos = startPosAttr.Value.ToIntOrDefault(startPos);
				}
			}

			int endLine = startLine, endPos = startPos + length;
			var lastElement =
				descendants.LastOrDefault(e => e.Attribute("startline") != null);
			if (lastElement != null) {
				var startLineAttr = lastElement.Attribute("startline");
				var startPosAttr = lastElement.Attribute("startpos");
				var endLineAttr = lastElement.Attribute("endline");
				var endPosAttr = lastElement.Attribute("endpos");

				endLine = startLineAttr.Value.ToIntOrDefault(endLine);
				if (endLineAttr != null) {
					endLine = endLineAttr.Value.ToIntOrDefault(endLine);
				}
				if (startPosAttr != null) {
					int value;
					if (int.TryParse(startPosAttr.Value, out value))
						endPos = value + startPosAttr.Value.Length;
				}
				if (endPosAttr != null) {
					endPos = endPosAttr.Value.ToIntOrDefault(endPos);
				}
			}

			return new CodePosition(startLine, endLine, startPos, endPos);
		}
	}
}