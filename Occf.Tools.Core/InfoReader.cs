using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Occf.Core.CoverageInfos;
using Occf.Core.TestInfos;

namespace Occf.Tools.Core {
	public static class InfoReader {
		public static CoverageInfo ReadCoverageInfo(
				string path, BinaryFormatter formatter) {
			using (var fs = new FileStream(path, FileMode.Open)) {
				return (CoverageInfo)formatter.Deserialize(fs);
			}
		}

		public static TestInfo ReadTestInfo(
				string path, BinaryFormatter formatter) {
			using (var fs = new FileStream(path, FileMode.Open)) {
				return (TestInfo)formatter.Deserialize(fs);
			}
		}
	}
}