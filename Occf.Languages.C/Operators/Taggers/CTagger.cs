﻿using System.Xml.Linq;
using Occf.Core.Operators.Taggers;

namespace Occf.Languages.C.Operators.Taggers {
	public class CTagger : Tagger {
		public override string Tag(XElement elements) {
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