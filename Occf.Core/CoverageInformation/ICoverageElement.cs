using Occf.Core.CodeInformation;

namespace Occf.Core.CoverageInformation {
	public interface ICoverageElement {
		CodePosition Position { get; }
		CoverageState State { get; }
		string Tag { get; }
		string RelativePath { get; }
	}
}