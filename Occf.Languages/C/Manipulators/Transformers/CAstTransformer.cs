using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Manipulators.Transformers;
using Occf.Core.TestInformation;
using Occf.Languages.C.Manipulators.Analyzers;
using Paraiba.Xml.Linq;

namespace Occf.Languages.C.Manipulators.Transformers {
	public class CAstTransformer : AntlrAstTransformer {
		protected override string MethodPrefix {
			get { return ""; }
		}

		public override void InsertImport(XElement target) {
			var ast = new XElement("TOKEN", "#include \"covman.h\"\r\n");
			target.AddFirst(ast);
		}

		public override void SupplementBlock(XElement root) {
			SupplementBlock(root, "compound_statement", "{", "}");
		}

		public override void SupplementDefaultCase(XElement root) {
			var targets = GetLackingDefaultCaseNodes(root);
			foreach (var target in targets) {
				var node = new XElement("TOKEN", "default:");
				target.AddAfterSelf(node);
			}
		}

		public override void SupplementDefaultConstructor(XElement root) {}

		protected override IEnumerable<XElement> FindLackingBlockNodes(XElement root) {
			var loops = CElements.Loop(root)
					.Select(CElements.LoopProcess);
			var selections = root.Descendants("selection_statement")
					.SelectMany(CElements.SelectionProcesses);

			return loops.Concat(selections);
		}

		private IEnumerable<XElement> GetLackingDefaultCaseNodes(XElement root) {
			foreach (var switchNode in CAstAnalyzer.Instance.FindSwitches(root)) {
				var last = CAstAnalyzer.Instance.FindCaseLabelTails(switchNode).LastOrDefault();
				// ケース文がないときは分岐していないと見なす
				if (last != null && last.Parent.FirstElement().Value != "default") {
					yield return last.Parent;
				}
			}
		}

		public override TestCase InsertTestCaseId(
				XElement target, long id, string relativePath) {
			throw new NotImplementedException();
		}
	}
}