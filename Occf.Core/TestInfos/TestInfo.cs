using System;
using System.Collections.Generic;

namespace Occf.Core.TestInfos {
	[Serializable]
	public class TestInfo {
		public string BasePath;
		public int Id;
		public List<TestCase> TestCases;

		public TestInfo(int id, string basePath) {
			Id = id;
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