using System;
using System.Xml.Linq;
using Code2Xml.Core.Position;
using Occf.Core.Operators.Taggers;

namespace Occf.Core.CoverageInformation {
	[Serializable]
	public class CoverageElement : ICoverageElement {
		public CoverageElement(string relativePath, XElement node, Tagger tagger) {
			RelativePath = relativePath;
			Position = CodePositionAnalyzer.Create(node);
			var tag = relativePath.Replace('\\', '>') + '>' + tagger.Tag(node);
			Tag = tag.EndsWith(">") ? tag : tag + ">";
		}

		#region ICoverageElement Members

		public CodePosition Position { get; private set; }

		public CoverageState State { get; protected set; }

		public string Tag { get; private set; }

		public string RelativePath { get; private set; }

		public virtual void UpdateState(CoverageState state) {
			State |= state;
		}

		#endregion
	}
}