using System.IO;
using NUnit.Framework;
using Occf.Core.CoverageCode;
using Occf.Core.Tests;
using Occf.Tools.Core;

namespace Occf.Languages.Tests.Java {
	[TestFixture]
	public class JavaMeasurementCodeInsertTest {
		[Test]
		[TestCase("Block1.java")]
		[TestCase("Block2.java")]
		[TestCase("Block3.java")]
		[TestCase("Condition.java")]
		[TestCase("Simple.java")]
		public void Should_Insert_Measurement_Code_In_Java_Code(string fileName) {
			var profile = ScriptCoverageProfile.Load("Java");
			CodeInsertTest.VerifyCodeInsertion(profile, fileName);
		}
	}
}