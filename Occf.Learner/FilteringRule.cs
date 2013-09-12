using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Occf.Learner {
	public abstract class FilteringRule<T> : IFilteringRule {
		protected readonly ISet<T> Set;

		protected FilteringRule(ISet<T> set) {
			Set = set;
		}

		public abstract IEnumerable<XElement> Filter(IEnumerable<XElement> targets);

		public int CountRemovableTargets(IEnumerable<XElement> targets) {
			return targets.Count() - Filter(targets).Count();
		}

		public bool RuleEquals(IFilteringRule rule) {
			var ruleWithSet = rule as FilteringRule<T>;
			return ruleWithSet != null && Set.SetEquals(ruleWithSet.Set);
		}

		public override string ToString() {
			return GetType().Name + " [" + String.Join(",", Set.Select(e => e.ToString())) + "]";
		}
	}
}