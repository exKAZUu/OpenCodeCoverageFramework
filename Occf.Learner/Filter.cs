using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Occf.Learner.Core {
	public interface IFilter {
		string ElementName { get; }
		bool IsAcceptable(XElement target);
		IEnumerable<XElement> Select(IEnumerable<XElement> targets);
		int CountRemovableTargets(IEnumerable<XElement> targets);
		bool TryUpdate(IFilter rule);
	}

	public abstract class Filter<TProperty, TExtractedProperty> : IFilter {
		public string ElementName { get; private set; }
		protected ISet<TProperty> Properties;
		protected readonly IPropertyExtractor<TExtractedProperty> Extractor;

		protected Filter(
				string elementName, ISet<TProperty> properties, IPropertyExtractor<TExtractedProperty> extractor) {
			ElementName = elementName;
			Properties = properties;
			Extractor = extractor;
		}

		public abstract bool IsAcceptable(XElement target);

		public IEnumerable<XElement> Select(IEnumerable<XElement> targets) {
			return targets.Where(IsAcceptable);
		}

		public int CountRemovableTargets(IEnumerable<XElement> targets) {
			return targets.Count() - Select(targets).Count();
		}

		public bool TryUpdate(IFilter rule) {
			if (GetType() != rule.GetType()) {
				return false;
			}
			var ruleWithSet = (Filter<TProperty, TExtractedProperty>)rule;
			if (ElementName != rule.ElementName || !Equals(Extractor, ruleWithSet.Extractor)) {
				return false;
			}
			Properties = ruleWithSet.Properties;
			return true;
		}

		public override string ToString() {
			var name = GetType().Name;
			var index = name.IndexOf("Filter");
			if (Extractor != null) {
				return ElementName + " " + name.Substring(0, index) + " " + Extractor +
				       " [" + String.Join(",", Properties.Select(e => e.ToString())) + "]";
			} else {
				return ElementName + " " + name.Substring(0, index);
			}
		}
	}
}