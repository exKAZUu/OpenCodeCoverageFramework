using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Java.Operators.Selectors {
	public class JavaForeachSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			var foreachBlocks = root.Descendants("forstatement")
					.Where(e => e.NthElement(2).Name.LocalName == "variableModifiers");
			return foreachBlocks;
		}
	}
}