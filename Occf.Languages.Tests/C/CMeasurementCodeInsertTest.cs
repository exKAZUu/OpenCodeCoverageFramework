using System.IO;
using NUnit.Framework;
using Occf.Core.CoverageCode;
using Occf.Core.Tests;
using Occf.Tools.Core;

namespace Occf.Languages.Tests.C {
	public class CMeasurementCodeInsertTest {
		[Test]
		[TestCase("Block1.c")]
		[TestCase("Block2.c")]
		[TestCase("Block3.c")]
		public void Should_Insert_Measurement_Code_In_C_Code(string fileName) {
			var profile = ScriptCoverageProfile.Load("C");
			CodeInsertTest.VerifyCodeInsertion(profile, fileName);
		}
	}
}