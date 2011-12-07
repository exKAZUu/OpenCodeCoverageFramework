using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.C.Operators.Selectors {
	public class CSwitchSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			return root.Descendants("selection_statement")
					.Where(e => e.FirstElement().Value == "switch");
		}
	}
}