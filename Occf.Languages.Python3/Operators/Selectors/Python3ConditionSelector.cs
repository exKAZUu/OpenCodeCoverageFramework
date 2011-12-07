using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;

namespace Occf.Languages.Python3.Operators.Selectors {
	public class Python3ConditionSelector : ConditionSelector {
		private static readonly string[] TargetNames = {
			"or_test",
			"and_test",
		};

		private static readonly string[] ParentNames = {
			"atom",
		};

		protected override bool IsConditionalTerm(XElement element) {
			return TargetNames.Contains(element.Name.LocalName);
		}

		protected override bool IsAllowableParent(XElement element) {
			return element.Elements().Count() == 1 ||
			       ParentNames.Contains(element.Name.LocalName);
		}
	}
}