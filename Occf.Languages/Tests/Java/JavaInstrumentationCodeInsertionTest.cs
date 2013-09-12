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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Occf.Core.CoverageInformation;
using Occf.Core.Manipulators;
using Occf.Core.Manipulators.Transformers;
using Occf.Core.Tests;
using Paraiba.IO;

namespace Occf.Languages.Tests.Java {
	[TestFixture]
	public class JavaInstrumentationCodeInsertionTest {
		private static IEnumerable<TestCaseData> TestCases {
			get {
				var names = new[] {
					"Block1.java",
					"Block2.java",
					"Block3.java",
					"Condition.java",
					"Simple.java",
					"GenerateCommand.java",
					"TestCodeGenerator.java",
				};
				return names.Select(name => new TestCaseData(name));
			}
		}

		[Test, TestCaseSource("TestCases")]
		public void VerifyInstrumentationCode(string fileName) {
			var profile = LanguageSupports.GetCoverageModeByClassName("Java");
			CodeInsertTest.VerifyCodeInsertion(profile, fileName);
		}

		[Test, TestCaseSource("TestCases")]
		public void InsertInstrumentationCode(string fileName) {
			var profile = LanguageSupports.GetCoverageModeByClassName("Java");
			CodeInsertTest.InsertInstrumentationCode(profile, fileName);

			var info = new CoverageInfo(
					Fixture.GetCoverageInputPath(), profile.Name, SharingMethod.SharedMemory);
			var inPath = Path.Combine(Fixture.GetCoverageInputPath(), fileName);
			var codeFile = new FileInfo(inPath);

			var relativePath = ParaibaPath.GetRelativePath(codeFile.FullName, info.BasePath);
			var ast = profile.CodeToXml.GenerateFromFile(codeFile.FullName);

			File.WriteAllText(Fixture.GetOutputPath(fileName) + ".xml", ast.ToString());

			// 測定用コードの埋め込み
			var path = relativePath;
			CodeTransformer.InstrumentStatementAndPredicate(info, ast, profile, path);

			File.WriteAllText(Fixture.GetOutputPath(fileName) + ".modified.xml", ast.ToString());
		}
	}
}