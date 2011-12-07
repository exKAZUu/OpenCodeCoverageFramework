using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.C.Operators.Selectors {
	public class CInitializerSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			return root.Descendants("initializer")
				.Where(e => e.Parent.Name.LocalName != "initializer_list")
				.SelectMany(e => e.Elements("assignment_expression"));
		}
	}
}