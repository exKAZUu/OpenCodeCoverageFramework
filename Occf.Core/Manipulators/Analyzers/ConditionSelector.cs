#region License

// Copyright (C) 2009-2012 Kazunori Sakamoto
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

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
					.Where(
							e => e.ParentsWhile(root)
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