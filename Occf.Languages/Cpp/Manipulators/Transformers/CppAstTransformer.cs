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
using Occf.Core.Manipulators.Transformers;
using Occf.Core.TestInformation;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Cpp.Manipulators.Transformers {
	public class CppAstTransformer : AntlrAstTransformer {
		protected override string MethodPrefix {
			get { return ""; }
		}

		protected override Tuple<string, string> CreatePredicateCoverageCode(
				XElement target, long id, ElementType type) {
			return Tuple.Create("(WritePredicate(" + id + "," + (int)type + ",", "))");
		}

		public override void InsertImport(XElement target) {
			var ast = new XElement("TOKEN", "#include \"covman.h\"\r\n");
			target.AddFirst(ast);
		}

		public override void SupplementBlock(XElement root) {
			SupplementBlock(root, "block", "{", "}");
		}

		public override void SupplementDefaultCase(XElement root) {
			// TODO: Implement
		}

		public override void SupplementDefaultConstructor(XElement root) {
			// TODO: Implement
		}

		protected override IEnumerable<XElement> FindLackingBlockNodes(XElement root) {
			var ifElseNodes = root.Descendants("then")
					.Select(e => e.FirstElement());
			var whileNodes = root.Descendants("while")
					.Select(e => e.LastElement());
			var doWhileNodes = root.Descendants("do")
					.Select(e => e.FirstElement());
			var forNodes = root.Descendants("for")
					.Select(e => e.LastElement());
			return ifElseNodes.Concat(whileNodes).Concat(doWhileNodes).Concat(forNodes);
		}

		private IEnumerable<XElement> GetLackingDefaultCaseNodes(XElement root) {
			// TODO: Implement
			yield break;
		}

		public override TestCase InsertTestCaseId(
				XElement target, long id, string relativePath) {
			// TODO: Implement
			throw new NotImplementedException();
		}
	}
}