using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Occf.Core.CoverageCode;
using Occf.Core.CoverageInformation;
using Occf.Core.Tests;
using Occf.Tools.Core;

namespace Occf.Languages.Tests {
	public static class CodeInsertTest {
		public static void VerifyCodeInsertion(ScriptCoverageProfile profile, string fileName) {
			var info = new CoverageInfo(Fixture.GetCoverageInputPath(),
				profile.Name, SharingMethod.SharedMemory);
			var inPath = Path.Combine(Fixture.GetCoverageInputPath(), fileName);
			var code = CoverageCodeGenerator.GetCoveragedCode(new FileInfo(inPath), info,
				profile);

			var expPath = Path.Combine(Fixture.GetCoverageExpectationPath(), fileName);
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
