using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core;
using Paraiba.Xml.Linq;

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

		protected bool Equals(PropertyExtractor<T> other) {
			return true;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}
			return Equals((PropertyExtractor<T>)obj);
		}

		public override int GetHashCode() {
			return GetType().GetHashCode();
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
			return string.Join("/", elements.Select(e2 => e2.NameOrTokenText()));
		}
	}

	public class OnlyChildSequenceExtractor : PropertyExtractor<string> {
		public override string ExtractProperty(XElement e) {
			var elements = new List<XElement>();
			while (e.Elements().Count() == 1) {
				e = e.Elements().First();
				elements.Add(e);
			}
			return string.Join("/", elements.Select(e2 => e2.NameOrTokenText()));
		}
	}

	public class ElementSequenceExtractor : PropertyExtractor<string> {
		private readonly int _depth;

		public ElementSequenceExtractor(int depth) {
			_depth = depth;
		}

		public override string ExtractProperty(XElement e) {
			var elements = Enumerable.Empty<XElement>();
			if (_depth < 0) {
				var ancestor = e.Parents().ElementAtOrDefault(-1 * _depth - 1);
				if (ancestor != null) {
					elements = ancestor.Elements();
				}
			} else {
				elements = e.Elements();
				for (int i = 0; i < _depth; i++) {
					elements = elements.Where(e2 => !e2.IsTokenSet());
					elements = elements.Elements();
				}
			}
			return string.Join("/", elements.Select(e2 => e2.NameOrTokenText()));
		}

		public override string ToString() {
			var name = GetType().Name;
			var index = name.IndexOf("Extractor");
			return name.Substring(0, index) + "(" + _depth + ")";
		}

		protected bool Equals(ElementSequenceExtractor other) {
			return _depth == other._depth;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}
			return Equals((ElementSequenceExtractor)obj);
		}

		public override int GetHashCode() {
			return _depth;
		}
	}

	public class ElementSequenceExtractorRemovingLoneElements : PropertyExtractor<string> {
		private readonly int _depth;

		public ElementSequenceExtractorRemovingLoneElements(int depth) {
			_depth = depth;
		}

		public override string ExtractProperty(XElement e) {
			var elements = Enumerable.Empty<XElement>();
			if (_depth < 0) {
				var depth = -_depth;
				var ancestor = e;
				do {
					ancestor = ancestor.Parent;
					if (ancestor.Elements().Count() > 1) {
						depth--;
					}
				} while (depth > 0 && ancestor != null);
				if (ancestor != null) {
					elements = ancestor.Elements();
				}
			} else {
				elements = new[] { e };
				var depth = _depth + 1;
				while (depth > 0 && elements.Any()) {
					elements = elements.Where(e2 => !e2.IsTokenSet());
					if (elements.Elements().Count() > 1) {
						depth--;
					}
					elements = elements.Elements();
				}
			}
			return string.Join("/", elements.Select(e2 => e2.NameOrTokenText()));
		}

		public override string ToString() {
			var name = GetType().Name;
			var index = name.IndexOf("Extractor");
			return name.Substring(0, index) + "(" + _depth + ")";
		}

		protected bool Equals(ElementSequenceExtractorRemovingLoneElements other) {
			return _depth == other._depth;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}
			return Equals((ElementSequenceExtractorRemovingLoneElements)obj);
		}

		public override int GetHashCode() {
			return _depth;
		}
	}

	public class ElementSetExtractor : PropertyExtractor<IEnumerable<string>> {
		private readonly int _depth;

		public ElementSetExtractor(int depth) {
			_depth = depth;
		}

		public override IEnumerable<string> ExtractProperty(XElement e) {
			var elements = Enumerable.Empty<XElement>();
			if (_depth < 0) {
				var ancestor = e.Parents().ElementAtOrDefault(-1 * _depth - 1);
				if (ancestor != null) {
					elements = ancestor.Elements();
				}
			} else {
				elements = e.Elements();
				for (int i = 0; i < _depth; i++) {
					elements = elements.Where(e2 => !e2.IsTokenSet());
					elements = elements.Elements();
				}
			}
			return elements.Select(e2 => e2.NameOrTokenText());
		}

		public override string ToString() {
			var name = GetType().Name;
			var index = name.IndexOf("Extractor");
			return name.Substring(0, index) + "(" + _depth + ")";
		}

		protected bool Equals(ElementSetExtractor other) {
			return _depth == other._depth;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}
			return Equals((ElementSetExtractor)obj);
		}

		public override int GetHashCode() {
			return _depth;
		}
	}
}