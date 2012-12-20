using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Occf.Languages.Java.Operators;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Java.Manipulators.Analyzers
{
	public class JavaAstAnalyzer : AstAnalyzer<JavaAstAnalyzer>
	{
		private static readonly string[] TargetNames = {
				"conditionalOrExpression", "conditionalAndExpression",
		};

		private static readonly string[] ParentNames = {
				"conditionalOrExpression", "conditionalAndExpression",
				"parExpression",
		};

		public override IEnumerable<XElement> FindFunctions(XElement root) {
			return root.Descendants()
					.Where(e => e.Name.LocalName == "methodDeclaration");
		}

		public override string GetFunctionName(XElement functionElement) {
			return functionElement.Element("IDENTIFIER").Value;
		}

		public override IEnumerable<XElement> FindStatements(XElement root) {
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

		public override IEnumerable<XElement> FindVariableInitializers(XElement root) {
			return root.Descendants("variableDeclarator")
					.SelectMany(e => e.Elements("variableInitializer"))
					.SelectMany(e => e.Elements("expression"));
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
					.Where(
							e =>
							e.NthElement(2).Name.LocalName
							!= "variableModifiers")
					.SelectMany(e => e.Elements("expression"));
			var ternaries = root.Descendants("conditionalExpression")
					.Where(e => e.Elements().Count() > 1)
					.Select(e => e.FirstElement());
			return ifWhileDoWhiles.Concat(fors).Concat(ternaries);
		}

		protected override bool IsConditionalTerm(XElement element) {
			return TargetNames.Contains(element.Name.LocalName);
		}

		protected override bool IsAvailableParent(XElement element) {
			return element.Elements().Count() == 1 ||
			       ParentNames.Contains(element.Name.LocalName);
		}

		public override IEnumerable<XElement> FindSwitches(XElement root) {
			return root.Descendants("statement")
					.Where(e => e.FirstElement().Value == "switch");
		}

		public override IEnumerable<XElement> FindCaseLabelTails(XElement root) {
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

		public override IEnumerable<XElement> FindForeach(XElement root) {
			var foreachBlocks = root.Descendants("forstatement")
					.Where( e => e.NthElement(2).Name.LocalName == "variableModifiers");
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
					.Where(e => e.Name.LocalName == "methodDeclaration")
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
