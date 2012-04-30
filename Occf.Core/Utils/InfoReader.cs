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
using System.Runtime.Serialization.Formatters.Binary;
using Occf.Core.CoverageInformation;
using Occf.Core.TestInfos;

namespace Occf.Core.Utils {
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