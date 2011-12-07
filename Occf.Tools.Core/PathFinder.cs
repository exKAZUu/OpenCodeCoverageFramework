using System.IO;
using System.Linq;

namespace Occf.Tools.Core {
	public static class PathFinder {
		private static string FindPath(string fileOrDirPath, string pattern) {
			if (Directory.Exists(fileOrDirPath)) {
				return Directory.EnumerateFiles(fileOrDirPath, pattern)
						.OrderByDescending(p => p.Count(c => c == Path.PathSeparator))
						.FirstOrDefault();
			}
			if (File.Exists(fileOrDirPath)) {
				return fileOrDirPath;
			}
			return null;
		}

		public static string FindCoverageInfoPath(string fileOrDirPath) {
			return FindPath(fileOrDirPath, Names.CoverageInfo);
		}

		public static string FindTestInfoPath(string fileOrDirPath) {
			return FindPath(fileOrDirPath, Names.TestInfo);
		}

		public static string FindCoverageDataPath(string fileOrDirPath) {
			return FindPath(fileOrDirPath, Names.CoverageData);
		}
	}
}