using System;
using System.Xml.Linq;
using Occf.Core.Operators.Taggers;

namespace Occf.Core.CoverageInfos {
	[Serializable]
	public class LoopCoverageElement : CoverageElement {
		private readonly CoverageElement _lastElement;

		public LoopCoverageElement(
				string relativePath, XElement node, Tagger tagger,
				CoverageElement lastElement)
				: base(relativePath, node, tagger) {
			_lastElement = lastElement;
		}

		#region ICoverageElement Members

		public override void UpdateState(CoverageElement lastElement, CoverageState state) {
			if (state != CoverageState.FalseOnly || _lastElement == lastElement) {
				State |= state;
			}
		}

		#endregion
	}
}