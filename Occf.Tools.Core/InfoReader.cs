using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Occf.Core.CoverageInformation;
using Occf.Core.TestInfos;

namespace Occf.Tools.Core {
	public static class InfoReader {
		public static CoverageInfo ReadCoverageInfo(
				FileInfo coverageInfoFile, BinaryFormatter formatter) {
			using (var fs = new FileStream(coverageInfoFile.FullName, FileMode.Open)) {
				return (CoverageInfo)formatter.Deserialize(fs);
			}
		}

		public static TestInfo ReadTestInfo(
				FileInfo testInfoFile, BinaryFormatter formatter) {
			using (var fs = new FileStream(testInfoFile.FullName, FileMode.Open)) {
				return (TestInfo)formatter.Deserialize(fs);
			}
		}
	}
}