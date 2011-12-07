using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.C.Operators.Selectors {
	public class CBranchSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			var ifs = root.Descendants("selection_statement")
					.Where(e => e.FirstElement().Value == "if")
					.Select(e => e.NthElement(2));
			var whilesAndDoWhiles = root.Descendants("iteration_statement")
					.Select(e => e.FirstElement().Value == "for" ? e.NthElement(3) : e)
					.SelectMany(e => e.Elements("expression"));
			var ternaries = root.Descendants("conditional_expression")
					.Where(e => e.Elements().Count() > 1)
					.Select(e => e.FirstElement());
			return ifs.Concat(whilesAndDoWhiles).Concat(ternaries);
		}
	}
}