using System.Collections.Generic;
using System.Xml.Linq;
using Paraiba.Linq;

namespace Occf.Learner.Core {
	public class MustNotHaveFilter<T> : Filter<T, IEnumerable<T>> {
		public MustNotHaveFilter(
				string elementName, ISet<T> properties, IPropertyExtractor<IEnumerable<T>> extractor)
				: base(elementName, properties, extractor) {}

		public override bool IsAcceptable(XElement target) {
			return !Extractor.ExtractProperty(target).IsIntersect(Properties);
		}
	}
}