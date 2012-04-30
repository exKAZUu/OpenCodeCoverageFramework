﻿#region License

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
	public class CBranchSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			var ifs = root.Descendants("selection_statement")
					.Where(e => e.FirstElement().Value == "if")
					.Select(e => e.NthElement(2));
			var whilesAndDoWhiles = root.Descendants("iteration_statement")
					.Select(e => e.FirstElement().Value == "for" ? e.NthElement(3) : e)
					.SelectMany(e => e.Elements("expression"));
			var ternaries = root.Descendants("conditional_expression")
					.Where(e => e.Elements().Count() > 1)
					.Select(e => e.FirstElement());
			return ifs.Concat(whilesAndDoWhiles).Concat(ternaries);
		}
	}
}