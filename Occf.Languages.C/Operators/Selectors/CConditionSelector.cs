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

using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;

namespace Occf.Languages.C.Operators.Selectors {
	public class CConditionSelector : ConditionSelector {
		private static readonly string[] TargetNames = {
				"logical_or_expression",
				"logical_and_expression",
		};

		private static readonly string[] ParentNames = {
				"logical_or_expression",
				"logical_and_expression",
				"primary_expression",
		};

		protected override bool IsConditionalTerm(XElement element) {
			return TargetNames.Contains(element.Name.LocalName);
		}

		protected override bool IsAllowableParent(XElement element) {
			return element.Elements().Count() == 1 ||
			       ParentNames.Contains(element.Name.LocalName);
		}
	}
}