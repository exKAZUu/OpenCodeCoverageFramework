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
using System.Xml.Linq;
using Antlr.Runtime;
using Code2Xml.Core.Antlr;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.XmlToCodes;

namespace Occf.Core.Operators.Inserters {
	public abstract class AntlrNodeInserter<TParser> : NodeInserter
			where TParser : Parser, IAntlrParser {
		protected abstract string MethodPrefix { get; }
		protected abstract AntlrCodeToXml<TParser> CodeToXml { get; }
		protected abstract XmlToCode XmlToCode { get; }
		protected abstract Func<TParser, XAstParserRuleReturnScope> ParseStatementFunc { get; }

		protected virtual string CreateTestCaseIdentifierCode(
				XElement target, long id, int value, ElementType type) {
			return MethodPrefix + "WriteTestCase(" + id + "," + (int)type + "," + value
			       + ");";
		}

		protected virtual string CreateStatementCoverageCode(
				XElement target, long id, int value, ElementType type) {
			return MethodPrefix + "WriteStatement(" + id + "," + (int)type + "," + value
			       + ");";
		}

		protected virtual Tuple<string, string> CreatePredicateCoverageCode(
				XElement target, long id, ElementType type) {
			return
					Tuple.Create(
							MethodPrefix + "WritePredicate(" + id + "," + (int)type + ",",
							")");
		}

		protected virtual Tuple<string, string, string> CreateInitializerCoverageCode(
				XElement target, long id, ElementType type) {
			var stmt = CreateStatementCoverageCode(target, id, 2, type);
			return
					Tuple.Create(
							stmt.Substring(0, stmt.Length - 1) + " ? (",
							") : (",
							")");
		}

		protected override IEnumerable<XElement> CreateStatementNode(
				XElement target, long id, int value, ElementType type) {
			var code = CreateStatementCoverageCode(target, id, value, type);
			var node = CodeToXml.Generate(code, ParseStatementFunc);
			yield return node;
		}

		public override void InsertPredicate(
				XElement target, long id, ElementType type) {
			var code = CreatePredicateCoverageCode(target, id, type);
			var node = AntlrNodeGenerator.GenerateWrappedNode(
					target,
					CodeToXml, XmlToCode,
					code.Item1, code.Item2);
			target.AddBeforeSelf(node);
			target.Remove();
		}

		public override void InsertInitializer(
				XElement target, long id, ElementType type) {
			var code = CreateInitializerCoverageCode(target, id, type);
			var node = AntlrNodeGenerator.GenerateWrappedNode(
					target,
					CodeToXml,
					XmlToCode,
					code.Item1, code.Item2, code.Item3);
			target.AddBeforeSelf(node);
			target.Remove();
		}

		public override void SupplementBlock(XElement root) {
			var nodes = GetLackingBlockNodes(root);
			ReplaceSafely(
					nodes,
					node => AntlrNodeGenerator.GenerateBlock(node, CodeToXml));
		}

		protected abstract IEnumerable<XElement> GetLackingBlockNodes(XElement root);
			}
}