using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core;
using Paraiba.Xml.Linq;

namespace Occf.Learner.Core.Tests {
	public class JavaScriptBranchExperiment : LearningExperiment {
		public JavaScriptBranchExperiment(IList<string> allPaths)
				: base(allPaths, "expression") {}

		protected override Processor Processor {
			get { return ProcessorLoader.JavaScriptUsingAntlr3; }
		}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			var ifConds = ast.Descendants("statement")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "ifStatement")
					.Select(e => e.NthElement(1).NthElement(1));
			var preConds = ast.Descendants("expression")
					.Where(
							e => {
								var primary = e.SafeParent().SafeParent().SafeParent().SafeParent();
								if (primary.SafeName() != "primary") {
									return false;
								}
								if (primary.NthElementOrDefault(0).SafeValue() != "Preconditions") {
									return false;
								}
								if (primary.NthElementOrDefault(2).SafeValue() != "checkArgument") {
									return false;
								}
								return true;
							});
			return ifConds.Concat(preConds);
		}
	}
}