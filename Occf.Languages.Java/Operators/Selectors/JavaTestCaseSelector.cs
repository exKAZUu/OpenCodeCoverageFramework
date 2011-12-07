using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Java.Operators.Selectors {
	public class JavaTestCaseLabelTailSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			return root.Descendants()
				.Where(e => e.Name.LocalName == "methodDeclaration")
				.Where(e => {
					var name = e.NthElement(2).Value;
					if (name.StartsWith("test"))
						return true;
					var annotation = e.FirstElement().Element("annotation");
					if (annotation != null && annotation.Value == "@Test")
						return true;
					return false;
				});
		}
	}
}