using System.Linq;
using System.Xml.Linq;
using Occf.Core.Operators.Selectors;

namespace Occf.Languages.Java.Operators.Selectors {
	public class JavaConditionSelector : ConditionSelector {
		private static readonly string[] TargetNames = {
			"conditionalOrExpression", "conditionalAndExpression",
		};

		private static readonly string[] ParentNames = {
			"conditionalOrExpression", "conditionalAndExpression", "parExpression",
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