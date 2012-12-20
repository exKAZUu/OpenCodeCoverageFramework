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
using Occf.Core.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.C.Operators.Selectors {
	public class CCaseLabelTailSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			return root.Elements("labeled_statement")
					.Where(
							label => label.FirstElement().Value == "case"
							         || label.FirstElement().Value == "default")
					// 親以外にswitch文がでてきてはいけない（直接の子供以外のラベルを除去）
					.Where(
							label => !label.ParentsWhile(root)
							          		.Any(
							          				parent => parent.Name.LocalName == "selection_statement"
							          				          && parent.FirstElement().Value == "switch"))
					// コロンを選択する
					.Select(label => label.Elements().First(e => e.Value == ":"));
		}
	}
}