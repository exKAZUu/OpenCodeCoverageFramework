using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Taggers;

namespace Occf.Languages.Java.Operators.Taggers {
	public class JavaTagger : Tagger {
		public override string Generate(XElement elements) {
			var tag = "";
			var outerNodes = elements.Ancestors()
				.Where(e => e.Name.LocalName == "normalClassDeclaration" ||
				            e.Name.LocalName == "methodDeclaration");
			foreach (var outerNode in outerNodes) {
				var node = outerNode.Element("IDENTIFIER");
				if (node == null)
					continue;
				tag = node.Value + '>' + tag;
				if (outerNode.Name.LocalName == "normalClassDeclaration")
					tag = "class " + tag;
				else
					tag = "method " + tag;
			}
			return tag;
		}
	}
}