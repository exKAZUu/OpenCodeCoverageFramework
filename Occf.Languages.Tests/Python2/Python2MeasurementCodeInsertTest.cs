using System.IO;
using NUnit.Framework;
using Occf.Core.CoverageCode;
using Occf.Core.Tests;
using Occf.Tools.Core;

namespace Occf.Languages.Tests.Python2 {
	public class Python2MeasurementCodeInsertTest {
		[Test]
		[TestCase("Block1.py")]
		[TestCase("Block2.py")]
		[TestCase("Block3.py")]
		public void Should_Insert_Measurement_Code_In_Python2_Code(string fileName) {
			var profile = ScriptCoverageProfile.Load("Python2");
			CodeInsertTest.VerifyCodeInsertion(profile, fileName);
		}
	}
}