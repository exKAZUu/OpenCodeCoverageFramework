using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;

namespace Occf.Languages.Java.Operators.Selectors {
	public class JavaInitializerSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			return root.Descendants("variableDeclarator")
				.SelectMany(e => e.Elements("variableInitializer"))
				.SelectMany(e => e.Elements("expression"));
		}
	}
}