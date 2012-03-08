using System.Collections.Generic;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.XmlToCodes;
using Occf.Core.Operators.Inserters;
using Occf.Core.Operators.Selectors;
using Occf.Core.Operators.Taggers;

namespace Occf.Core.CoverageProfile {
	public abstract class CoverageProfile {
		public abstract string FilePattern { get; }
		public abstract string Name { get; }

		public abstract CodeToXml CodeToXml { get; }
		public abstract XmlToCode XmlToCode { get; }

		public abstract NodeInserter NodeInserter { get; }

	    public abstract Selector FunctionSelector { get; }
	    public abstract Selector FunctionNameSelector { get; }

		public abstract Selector StatementSelector { get; }
		public abstract Selector InitializerSelector { get; }

		public abstract Selector BranchSelector { get; }
		public abstract Selector ConditionSelector { get; }
		public abstract Selector SwitchSelector { get; }
		public abstract Selector CaseLabelTailSelector { get; }
		public abstract Selector ForeachSelector { get; }
		public abstract Selector ForeachHeadSelector { get; }
		public abstract Selector ForeachTailSelector { get; }

		public abstract Selector TestCaseLabelTailSelector { get; }

		public abstract Tagger Tagger { get; }
		public abstract IEnumerable<string> LibraryNames { get; }
	}
}