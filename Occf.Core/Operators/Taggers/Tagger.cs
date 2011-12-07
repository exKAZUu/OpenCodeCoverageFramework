using System.Xml.Linq;

namespace Occf.Core.Operators.Taggers {
	public abstract class Tagger {
		public abstract string Generate(XElement elements);
	}
}