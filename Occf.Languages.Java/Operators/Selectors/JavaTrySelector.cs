using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Java.Operators.Selectors {
	public class JavaTrySelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			return root.Descendants("trystatement");
		}
	}
}