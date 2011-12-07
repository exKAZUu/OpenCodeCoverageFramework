using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Taggers;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Python3.Operators.Taggers {
	public class Python3Tagger : Tagger {
		public override string Generate(XElement elements) {
			var tag = "";
			var classNodes = elements.Ancestors()
				.Where(e => e.Name.LocalName == "classdef");
			foreach (var classNode in classNodes) {
				var node = classNode.NthElementOrDefault(1);
				if (node == null)
					continue;
				tag += node.Value + '>';
			}
			return tag;
		}
	}
}