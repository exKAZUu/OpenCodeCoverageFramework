using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Occf.Learner.Core {
	public interface IPropertyExtractor<out T> {
		T ExtractProperty(XElement e);
	}

	public abstract class PropertyExtractor<T> : IPropertyExtractor<T> {
		public abstract T ExtractProperty(XElement e);

		public override bool Equals(object obj) {
			return GetType() == obj.GetType();
		}

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

	public class AncestorsWithoutSiblingsAndParentExtractor : PropertyExtractor<string> {
		public override string ExtractProperty(XElement e) {
			var elements = new List<XElement>();
			if (e.Parent != null) {
				do {
					e = e.Parent;
					elements.Add(e);
				} while (e.Parent != null && e.Parent.Elements().Count() == 1);
			}
			return string.Join("/", elements.Select(e2 => e2.Name.LocalName));
		}
	}

	public class AncestorsWithoutSiblingsExtractor : PropertyExtractor<string> {
		public override string ExtractProperty(XElement e) {
			var elements = new List<XElement>();
			while (e.Parent != null && e.Parent.Elements().Count() == 1) {
				e = e.Parent;
				elements.Add(e);
			}
			return string.Join("/", elements.Select(e2 => e2.Name.LocalName));
		}
	}

	public class OnlyChildSequenceExtractor : PropertyExtractor<string> {
		public override string ExtractProperty(XElement e) {
			var elements = new List<XElement>();
			while (e.Elements().Count() == 1) {
				e = e.Elements().First();
				elements.Add(e);
			}
			return string.Join("/", elements.Select(e2 => e2.Name.LocalName));
		}
	}

	public class ChildrenSequenceExtractor : PropertyExtractor<string> {
		public override string ExtractProperty(XElement e) {
			return string.Join("/", e.Elements().Select(e2 => e2.NameOrValue()));
		}
	}

	public class SelfSequenceExtractor : PropertyExtractor<string> {
		public override string ExtractProperty(XElement e) {
			if (e.Parent == null) {
				return "";
			}
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
			if (e.Parent == null) {
				return new string[0];
			}
			return e.Parent.Elements().Select(e2 => e2.NameOrValue());
		}
	}
}