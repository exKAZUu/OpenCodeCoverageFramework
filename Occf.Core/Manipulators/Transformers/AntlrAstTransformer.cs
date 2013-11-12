using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Paraiba.Xml.Linq;

namespace Occf.Core.Manipulators.Transformers {
	public abstract class AntlrAstTransformer : AstTransformer {
		protected abstract string MethodPrefix { get; }

		protected virtual string CreateTestCaseIdentifierCode(
				XElement target, long id, int value, ElementType type) {
			return MethodPrefix + "WriteTestCase(" + id + "," + (int)type + "," + value + ");";
		}

		protected virtual string CreateStatementCoverageCode(
				XElement target, long id, int value, ElementType type) {
			return MethodPrefix + "WriteStatement(" + id + "," + (int)type + "," + value + ");";
		}

		protected virtual Tuple<string, string> CreatePredicateCoverageCode(
				XElement target, long id, ElementType type) {
			return Tuple.Create(MethodPrefix + "WritePredicate(" + id + "," + (int)type + ",", ")");
		}

		protected virtual Tuple<string, string, string> CreateInitializerCoverageCode(
				XElement target, long id, ElementType type) {
			var stmt = CreateStatementCoverageCode(target, id, 2, type);
			return Tuple.Create(stmt.Substring(0, stmt.Length - 1) + " ? (", ") : (", ")");
		}

		protected override IEnumerable<XElement> CreateStatementNode(
				XElement target, long id, int value, ElementType type) {
			var code = CreateStatementCoverageCode(target, id, value, type);
			var node = new XElement("TOKEN", code);
			yield return node;
		}

		public override void InsertPredicate(XElement target, long id, ElementType type) {
			var code = CreatePredicateCoverageCode(target, id, type);
			target.AddBeforeSelf(new XElement("TOKEN", code.Item1));
			target.AddAfterSelf(new XElement("TOKEN", code.Item2));
		}

		public override void InsertInitializer(XElement target, long id, ElementType type) {
			var code = CreateInitializerCoverageCode(target, id, type);
			target.AddBeforeSelf(new XElement("TOKEN", code.Item1));
			target.AddAfterSelf(new XElement("TOKEN", code.Item2));
		}

		public override void InsertEqual(
				XElement target, XElement left, XElement right, long id, ElementType type) {
			var code = MethodPrefix + "WriteEqual(" + id + "," + (int)type + "," + left.Value + ","
			           + right.Value + ")";
			target.AddBeforeSelf(new XElement("TOKEN", code));
			target.Remove();
		}

		public override void InsertNotEqual(
				XElement target, XElement left, XElement right, long id, ElementType type) {
			var code = MethodPrefix + "WriteNotEqual(" + id + "," + (int)type + "," + left.Value + ","
			           + right.Value + ")";
			target.AddBeforeSelf(new XElement("TOKEN", code));
			target.Remove();
		}

		public override void InsertLessThan(
				XElement target, XElement left, XElement right, long id, ElementType type) {
			var code = MethodPrefix + "WriteLessThan(" + id + "," + (int)type + "," + left.Value + ","
			           + right.Value + ")";
			target.AddBeforeSelf(new XElement("TOKEN", code));
			target.Remove();
		}

		public override void InsertGraterThan(
				XElement target, XElement left, XElement right, long id, ElementType type) {
			var code = MethodPrefix + "WriteGraterThan(" + id + "," + (int)type + "," + left.Value + ","
			           + right.Value + ")";
			target.AddBeforeSelf(new XElement("TOKEN", code));
			target.Remove();
		}

		public void SupplementBlock(
				XElement root, string elementName, string begin, string end) {
			var nodes = FindLackingBlockNodes(root)
					.Where(e => e.Name.LocalName != elementName)
					.Where(e => e.FirstElementOrDefault(elementName) == null || e.Elements().Skip(1).Any());
			ReplaceSafely(
					nodes,
					node =>
							new XElement(elementName, new XElement("TOKEN", begin), node,
									new XElement("TOKEN", end)));
		}

		protected abstract IEnumerable<XElement> FindLackingBlockNodes(XElement root);
	}
}