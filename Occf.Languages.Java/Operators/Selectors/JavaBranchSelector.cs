using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Java.Operators.Selectors {
	public class JavaBranchSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			//var eqs = root.Descendants("equalityExpression")
			//        .Where(e => e.Elements().Count() > 1)
			//        .Where(e => e.Parents().Any())
			var ifWhileDoWhiles = root.Descendants("statement")
					.Where(
							e =>
							e.FirstElement().Value == "if"
							|| e.FirstElement().Value == "while"
							|| e.FirstElement().Value == "do")
					.Select(e => e.Element("parExpression"))
					.Select(e => e.NthElement(1));
			var fors = root.Descendants("forstatement")
					.Where(e => e.NthElement(2).Name.LocalName != "variableModifiers")
					.SelectMany(e => e.Elements("expression"));
			var ternaries = root.Descendants("conditionalExpression")
					.Where(e => e.Elements().Count() > 1)
					.Select(e => e.FirstElement());
			return ifWhileDoWhiles.Concat(fors).Concat(ternaries);
		}
	}
}