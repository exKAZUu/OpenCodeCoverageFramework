using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;

namespace Occf.Languages.Python3.Operators.Selectors {
	public class Python3TernaryOperatorSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			return root.Descendants()
				.Where(e => e.Name.LocalName == "test")
				.Select(e => e.Elements().ElementAtOrDefault(2))
				.Where(e => e != null);
		}
	}
}