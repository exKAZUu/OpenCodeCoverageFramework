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

using System.IO;
using Occf.Core.CoverageInformation;
using Occf.Core.Operators.Inserters;
using Occf.Core.Profiles;
using Occf.Core.TestInfos;
using Paraiba.IO;
using Paraiba.Text;

namespace Occf.Core.CoverageCode {
	public static class CoverageCodeGenerator {
		public static string WriteIdentifiedTest(
				CoverageProfile coverageProfile, TestInfo info, FileInfo fullPath,
				DirectoryInfo outDirPath) {
			string relativePath;
			var code = GetIdentifiedTest(
					fullPath, info, coverageProfile,
					out relativePath);
			return WriteCode(relativePath, outDirPath, code);
		}

		public static string GetIdentifiedTest(
				FileInfo testFile, TestInfo info, CoverageProfile coverageProfile) {
			string relativePath;
			return GetIdentifiedTest(testFile, info, coverageProfile, out relativePath);
		}

		public static string GetIdentifiedTest(
				FileInfo testFile, TestInfo info, CoverageProfile coverageProfile,
				out string relativePath) {
			relativePath = XPath.GetRelativePath(testFile.FullName, info.BasePath);
			var ast = coverageProfile.CodeToXml.GenerateFromFile(testFile.FullName);

			// テストケース識別用コードの埋め込み
			TestCaseInserter.Insert(
					info, ast, coverageProfile.TestCaseLabelTailSelector,
					coverageProfile.NodeInserter, relativePath);

			// コード生成
			return coverageProfile.XmlToCode.Generate(ast);
		}

		public static string WriteCoveragedCode(
				CoverageProfile coverageProfile, CoverageInfo info, FileInfo codeFile,
				DirectoryInfo outDir) {
			string relativePath;
			var code = GetCoveragedCode(
					codeFile, info, coverageProfile, out relativePath);
			return WriteCode(relativePath, outDir, code);
		}

		public static string GetCoveragedCode(
				FileInfo codeFile, CoverageInfo info, CoverageProfile coverageProfile) {
			string relativePath;
			return GetCoveragedCode(codeFile, info, coverageProfile, out relativePath);
		}

		public static string GetCoveragedCode(
				FileInfo codeFile, CoverageInfo info, CoverageProfile coverageProfile,
				out string relativePath) {
			relativePath = XPath.GetRelativePath(codeFile.FullName, info.BasePath);
			var ast = coverageProfile.CodeToXml.GenerateFromFile(codeFile.FullName);
			var nodeIns = coverageProfile.NodeInserter;

			// 測定用コードの埋め込み
			var path = relativePath;
			CoverageInserter.InstrumentStatementAndPredicate(
					info, ast, coverageProfile, path);
			//CoverageInserter.InstrumentStatement(
			//        info, ast, coverageProfile, path);
			//CoverageInserter.InsertPredicate(
			//        info, ast, coverageProfile, path);

			// コード生成
			return coverageProfile.XmlToCode.Generate(ast);
		}

		private static string WriteCode(
				string relativePath, DirectoryInfo outDir,
				string code) {
			var outPath = XPath.GetFullPath(relativePath, outDir.FullName);
			Directory.CreateDirectory(Path.GetDirectoryName(outPath));
			using (var writer = new StreamWriter(outPath, false, XEncoding.SJIS)) {
				writer.Write(code);
			}
			return outPath;
		}
	}
}