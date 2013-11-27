using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core;
using Paraiba.Xml.Linq;

namespace Occf.Learner.Core.Tests {
	public class JavaBranchExperiment : LearningExperiment {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public JavaBranchExperiment(IList<string> allPaths)
				: base(allPaths, "expression") {}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			var ifWhileDoWhile = new[] { "if", "while", "do" };
			var ifWhileDoWhileConds = ast.Descendants("statement")
					.Where(e => ifWhileDoWhile.Contains(e.FirstElementOrDefault().SafeValue()))
					.Select(e => e.Element("parExpression").NthElement(1));
			var forConds = ast.Descendants("forstatement")
					.Where(e => e.Elements().Count(e2 => e2.TokenText() == ";") >= 2)
					.Select(e => e.Element("expression"));
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
			return preConds;
			//return ifWhileDoWhileConds.Concat(forConds).Concat(preConds);
		}
	}
}