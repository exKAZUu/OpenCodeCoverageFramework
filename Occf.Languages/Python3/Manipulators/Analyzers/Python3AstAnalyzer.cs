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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Manipulators.Analyzers;

namespace Occf.Languages.Python3.Manipulators.Analyzers {
	public class Python3AstAnalyzer : AstAnalyzer<Python3AstAnalyzer> {
		private static readonly string[] TargetNames = {
				"or_test",
				"and_test",
		};

		private static readonly string[] ParentNames = {
				"atom",
		};

		private static readonly string[] StatementNames = {
				"if_stmt",
				"while_stmt",
				"for_stmt",
				"with_stmt",
		};

		public override IEnumerable<XElement> FindFunctions(XElement root) {
			throw new NotImplementedException();
		}

		public override string GetFunctionName(XElement functionElement) {
			throw new NotImplementedException();
		}

		public IEnumerable<XElement> FindCompoundStatements(XElement root) {
			return root.Descendants()
					.Where(e => StatementNames.Any(e.Name.LocalName.EndsWith));
		}

		public override IEnumerable<XElement> FindStatements(XElement root) {
			return root
					.Descendants()
					.Where(
							e =>
									e.Name.LocalName == "small_stmt" || e.Name.LocalName == "compound_stmt");
		}

		public override IEnumerable<XElement> FindVariableInitializers(XElement root) {
			return Enumerable.Empty<XElement>();
		}

		public override IEnumerable<XElement> FindBranches(XElement root) {
			return root.Descendants("if_stmt")
					.SelectMany(e => e.Elements("test"));
		}

		protected override bool IsConditionalTerm(XElement element) {
			return TargetNames.Contains(element.Name.LocalName);
		}

		protected override bool IsAvailableParent(XElement element) {
			return element.Elements().Count() == 1 ||
					ParentNames.Contains(element.Name.LocalName);
		}

		public override IEnumerable<XElement> FindSwitches(XElement root) {
			return Enumerable.Empty<XElement>();
		}

		public override IEnumerable<XElement> FindCaseLabelTails(XElement root) {
			return Enumerable.Empty<XElement>();
		}

		public override IEnumerable<XElement> FindForeach(XElement root) {
			return Enumerable.Empty<XElement>();
		}

		public override IEnumerable<XElement> FindForeachHead(XElement foreachElement) {
			return Enumerable.Empty<XElement>();
		}

		public override IEnumerable<XElement> FindForeachTail(XElement foreachElement) {
			return Enumerable.Empty<XElement>();
		}

		public override IEnumerable<XElement> FindTestCases(XElement root) {
			return Enumerable.Empty<XElement>();
		}
	}
}