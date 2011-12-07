using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.C.Operators.Selectors {
	public class CStatementSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			return root.Descendants("statement")
					.Where(
							e =>
							e.FirstElement().Name.LocalName != "labeled_statement"
							&& e.FirstElement().Name.LocalName != "compound_statement");
		}
	}
}