using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;

namespace Occf.Languages.Python3.Operators.Selectors {
	public class Python3CompoundStatementSelector : Selector {
		private static readonly string[] StatementNames = {
			"if_stmt",
			"while_stmt",
			"for_stmt",
			"with_stmt",
		};

		public override IEnumerable<XElement> Select(XElement root) {
			return root.Descendants()
				.Where(e => StatementNames.Any(e.Name.LocalName.EndsWith));
		}
	}
}