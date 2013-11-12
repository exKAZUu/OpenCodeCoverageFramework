using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Manipulators.Transformers;
using Occf.Core.TestInformation;
using Occf.Languages.Java.Manipulators.Analyzers;
using Occf.Languages.Java.Manipulators.Taggers;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Java.Manipulators.Transformers {
	public class JavaAstTransformer : AntlrAstTransformer {
		protected override string MethodPrefix {
			get { return "net.exkazuu.CoverageWriter."; }
		}

		public override void InsertImport(XElement target) {}

		public override void SupplementBlock(XElement root) {
			SupplementBlock(root, "block", "{", "}");
		}

		public override void SupplementDefaultCase(XElement root) {
			var targets = FindLackingDefaultCaseNodes(root);
			foreach (var target in targets) {
				target.AddAfterSelf(new XElement("TOKEN", "default:"));
			}
		}

		private static IEnumerable<XElement> FindLackingDefaultCaseNodes(XElement root) {
			foreach (var switchNode in JavaAstAnalyzer.Instance.FindSwitches(root)) {
				// ケース文がないときは分岐していないと見なす
				var last = JavaAstAnalyzer.Instance.FindCaseLabelTails(switchNode).LastOrDefault();
				if (last != null && last.Parent.FirstElement().Value != "default") {
					yield return last.Parent.Parent;
				}
			}
		}

		public override void SupplementDefaultConstructor(XElement root) {
			throw new NotImplementedException();
		}

		public override TestCase InsertTestCaseId(XElement target, long id, string relativePath) {
			var testCase = new TestCase(
					relativePath, string.Join(".", JavaTagger.GetTag(target)), target);
			var blockElement = target.Element("block").NthElement(1);
			var code = CreateTestCaseIdentifierCode(target, id, 2, ElementType.TestCase);
			var node = new XElement("TOKEN", code);
			if (blockElement.Name.LocalName == "blockStatement") {
				blockElement.AddFirst(node);
			} else {
				blockElement.AddBeforeSelf(node);
			}
			return testCase;
		}

		protected override IEnumerable<XElement> FindLackingBlockNodes(XElement root) {
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

		private static void SupplementEmptyMethod(XElement root) {
			// TODO: This transformation causes compile errors due to unrechable code
			// e.g. return; return;
			var methods = root.Descendants("methodDeclaration")
					.Where(e => e.Elements().Any(e2 => e2.Value == "void"))
					.Select(e => e.Element("block"))
					.Where(e => e != null)
					.Where(e => !(e.Descendants("statement").Any() &&
					              e.Descendants("statement")
							              .Last()
							              .FirstElement()
							              .Value == "throw"))
					.Select(e => e.LastElement());
			var node = new XElement("TOKEN", "return;");
			foreach (var method in methods) {
				method.AddBeforeSelf(node);
			}
		}
	}
}