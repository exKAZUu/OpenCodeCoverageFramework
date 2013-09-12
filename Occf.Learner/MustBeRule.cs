using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Occf.Learner {
	public class MustBeRule<T> : FilteringRule<T> {
		private readonly Func<XElement, T> _selector;

		public MustBeRule(ISet<T> set, Func<XElement, T> selector) : base(set) {
			_selector = selector;
		}

		public override IEnumerable<XElement> Filter(IEnumerable<XElement> targets) {
			return targets.Where(e => Set.Contains(_selector(e)));
		}
	}
}