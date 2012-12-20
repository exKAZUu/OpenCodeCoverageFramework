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
	public class CAstTransformer : AntlrAstTransformer<CParser> {
		private readonly CSwitchSelector _switchSelector = new CSwitchSelector();

		private readonly CCaseLabelTailSelector _caseLabelTailSelector =
				new CCaseLabelTailSelector();

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

		public override void SupplementDefaultConstructor(XElement root) {}

		protected override IEnumerable<XElement> GetLackingBlockNodes(XElement root) {
			var loops = CElements.Loop(root)
					.Select(CElements.LoopProcess);
			var selections = root.Descendants("selection_statement")
					.SelectMany(CElements.SelectionProcesses);

			return loops.Concat(selections);
		}

		private IEnumerable<XElement> GetLackingDefaultCaseNodes(XElement root) {
			foreach (var switchNode in _switchSelector.Select(root)) {
				var last = _caseLabelTailSelector.Select(switchNode).LastOrDefault();
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