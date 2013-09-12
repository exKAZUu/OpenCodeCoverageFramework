using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Paraiba.Linq;

namespace Occf.Learner {
	public class MustNotHaveRule<T> : FilteringRule<T> {
		private readonly Func<XElement, IEnumerable<T>> _selector;

		public MustNotHaveRule(ISet<T> set, Func<XElement, IEnumerable<T>> selector) : base(set) {
			_selector = selector;
		}

		public override IEnumerable<XElement> Filter(IEnumerable<XElement> targets) {
			return targets.Where(e => !_selector(e).IsIntersect(Set));
		}
	}
}