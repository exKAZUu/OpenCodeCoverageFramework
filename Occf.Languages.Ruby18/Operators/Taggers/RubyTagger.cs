using System.Xml.Linq;
using Occf.Core.Operators.Taggers;

namespace Occf.Languages.Ruby18.Operators.Taggers {
	public class RubyTagger : Tagger {
		public override string Generate(XElement elements) {
			var tag = "";
			//var classNodes = elements.Parents()
			//    .Where(e => e.Name.LocalName == "classdef");
			//foreach (var classNode in classNodes) {
			//    var node = classNode.ElementAtOrDefault(1);
			//    if (node == null)
			//        continue;
			//    tag += node.Value + '>';
			//}
			return tag;
		}
	}
}