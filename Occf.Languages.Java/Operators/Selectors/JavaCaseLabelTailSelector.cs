using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Java.Operators.Selectors {
	public class JavaCaseLabelTailSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			return root.Descendants("switchLabel")
					// 親以外にswitch文がでてきてはいけない（直接の子供以外のラベルを除去）
					.Where(
							label => !label.ParentsWhile(root)
							          		.Any(
							          				parent => parent.Name.LocalName == "statement"
							          				          && parent.FirstElement().Value == "switch"))
					// コロンを選択する
					.Select(label => label.Elements().First(e => e.Value == ":"));
		}
	}
}