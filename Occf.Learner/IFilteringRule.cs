using System.Collections.Generic;
using System.Xml.Linq;

namespace Occf.Learner {
	public interface IFilteringRule {
		IEnumerable<XElement> Filter(IEnumerable<XElement> targets);
		int CountRemovableTargets(IEnumerable<XElement> targets);
		bool RuleEquals(IFilteringRule rule);
	}
}