using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Occf.Core.Operators.Selectors {
	public class NoSelector : Selector {
		public static Selector Instance = new NoSelector();

		public override IEnumerable<XElement> Select(XElement root) {
			return Enumerable.Empty<XElement>();
		}
	}
}