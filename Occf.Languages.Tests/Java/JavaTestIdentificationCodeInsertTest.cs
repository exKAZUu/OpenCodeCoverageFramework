using System.IO;
using NUnit.Framework;
using Occf.Core.CoverageCode;
using Occf.Core.TestInfos;
using Occf.Core.Tests;
using Occf.Tools.Core;

namespace Occf.Languages.Tests.Java {
	[TestFixture]
	public class JavaTestIdentificationCodeInsertTest {
		[Test]
		[TestCase("FizzBuzzTest.java")]
		public void Should_Insert_Measurement_Code_In_JUnit4_Code(string fileName) {
			var info = new TestInfo(0, Fixture.GetTestInputPath());
			var inPath = Path.Combine(Fixture.GetTestInputPath(), fileName);
			var code = CoverageCodeGenerator.GetIdentifiedTest(new FileInfo(inPath), info,
				ScriptCoverageProfile.Load("Java"));

			var expPath = Path.Combine(Fixture.GetTestExpectationPath(), fileName);
			using (var reader = new StreamReader(expPath)) {
				var expected = reader.ReadToEnd();
				try {
					Assert.That(code, Is.EqualTo(expected));
				} catch {
					var path = Fixture.GetOutputPath(fileName);
					File.WriteAllText(path, code);
					throw;
				}
			}
		}
	}
}