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

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Occf.Core.Utils;

namespace Occf.Core.TestInfos {
	[Serializable]
	public class TestInfo {
		/// <summary>
		/// A base path for relative paths in <see cref="TestCase"/> class.
		/// </summary>
		public string BasePath;

		/// <summary>
		/// A list of test case information.
		/// </summary>
		public List<TestCase> TestCases;

		public TestInfo(string basePath) {
			TestCases = new List<TestCase>();
			BasePath = basePath;
		}

		public void InitializeForStoringData(bool initialPassed) {
			foreach (var testCase in TestCases) {
				testCase.InitializeForStoringData(initialPassed);
			}
		}

		public static TestInfo ReadTestInfo(FileInfo infoFile) {
			return ReadTestInfo(infoFile, new BinaryFormatter());
		}

		public static TestInfo ReadTestInfo(FileInfo infoFile, BinaryFormatter formatter) {
			using (var fs = new FileStream(infoFile.FullName, FileMode.Open)) {
				return (TestInfo)formatter.Deserialize(fs);
			}
		}

		public static void WriteTestInfo(DirectoryInfo rootDir, TestInfo testInfo) {
			WriteTestInfo(rootDir, testInfo, new BinaryFormatter());
		}

		public static void WriteTestInfo(
				DirectoryInfo rootDir, TestInfo testInfo, BinaryFormatter formatter) {
			var testPath = Path.Combine(rootDir.FullName, OccfNames.TestInfo);
			using (var fs = new FileStream(testPath, FileMode.Create)) {
				formatter.Serialize(fs, testInfo);
			}
		}
	}
}