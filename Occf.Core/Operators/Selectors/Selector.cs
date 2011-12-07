using System.Collections.Generic;
using System.Xml.Linq;

namespace Occf.Core.Operators.Selectors {
	public abstract class Selector {
		public abstract IEnumerable<XElement> Select(XElement root);
	}
}