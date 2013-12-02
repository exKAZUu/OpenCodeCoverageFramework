using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core;
using Paraiba.Xml.Linq;

namespace Occf.Learner.Core.Tests {
	public class JavaIfExperiment : LearningExperiment {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public JavaIfExperiment() : base("expression") {}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			return ast.Descendants("statement")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "if")
					.Select(e => e.Element("parExpression").NthElement(1));
		}
	}

	public class JavaWhileExperiment : LearningExperiment {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public JavaWhileExperiment() : base("expression") {}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			return ast.Descendants("statement")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "while")
					.Select(e => e.Element("parExpression").NthElement(1));
		}
	}

	public class JavaDoWhileExperiment : LearningExperiment {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public JavaDoWhileExperiment() : base("expression") {}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			return ast.Descendants("statement")
					.Where(e => e.FirstElementOrDefault().SafeValue() == "do")
					.Select(e => e.Element("parExpression").NthElement(1));
		}
	}

	public class JavaForExperiment : LearningExperiment {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public JavaForExperiment() : base("expression") {}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			return ast.Descendants("forstatement")
					.Where(e => e.Elements().Count(e2 => e2.TokenText() == ";") >= 2)
					.Select(e => e.Element("expression"));
		}
	}

	public class JavaPreconditionsExperiment : LearningExperiment {
		protected override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public JavaPreconditionsExperiment() : base("expression") {}

		protected override IEnumerable<XElement> GetAcceptedElements(XElement ast) {
			return ast.Descendants("expression")
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
		}
	}
}