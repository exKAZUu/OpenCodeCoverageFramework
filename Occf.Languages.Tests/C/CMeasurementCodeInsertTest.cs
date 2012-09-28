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
using Occf.Core.Profiles;
using Occf.Core.Utils;

namespace Occf.Languages.Tests.C {
	public class CMeasurementCodeInsertTest {
		public CMeasurementCodeInsertTest() {
			OccfGlobal.SaveCurrentDirectory();
		}

		[Test]
        [TestCase("mul_mv.c")]
        [TestCase("mul_mv2.c")]
        [TestCase("mersenne.c")]
        [TestCase("multi.h")]
        //[TestCase("bubblesort.c")]
        //[TestCase("quicksort_p.c")]
        [TestCase("Block1.c")]
		[TestCase("Block2.c")]
		[TestCase("Block3.c")]
		public void Should_Insert_Measurement_Code_In_C_Code(string fileName) {
			var profile = CoverageProfiles.GetCoverageProfileByClassName("C");
			CodeInsertTest.VerifyCodeInsertion(profile, fileName);
		}
	}
}