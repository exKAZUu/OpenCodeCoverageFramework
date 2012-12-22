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
using Paraiba.IO;

namespace Occf.Core.Utils {
	public static class FileUtil {
		public static FileInfo GetCoverageInfo(DirectoryInfo dirInfo) {
			return GetFile(dirInfo, OccfNames.CoverageInfo);
		}

		public static FileInfo GetTestInfo(DirectoryInfo dirInfo) {
			return GetFile(dirInfo, OccfNames.TestInfo);
		}

		public static FileInfo GetCoverageData(
				FileInfo covFileInfo, DirectoryInfo rootDirInfo) {
			if (covFileInfo.SafeExists()) {
				return covFileInfo;
			}
			return GetFile(rootDirInfo, OccfNames.CoverageData);
		}

		private static FileInfo GetFile(DirectoryInfo dirInfo, string relativePath) {
			if (!dirInfo.SafeExists()) {
				return null;
			}
			var fileInfo = dirInfo.GetFile(relativePath);
			return fileInfo.Exists ? fileInfo : null;
		}
	}
}