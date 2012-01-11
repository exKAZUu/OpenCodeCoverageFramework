using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core.Antlr;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.XmlToCodes;
using Code2Xml.Languages.Java.CodeToXmls;
using Code2Xml.Languages.Java.XmlToCodes;
using Occf.Core.Operators.Inserters;
using Occf.Core.TestInfos;
using Occf.Languages.Java.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Java.Operators.Inserters {
	public class JavaNodeInserter : AntlrNodeInserter<JavaParser> {
		private readonly JavaSwitchSelector _switchSelector = new JavaSwitchSelector();
		private readonly JavaCaseLabelTailSelector _caseLabelTailSelector = new JavaCaseLabelTailSelector();

		protected override string MethodPrefix {
			get { return "jp.ac.waseda.cs.washi.CoverageWriter."; }
		}

		protected override AntlrCodeToXml<JavaParser> CodeToXml {
			get { return JavaCodeToXml.Instance; }
		}

		protected override XmlToCode XmlToCode {
			get { return JavaXmlToCode.Instance; }
		}

		protected override Func<JavaParser, XParserRuleReturnScope> ParseStatementFunc {
			get { return p => p.statement(); }
		}

		public override void InsertImport(XElement target) {}

		public override void SupplementDefaultCase(XElement root) {
			var targets = GetLackingDefaultCaseNodes(root);
			foreach (var target in targets) {
				var node = JavaCodeToXml.Instance.Generate(
						"default:", p => p.switchLabel());
				target.AddAfterSelf(node);
			}
		}

		private IEnumerable<XElement> GetLackingDefaultCaseNodes(XElement root) {
			foreach(var switchNode in _switchSelector.Select(root)) {
				var last = _caseLabelTailSelector.Select(switchNode).LastOrDefault();
				// ケース文がないときは分岐していないと見なす
				if (last != null && last.Parent.FirstElement().Value != "default") {
					yield return last.Parent;
				}
			}
		}

		public override void SupplementDefaultConstructor(XElement root) {
			throw new NotImplementedException();
		}

		public override TestCase InsertTestCaseId(
				XElement target, int id, string relativePath) {
			var testCase = new TestCase(relativePath, target.NthElement(2).Value, target);
			var blockElement = target.Element("block").NthElement(1);
			var code = CreateTestCaseIdentifierCode(target, id, 2, ElementType.TestCase);
			var node = JavaCodeToXml.Instance.Generate(code, p => p.statement());
			if (blockElement.Name.LocalName == "blockStatement") {
				blockElement.AddFirst(node);
			} else {
				blockElement.AddBeforeSelf(node);
			}
			return testCase;
		}

		protected override IEnumerable<XElement> GetLackingBlockNodes(XElement root) {
			var methods = root.Descendants("methodDeclaration")
					.Where(e => e.Elements().Any(e2 => e2.Value == "void"))
					.Select(e => e.Element("block"))
					.Where(e => e != null)
					.Where(e => !(e.Descendants("statement").Any() && e.Descendants("statement").Last().FirstElement().Value == "throw"))
					.Select(e => e.LastElement());
			var node = JavaCodeToXml.Instance.Generate("return;", p => p.statement());
			foreach (var method in methods) {
				method.AddBeforeSelf(node);
			}

			var ifs = JavaElements.If(root)
					.SelectMany(JavaElements.IfAndElseProcesses);
			var whiles = JavaElements.While(root)
					.Select(JavaElements.WhileProcess);
			var dos = JavaElements.DoWhile(root)
					.Select(JavaElements.DoWhileProcess);
			var fors = JavaElements.For(root)
					.Select(JavaElements.ForProcess);

			return ifs.Concat(whiles)
					.Concat(dos)
					.Concat(fors);
		}
	}
}