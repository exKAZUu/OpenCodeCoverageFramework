using Occf.Core.CodeInformations;

namespace Occf.Core.CoverageInfos {
	public interface ICoverageElement {
		CodePosition Position { get; }
		CoverageState State { get; }
		string Tag { get; }
		string RelativePath { get; }
	}
}