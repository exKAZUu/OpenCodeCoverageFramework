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

namespace Occf.Languages.Java.Operators.Selectors {
	public class JavaStatementSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			return root.Descendants("statement")
					.Where(
							e => {
								// ブロック自身は意味を持たないステートメントで、中身だけが必要なので除外
								if (e.FirstElement().Name.LocalName == "block") {
									return false;
								}
								// ラベル自身は意味を持たないステートメントで、中身だけが必要なので除外
								var second = e.NthElementOrDefault(1);
								if (second != null && second.Value == ":") {
									return false;
								}
								if (e.FirstElement().Value == ";") {
									return false;
								}
								return true;
							});
		}
	}
}