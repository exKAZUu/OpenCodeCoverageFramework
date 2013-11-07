using System.Collections.Generic;
using System.Xml.Linq;
using Paraiba.Linq;

namespace Occf.Learner.Core {
	public class MustHaveFilter<T> : Filter<T, IEnumerable<T>> {
		public MustHaveFilter(
				string elementName, ISet<T> properties, IPropertyExtractor<IEnumerable<T>> extractor)
				: base(elementName, properties, extractor) {}

		public override bool IsAcceptable(XElement target) {
			return Extractor.ExtractProperty(target).IsIntersect(Properties);
		}
	}
}