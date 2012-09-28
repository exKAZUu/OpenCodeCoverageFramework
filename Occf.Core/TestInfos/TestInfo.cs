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

		public void InitializeForStoringData() {
			foreach (var testCase in TestCases) {
				testCase.InitializeForStoringData();
			}
		}
	}
}