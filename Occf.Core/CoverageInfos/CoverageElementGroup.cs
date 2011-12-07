using System;
using System.Collections.Generic;
using System.Linq;
using Occf.Core.CodeInformations;

namespace Occf.Core.CoverageInfos {
	[Serializable]
	public class CoverageElementGroup : ICoverageElement {
		public CoverageElementGroup(CoverageElement parentElement,
		                            IList<CoverageElement> targets) {
			ParentElement = parentElement;
			Targets = targets;
		}

		public CoverageElement ParentElement { get; private set; }

		public IList<CoverageElement> Targets { get; private set; }

		public CoverageState StateChildrenOrParent {
			get {
				return Targets.Count > 0
				       	? Targets.Aggregate(CoverageState.Done, (s, t) => s & t.State)
				       	: ParentElement.State;
			}
		}

		#region ICoverageElement Members

		public CodePosition Position {
			get { return ParentElement.Position; }
		}

		public CoverageState State {
			get { return Targets.Aggregate(ParentElement.State, (s, t) => s & t.State); }
		}

		public string Tag {
			get { return ParentElement.Tag; }
		}

		public string RelativePath {
			get { return ParentElement.RelativePath; }
		}

		public void Execute(CoverageElement lastElement, CoverageState state) {
		}

		#endregion
	}
}