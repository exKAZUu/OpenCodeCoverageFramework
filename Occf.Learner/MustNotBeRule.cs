using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Occf.Learner {
	public class MustNotBeRule<T> : FilteringRule<T> {
		private readonly Func<XElement, T> _selector;

		public MustNotBeRule(ISet<T> set, Func<XElement, T> selector) : base(set) {
			_selector = selector;
		}

		public override IEnumerable<XElement> Filter(IEnumerable<XElement> targets) {
			return targets.Where(e => !Set.Contains(_selector(e)));
		}
	}
}