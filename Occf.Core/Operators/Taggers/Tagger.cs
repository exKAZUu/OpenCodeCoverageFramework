using System.Xml.Linq;

namespace Occf.Core.Operators.Taggers {
	public abstract class Tagger {
		public abstract string Tag(XElement elements);
	}
}