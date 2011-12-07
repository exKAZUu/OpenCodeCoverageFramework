using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Paraiba.Linq;
using Paraiba.Xml.Linq;

namespace Occf.Core.Operators.Selectors {
	public abstract class ConditionSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			var targetParents = root.Descendants()
				.Where(IsConditionalTerm)
				.Where(e => e.Elements().Count() >= 3)
				.Where(e => e.ParentsWhile(root)
				            	.All(IsAllowableParent)
				);
			var targets = targetParents
				.SelectMany(e => e.Elements().OddIndexElements());

			// 他の項の要素を含まない項の要素のみを抽出
			// a == b && (a == c || a == d) => a == b, a == c, a == d
			var atomicTargets = targets.Independents().ToList();

			// XML要素の位置でソーティング
			atomicTargets.Sort((e1, e2) => e1.IsBefore(e2) ? -1 : 1);

			return atomicTargets;
		}

		protected abstract bool IsConditionalTerm(XElement element);
		protected abstract bool IsAllowableParent(XElement element);
	}
}