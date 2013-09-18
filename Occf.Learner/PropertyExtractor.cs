using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Occf.Learner.Core {
	public interface IPropertyExtractor<out T> {
		T ExtractProperty(XElement e);
	}

	public abstract class PropertyExtractor<T> : IPropertyExtractor<T> {
		public abstract T ExtractProperty(XElement e);

		public override string ToString() {
			var name = GetType().Name;
			var index = name.IndexOf("Extractor");
			return name.Substring(0, index);
		}
	}

	public class ChildrenCountExtractor : PropertyExtractor<int> {
		public override int ExtractProperty(XElement e) {
			return e.Elements().Count();
		}
	}

	public class ChildrenSequenceExtractor : PropertyExtractor<string> {
		public override string ExtractProperty(XElement e) {
			return string.Join("/", e.Elements().Select(e2 => e2.NameOrValue()));
		}
	}

	public class SelfSequenceExtractor : PropertyExtractor<string> {
		public override string ExtractProperty(XElement e) {
			return string.Join("/", e.Parent.Elements().Select(e2 => e2.NameOrValue()));
		}
	}

	public class ChildrenSetExtractor : PropertyExtractor<IEnumerable<string>> {
		public override IEnumerable<string> ExtractProperty(XElement e) {
			return e.Elements().Select(e2 => e2.NameOrValue());
		}
	}

	public class SelfSetExtractor : PropertyExtractor<IEnumerable<string>> {
		public override IEnumerable<string> ExtractProperty(XElement e) {
			return e.Parent.Elements().Select(e2 => e2.NameOrValue());
		}
	}
}