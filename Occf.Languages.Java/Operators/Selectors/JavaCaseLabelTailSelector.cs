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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Java.Operators.Selectors {
	public class JavaCaseLabelTailSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			Contract.Requires(root.Name == "switch");
			/*
			statement 
				:   'switch' parExpression '{' switchBlockStatementGroups '}'
				;

			switchBlockStatementGroups 
				:   (switchBlockStatementGroup )*
				;

			switchBlockStatementGroup 
				:
					switchLabel
					(blockStatement
					)*
				;

			switchLabel 
				:   'case' expression ':'
				|   'default' ':'
				;
			*/
			return root.Element("switchBlockStatementGroups")
					.Elements("switchBlockStatementGroup")
					.SelectMany(e => e.Elements("switchLabel"))
					// コロンを選択する
					.Select(e => e.LastElement());
		}
	}
}