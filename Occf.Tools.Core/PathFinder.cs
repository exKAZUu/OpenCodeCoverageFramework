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

namespace Occf.Tools.Core {
	public static class PathFinder {
		public static FileInfo FindCoverageInfoPath(FileSystemInfo fileOrDirInfo) {
			return FindPath(fileOrDirInfo, Names.CoverageInfo);
		}

		public static FileInfo FindTestInfoPath(FileSystemInfo fileOrDirInfo) {
			return FindPath(fileOrDirInfo, Names.TestInfo);
		}

		public static FileInfo FindCoverageDataPath(
				FileInfo covFileInfo, DirectoryInfo rootDirInfo) {
			if (!covFileInfo.SafeExists()) {
				covFileInfo = FindPath(covFileInfo, Names.CoverageData);
			}
			if (!covFileInfo.SafeExists()) {
				covFileInfo = FindPath(rootDirInfo, Names.CoverageData);
			}
			return covFileInfo;
		}

		private static FileInfo FindPath(FileSystemInfo fileOrDirInfo, string pattern) {
			if (fileOrDirInfo.SafeDirectoryExists()) {
				var path = Directory.EnumerateFiles(fileOrDirInfo.FullName, pattern)
						.OrderByDescending(p => p.Count(c => c == Path.PathSeparator))
						.FirstOrDefault();
				return path != null ? new FileInfo(path) : null;
			}
			return fileOrDirInfo.SafeFileExists()
			       		? new FileInfo(fileOrDirInfo.FullName) : null;
		}
	}
}