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
		bool RuleEquals(IFilter rule);
	}

	public abstract class Filter<TProperty, TExtractedProperty> : IFilter {
		public string ElementName { get; private set; }
		protected readonly ISet<TProperty> Properties;
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

		public bool RuleEquals(IFilter rule) {
			var ruleWithSet = rule as Filter<TProperty, TExtractedProperty>;
			return ruleWithSet != null && Properties.SetEquals(ruleWithSet.Properties);
		}

		public override string ToString() {
			var name = GetType().Name;
			var index = name.IndexOf("Filter");
			return ElementName + " " + name.Substring(0, index) + ": " + Extractor +
			       " [" + String.Join(",", Properties.Select(e => e.ToString())) + "]";
		}
	}
}