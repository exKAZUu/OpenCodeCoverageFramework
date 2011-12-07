using System;

namespace Occf.Core.CoverageInfos {
	[Flags]
	public enum CoverageState {
		None = 0,
		TrueOnly = 1,
		FalseOnly = 2,
		Done = 3,
	}
}