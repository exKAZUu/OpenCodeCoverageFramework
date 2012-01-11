using System;

namespace Occf.Core.CoverageInformation {
	[Flags]
	public enum CoverageState {
		None = 0,
		FalseOnly = 1,
		TrueOnly = 2,
		Done = 3,
	}
}