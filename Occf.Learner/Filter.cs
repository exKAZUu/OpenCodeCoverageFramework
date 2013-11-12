using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Occf.Learner.Core {
	public interface IFilter {
		string ElementName { get; }
		int LivingTime { get; }
		int PropertiesCount { get; }
		bool IsAcceptable(XElement target);
		IEnumerable<XElement> Select(IEnumerable<XElement> targets);
		int CountRemovableTargets(XElement ast);
		int CountRemovableTargets(IEnumerable<XElement> targets);
		bool TryUpdateProperties(IFilter rule);
	}

	public abstract class Filter<TProperty, TExtractedProperty> : IFilter {
		public string ElementName { get; private set; }

		public int LivingTime { get; private set; }

		public int PropertiesCount {
			get { return Properties == null ? 0 : Properties.Count; }
		}

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

		public int CountRemovableTargets(XElement ast) {
			return CountRemovableTargets(ast.Descendants(ElementName));
		}

		public int CountRemovableTargets(IEnumerable<XElement> targets) {
			return targets.Count() - Select(targets).Count();
		}

		public bool TryUpdateProperties(IFilter rule) {
			if (GetType() != rule.GetType()) {
				return false;
			}
			var ruleWithSet = (Filter<TProperty, TExtractedProperty>)rule;
			if (ElementName != rule.ElementName || !Equals(Extractor, ruleWithSet.Extractor)) {
				return false;
			}
			if (Properties == ruleWithSet.Properties || Properties.SetEquals(ruleWithSet.Properties)) {
				LivingTime++;
			} else {
				Properties = ruleWithSet.Properties;
			}
			Properties = ruleWithSet.Properties;
			return true;
		}

		public override string ToString() {
			var name = GetType().Name;
			var index = name.IndexOf("Filter");
			var head = "t(" + LivingTime + ") " + ElementName + " " + name.Substring(0, index);
			if (Extractor != null) {
				return head + " " + Extractor + " " +
				       Properties.Count + ": [" + String.Join(",", Properties.Select(e => e.ToString())) + "]";
			} else {
				return head;
			}
		}
	}
}