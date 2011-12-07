using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Java.Operators.Selectors {
	public class JavaStatementSelector : Selector {
		public override IEnumerable<XElement> Select(XElement root) {
			return root.Descendants("statement")
				.Where(e => {
					// ブロック自身は意味を持たないステートメントで、中身だけが必要なので除外
					if (e.FirstElement().Name.LocalName == "block")
						return false;
					// ラベル自身は意味を持たないステートメントで、中身だけが必要なので除外
					var second = e.NthElementOrDefault(1);
					if (second != null && second.Value == ":") {
						return false;
					}
					if (e.FirstElement().Value == ";") {
						return false;
					}
					return true;
				});
		}
	}
}