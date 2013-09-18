using System.Collections.Generic;
using System.Xml.Linq;

namespace Occf.Learner.Core {
	public class NopFilter : Filter<object, object> {
		public NopFilter(string elementName)
				: base(elementName, null, null) {}

		public override bool IsAcceptable(XElement target) {
			return true;
		}
	}
}