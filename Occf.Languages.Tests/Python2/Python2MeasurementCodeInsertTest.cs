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

using NUnit.Framework;
using Occf.Core.Manipulators;
using Occf.Core.Utils;

namespace Occf.Languages.Tests.Python2 {
	public class Python2MeasurementCodeInsertTest {
		public Python2MeasurementCodeInsertTest() {
			OccfGlobal.SaveCurrentState();
		}

		[Test]
		[TestCase("Block1.py")]
		[TestCase("Block2.py")]
		[TestCase("Block3.py")]
		public void Should_Insert_Measurement_Code_In_Python2_Code(string fileName) {
			var profile = LanguageSupports.GetCoverageModeByClassName("Python2");
			CodeInsertTest.VerifyCodeInsertion(profile, fileName);
		}
	}
}