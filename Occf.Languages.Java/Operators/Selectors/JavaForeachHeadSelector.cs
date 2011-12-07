using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Java.Operators.Selectors {
	public class JavaForeachHeadSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			yield return root.Descendants("block").First().FirstElement();
		}
	}
}