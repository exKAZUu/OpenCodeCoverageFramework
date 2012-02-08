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
				XElement target, int id, int value, ElementType type) {
			return MethodPrefix + "WriteTestCase(" + id + "," + (int)type + "," + value + ");";
		}

		protected virtual string CreateStatementCoverageCode(
				XElement target, int id, int value, ElementType type) {
			return MethodPrefix + "WriteStatement(" + id + "," + (int)type + "," + value + ");";
		}

		protected virtual Tuple<string, string> CreatePredicateCoverageCode(
				XElement target, int id, ElementType type) {
			return
					Tuple.Create(
							MethodPrefix + "WritePredicate(" + id + "," + (int)type + ",",
							")");
		}

		protected virtual Tuple<string, string, string> CreateInitializerCoverageCode(
				XElement target, int id, ElementType type) {
			var stmt = CreateStatementCoverageCode(target, id, 2, type);
			return
					Tuple.Create(
							stmt.Substring(0, stmt.Length - 1) + " ? (",
							") : (",
							")");
		}

		protected override IEnumerable<XElement> CreateStatementNode(
				XElement target, int id, int value, ElementType type) {
			var code = CreateStatementCoverageCode(target, id, value, type);
			var node = CodeToXml.Generate(code, ParseStatementFunc);
			yield return node;
		}

		public override void InsertPredicate(
				XElement target, int id, ElementType type) {
			var code = CreatePredicateCoverageCode(target, id, type);
			var node = AntlrNodeGenerator.GenerateWrappedNode(
					target,
					CodeToXml, XmlToCode,
					code.Item1, code.Item2);
			target.AddBeforeSelf(node);
			target.Remove();
		}

		public override void InsertInitializer(
				XElement target, int id, ElementType type) {
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