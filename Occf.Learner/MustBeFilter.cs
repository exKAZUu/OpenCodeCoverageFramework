using System.Collections.Generic;
using System.Xml.Linq;

namespace Occf.Learner.Core {
	public class MustBeFilter<T> : Filter<T, T> {
		public MustBeFilter(string elementName, ISet<T> properties, IPropertyExtractor<T> extractor)
				: base(elementName, properties, extractor) {}

		public override bool IsAcceptable(XElement target) {
			return Properties.Contains(Extractor.ExtractProperty(target));
		}
	}
}