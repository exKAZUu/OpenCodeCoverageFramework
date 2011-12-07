using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;

namespace Occf.Languages.Python3.Operators.Selectors {
	internal class Python3ForIfConditionSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			return root.Descendants()
				.Where(e => e.Name.LocalName == "comp_if")
				.Select(e => e.Element("test_nocond"));
		}
	}
}