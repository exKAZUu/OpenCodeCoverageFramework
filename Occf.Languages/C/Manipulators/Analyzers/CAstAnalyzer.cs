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
using Occf.Core.Manipulators;
using Occf.Core.Manipulators.Analyzers;
using Paraiba.Xml.Linq;

namespace Occf.Languages.C.Manipulators.Analyzers {
	public class CAstAnalyzer : AstAnalyzer<CAstAnalyzer> {
		private static readonly string[] TargetNames = {
				"logical_or_expression",
				"logical_and_expression",
		};

		private static readonly string[] ParentNames = {
				"logical_or_expression",
				"logical_and_expression",
				"primary_expression",
		};

		public override IEnumerable<XElement> FindFunctions(XElement root) {
            // TODO: Implement
            yield break;
		}

		public override string GetFunctionName(XElement functionElement) {
			// TODO: Implement
			throw new NotImplementedException();
		}

		public override IEnumerable<XElement> FindStatements(XElement root) {
			return root.Descendants("statement")
					.Where(
							e => e.FirstElement().Name.LocalName != "labeled_statement"
									&& e.FirstElement().Name.LocalName != "compound_statement");
		}

		public override IEnumerable<XElement> FindVariableInitializers(XElement root) {
			// TODO: Fix
			//return root.Descendants("initializer")
			//        .Where(e => e.Parent.Name.LocalName != "initializer_list")
			//        .SelectMany(e => e.Elements("assignment_expression"));
			return Enumerable.Empty<XElement>();
		}

		public override IEnumerable<XElement> FindBranches(XElement root) {
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

		protected override bool IsConditionalTerm(XElement element) {
			return TargetNames.Contains(element.Name.LocalName);
		}

		protected override bool IsAvailableParent(XElement element) {
			return element.Elements().Count() == 1 ||
					ParentNames.Contains(element.Name.LocalName);
		}

	    public override Tuple<XElement, XElement, ComparatorType> GetComparedElements(XElement root) {
	        throw new NotImplementedException();
	    }

	    public override IEnumerable<XElement> FindSwitches(XElement root) {
			return root.Descendants("selection_statement")
					.Where(e => e.FirstElement().Value == "switch");
		}

		public override IEnumerable<XElement> FindCaseLabelTails(XElement root) {
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

		public override IEnumerable<XElement> FindForeach(XElement root) {
            // TODO: Implement
            yield break;
		}

		public override IEnumerable<XElement> FindForeachHead(XElement foreachElement) {
            // TODO: Implement
            yield break;
		}

		public override IEnumerable<XElement> FindForeachTail(XElement foreachElement) {
            // TODO: Implement
            yield break;
		}

		public override IEnumerable<XElement> FindTestCases(XElement root) {
            // TODO: Implement
            yield break;
		}
	}
}