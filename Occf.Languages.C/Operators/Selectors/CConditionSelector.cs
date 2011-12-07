using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;

namespace Occf.Languages.C.Operators.Selectors {
	public class CConditionSelector : ConditionSelector {
		private static readonly string[] TargetNames = {
			"logical_or_expression",
			"logical_and_expression",
		};

		private static readonly string[] ParentNames = {
			"logical_or_expression",
			"logical_and_expression",
			"primary_expression",
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