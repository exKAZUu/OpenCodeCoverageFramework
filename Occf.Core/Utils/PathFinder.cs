#region License

// Copyright (C) 2009-2012 Kazunori Sakamoto
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.IO;
using System.Linq;
using Paraiba.IO;

namespace Occf.Core.Utils {
	public static class PathFinder {
		public static FileInfo FindCoverageInfoPath(DirectoryInfo dirInfo) {
			return FindPath(dirInfo, OccfNames.CoverageInfo);
		}

		public static FileInfo FindTestInfoPath(DirectoryInfo dirInfo) {
			return FindPath(dirInfo, OccfNames.TestInfo);
		}

		public static FileInfo FindCoverageDataPath(
				FileInfo covFileInfo, DirectoryInfo rootDirInfo) {
			if (covFileInfo.SafeExists()) {
				return covFileInfo;
			}
			return FindPath(rootDirInfo, OccfNames.CoverageData);
		}

		private static FileInfo FindPath(DirectoryInfo dirInfo, string pattern) {
			if (!dirInfo.SafeExists()) {
				return null;
			}
			var path = Directory.EnumerateFiles(dirInfo.FullName, pattern)
					.OrderByDescending(p => p.Count(c => c == Path.PathSeparator))
					.FirstOrDefault();
			return path != null ? new FileInfo(path) : null;
		}
	}
}