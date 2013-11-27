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
			var ifConds = ast.Descendants("ifStatement")
					.Select(e => e.Element("expression"));
			var whileConds = ast.Descendants("whileStatement")
					.Select(e => e.Element("expression"));
			var doWhileConds = ast.Descendants("doWhileStatement")
					.Select(e => e.Element("expression"));
			var forConds = ast.Descendants("forStatement")
					.Select(e => e.Elements().First(e2 => e2.TokenText() == ";"))
					.Where(e => e.NextElement().Name() == "expression")
					.Select(e => e.NextElement());
			var preConds = ast.Descendants("callExpression")
					.Where(e => e.FirstElement().Value == "console.log")
					.Select(e => e.Element("arguments").Element("assignmentExpression"))
					.Where(e => e != null);
			return ifConds.Concat(whileConds).Concat(doWhileConds).Concat(forConds)/*.Concat(preConds)*/;
		}
	}

	public class JavaScriptConsoleLogExperiment : LearningExperiment {
		public JavaScriptConsoleLogExperiment(IList<string> allPaths)
				: base(allPaths, "assignmentExpression") {}

		protected override Processor Processor {
			get { return ProcessorLoader.JavaScriptUsingAntlr3; }
		}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			var preConds = ast.Descendants("callExpression")
					.Where(e => e.FirstElement().Value == "console.log")
					.Select(e => e.Element("arguments").Element("assignmentExpression"))
					.Where(e => e != null);
			return preConds;
		}
	}
}