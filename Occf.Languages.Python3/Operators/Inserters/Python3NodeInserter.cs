using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Languages.Python3.CodeToXmls;
using Code2Xml.Languages.Python3.XmlToCodes;
using Occf.Core.Operators.Inserters;
using Occf.Core.TestInfos;

namespace Occf.Languages.Python3.Operators.Inserters {
	public class Python3NodeInserter : NodeInserter {
		public override void InsertImport(XElement target) {
			var code = "import CoverageWriter";
			var ast = Python3CodeToXml.Instance.Generate(code);
			target.AddFirst(ast);
		}

		protected override IEnumerable<XElement> CreateStatementNode(
				XElement target, int id, int value, ElementType type) {
			var code = "CoverageWriter.WriteStatement(" + id + "," + (int)type + ","
			           + value + ");";
			if (target.Name.LocalName == "small_stmt") {
				yield return Python3CodeToXml.Instance.Generate(code)
						.Descendants(target.Name)
						.First();
				yield return new XElement("SEMI", ";");
			}
			else {
				var node = Python3CodeToXml.Instance.Generate(code)
						.Descendants("simple_stmt")
						.First();
				yield return node;
			}
		}

		public override void InsertPredicate(
				XElement target, int id, ElementType type) {
			var oldcode = Python3XmlToCode.Instance.Generate(target);
			var code = "CoverageWriter.WritePredicate(" + id + "," + (int)type + ","
			           + oldcode + ")";
			var node = Python3CodeToXml.Instance.Generate(code)
					.Descendants(target.Name)
					.First();
			target.AddBeforeSelf(node);
			target.Remove();
		}

		public override void InsertInitializer(
				XElement target, int id, ElementType type) {}

		public override void SupplementBlock(XElement root) {}

		public override void SupplementDefaultCase(XElement root) {}

		public override void SupplementDefaultConstructor(XElement root) {}

		public override TestCase InsertTestCaseId(
				XElement target, int id, string relativePath) {
			throw new NotImplementedException();
		}
	}
}