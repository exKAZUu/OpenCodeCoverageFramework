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

namespace Occf.Languages.Python3.Operators.Selectors {
	public class Python3CompoundStatementSelector : Selector {
		private static readonly string[] StatementNames = {
				"if_stmt",
				"while_stmt",
				"for_stmt",
				"with_stmt",
		};

		public override IEnumerable<XElement> Select(XElement root) {
			return root.Descendants()
					.Where(e => StatementNames.Any(e.Name.LocalName.EndsWith));
		}
	}
}