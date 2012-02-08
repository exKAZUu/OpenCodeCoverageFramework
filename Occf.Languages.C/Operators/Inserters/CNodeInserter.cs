using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core.Antlr;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.XmlToCodes;
using Code2Xml.Languages.C.CodeToXmls;
using Code2Xml.Languages.C.XmlToCodes;
using Occf.Core.Operators.Inserters;
using Occf.Core.TestInfos;
using Occf.Languages.C.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.C.Operators.Inserters {
	public class CNodeInserter : AntlrNodeInserter<CParser> {
		private readonly CSwitchSelector _switchSelector = new CSwitchSelector();
		private readonly CCaseLabelTailSelector _caseLabelTailSelector = new CCaseLabelTailSelector();

		protected override string MethodPrefix {
			get { return ""; }
		}

		protected override AntlrCodeToXml<CParser> CodeToXml {
			get { return CCodeToXml.Instance; }
		}

		protected override XmlToCode XmlToCode {
			get { return CXmlToCode.Instance; }
		}

		protected override Func<CParser, XAstParserRuleReturnScope> ParseStatementFunc {
			get { return p => p.statement(); }
		}

		public override void InsertImport(XElement target) {
			var ast = new XElement("TOKEN", "#include \"covman.h\"\r\n");
			target.AddFirst(ast);
		}

		public override void SupplementDefaultCase(XElement root) {
			var targets = GetLackingDefaultCaseNodes(root);
			foreach (var target in targets) {
				var node = CCodeToXml.Instance.Generate(
						"default:", p => p.labeled_statement());
				target.AddAfterSelf(node);
			}
		}

		public override void SupplementDefaultConstructor(XElement root) {
		}

		protected override IEnumerable<XElement> GetLackingBlockNodes(XElement root) {
			var loops = CElements.Loop(root)
					.Select(CElements.LoopProcess);
			var selections = root.Descendants("selection_statement")
					.SelectMany(CElements.SelectionProcesses);

			return loops.Concat(selections);
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

		public override TestCase InsertTestCaseId(
				XElement target, int id, string relativePath) {
			throw new NotImplementedException();
		}
	}
}