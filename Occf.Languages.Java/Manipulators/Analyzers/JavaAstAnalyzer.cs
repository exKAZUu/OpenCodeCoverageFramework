#region License

// Copyright (C) 2009-2013 Kazunori Sakamoto
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
using Occf.Core.Manipulators.Analyzers;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Java.Manipulators.Analyzers {
	public class JavaAstAnalyzer : AstAnalyzer<JavaAstAnalyzer> {
		private static readonly string[] TargetNames = {
				"conditionalOrExpression", "conditionalAndExpression",
		};

		private static readonly string[] ParentNames = {
				"conditionalOrExpression", "conditionalAndExpression",
				"parExpression",
		};

		public override IEnumerable<XElement> FindFunctions(XElement root) {
			return root.Descendants()
					.Where(e => e.Name() == "methodDeclaration");
		}

		public override string GetFunctionName(XElement functionElement) {
			return functionElement.Element("IDENTIFIER").Value;
		}

		public override IEnumerable<XElement> FindStatements(XElement root) {
			return root.Descendants("statement")
					.Where(
							e => {
								// ブロック自身は意味を持たないステートメントで、中身だけが必要なので除外
								if (e.FirstElement().Name() == "block") {
									return false;
								}
								// ラベルはループ文に付くため，ラベルの中身は除外
								var second = e.Parent.NthElementOrDefault(1);
								if (second != null && second.Value == ":" && e.Parent.Name() == "statement") {
									return false;
								}
								if (e.FirstElement().Value == ";") {
									return false;
								}
								return true;
							});
		}
        /*
		public override XElement GetBaseElementForStatement(XElement statement) {
			var val = statement.FirstElement().Value;
			if (val == "if" || val == "while" || val == "do") {
				return statement.Element("parExpression");
			}
			return statement;
		}
        */

		public override IEnumerable<XElement> FindVariableInitializers(XElement root) {
			return Enumerable.Empty<XElement>();
			//return root.Descendants("variableDeclarator")
			//        .SelectMany(e => e.Elements("variableInitializer"))
			//        .SelectMany(e => e.Elements("expression"));
		}

		public override IEnumerable<XElement> FindBranches(XElement root) {
			//var eqs = root.Descendants("equalityExpression")
			//        .Where(e => e.Elements().Count() > 1)
			//        .Where(e => e.Parents().Any())
			var ifWhileDoWhiles = root.Descendants("statement")
					.Where(
							e =>
									e.FirstElement().Value == "if"
											|| e.FirstElement().Value == "while"
											|| e.FirstElement().Value == "do")
					.Select(e => e.Element("parExpression"))
					.Select(e => e.NthElement(1));
			var fors = root.Descendants("forstatement")
					.Where(e => e.NthElement(2).Name() != "variableModifiers")
					.SelectMany(e => e.Elements("expression"));
			var ternaries = root.Descendants("conditionalExpression")
					.Where(e => e.Elements().Count() > 1)
					.Select(e => e.FirstElement());
			return ifWhileDoWhiles.Concat(fors).Concat(ternaries);
		}

		protected override bool IsConditionalTerm(XElement element) {
			return TargetNames.Contains(element.Name());
		}

		protected override bool IsAvailableParent(XElement element) {
			return element.Elements().Count() == 1 ||
					ParentNames.Contains(element.Name());
		}

		public override IEnumerable<XElement> FindSwitches(XElement root) {
			return root.Descendants("statement")
					.Where(e => e.FirstElement().Value == "switch");
		}

		public override IEnumerable<XElement> FindCaseLabelTails(XElement root) {
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

		public override IEnumerable<XElement> FindForeach(XElement root) {
			var foreachBlocks = root.Descendants("forstatement")
					.Where(e => e.NthElement(2).Name() == "variableModifiers");
			return foreachBlocks;
		}

		public override IEnumerable<XElement> FindForeachHead(XElement foreachElement) {
			yield return foreachElement.Descendants("block").First().FirstElement();
		}

		public override IEnumerable<XElement> FindForeachTail(XElement foreachElement) {
			yield return foreachElement.Descendants("block").First().LastElement();
		}

		public override IEnumerable<XElement> FindTestCases(XElement root) {
			return root.Descendants()
					.Where(e => e.Name() == "methodDeclaration")
					.Where(
							e => {
								var name = e.NthElement(2).Value;
								if (name.StartsWith("test")) {
									return true;
								}
								var annotation = e.FirstElement().Element("annotation");
								if (annotation != null && annotation.Value == "@Test") {
									return true;
								}
								return false;
							});
		}
	}
}