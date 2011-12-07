using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Java.Operators.Selectors {
	public class JavaCatchBlockSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			return root.Elements("catches")
					.SelectMany(e => e.Elements("catchClause"))
					.Select(e => e.Element("block").FirstElement());
		}
	}
}