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
using System.Xml.XPath;
using Paraiba.Xml.Linq;

namespace Occf.Core.Operators.Selectors {
	public class FilteredSelectorByXPath : Selector {
		private readonly Selector _selector;

		public FilteredSelectorByXPath(Selector selector) {
			_selector = selector;
		}

		public string WithParentsExpression { get; set; }

		public string WithoutParentsExpression { get; set; }

		public string WithChildrenExpression { get; set; }

		public string WithoutChildrenExpression { get; set; }

		public string WithBeforeSiblingExpression { get; set; }

		public string WithoutBigSiblingExpression { get; set; }

		public string WithAfterSiblingExpression { get; set; }

		public string WithoutLittleSiblingExpression { get; set; }

		public override IEnumerable<XElement> Select(XElement root) {
			//var result = _aspect.PointCut(root);
			//if (!string.IsNullOrWhiteSpace(_withParentsExpression)) {
			//    var parents = root
			//        .XPathSelectElements(_withParentsExpression).ToList();
			//    result = result.Where(e => parents.Any(p => p.Contains(e)));
			//}
			var result = string.IsNullOrWhiteSpace(WithParentsExpression)
			             		? _selector.Select(root)
			             		: root.XPathSelectElements(WithParentsExpression)
			             		  		.SelectMany(r => _selector.Select(r));
			if (!string.IsNullOrWhiteSpace(WithoutParentsExpression)) {
				var parents = root
						.XPathSelectElements(WithoutParentsExpression).ToList();
				result = result.Where(e => !parents.Any(p => p.Contains(e)));
			}

			if (!string.IsNullOrWhiteSpace(WithChildrenExpression)) {
				var children = root
						.XPathSelectElements(WithChildrenExpression).ToList();
				result = result.Where(e => children.Any(c => e.Contains(c)));
			}
			if (!string.IsNullOrWhiteSpace(WithoutChildrenExpression)) {
				var children = root
						.XPathSelectElements(WithoutChildrenExpression).ToList();
				result = result.Where(e => !children.Any(c => e.Contains(c)));
			}

			if (!string.IsNullOrWhiteSpace(WithBeforeSiblingExpression)) {
				var siblings = root
						.XPathSelectElements(WithBeforeSiblingExpression).ToList();
				result = result.Where(e => e.ElementsBeforeSelf().Any(siblings.Contains));
			}
			if (!string.IsNullOrWhiteSpace(WithoutBigSiblingExpression)) {
				var siblings = root
						.XPathSelectElements(WithoutBigSiblingExpression).ToList();
				result = result.Where(e => !e.ElementsBeforeSelf().Any(siblings.Contains));
			}

			if (!string.IsNullOrWhiteSpace(WithAfterSiblingExpression)) {
				var siblings = root
						.XPathSelectElements(WithAfterSiblingExpression).ToList();
				result = result.Where(e => e.ElementsAfterSelf().Any(siblings.Contains));
			}
			if (!string.IsNullOrWhiteSpace(WithoutLittleSiblingExpression)) {
				var siblings = root
						.XPathSelectElements(WithoutLittleSiblingExpression).ToList();
				result = result.Where(e => !e.ElementsAfterSelf().Any(siblings.Contains));
			}

			return result;
		}
	}
}