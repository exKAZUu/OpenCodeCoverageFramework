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
				CoverageMode mode, TestInfo info, FileInfo fullPath,
				DirectoryInfo outDirPath) {
			string relativePath;
			var code = GetIdentifiedTest(fullPath, info, mode, out relativePath);
			return WriteCode(relativePath, outDirPath, code);
		}

		public static string GetIdentifiedTest(
				FileInfo testFile, TestInfo info, CoverageMode mode) {
			string relativePath;
			return GetIdentifiedTest(testFile, info, mode, out relativePath);
		}

		public static string GetIdentifiedTest(
				FileInfo testFile, TestInfo info, CoverageMode mode,
				out string relativePath) {
			relativePath = XPath.GetRelativePath(testFile.FullName, info.BasePath);
			var ast = mode.CodeToXml.GenerateFromFile(testFile.FullName);

			// テストケース識別用コードの埋め込み
			TestCaseInserter.Insert(
					info, ast, mode.TestCaseLabelTailSelector,
					mode.NodeInserter, relativePath);

			// コード生成
			return mode.XmlToCode.Generate(ast);
		}

		public static string WriteCoveragedCode(
				CoverageMode mode, CoverageInfo info, FileInfo codeFile,
				DirectoryInfo outDir) {
			string relativePath;
			var code = GetCoveragedCode(
					codeFile, info, mode, out relativePath);
			return WriteCode(relativePath, outDir, code);
		}

		public static string GetCoveragedCode(
				FileInfo codeFile, CoverageInfo info, CoverageMode mode) {
			string relativePath;
			return GetCoveragedCode(codeFile, info, mode, out relativePath);
		}

		public static string GetCoveragedCode(
				FileInfo codeFile, CoverageInfo info, CoverageMode mode,
				out string relativePath) {
			relativePath = XPath.GetRelativePath(codeFile.FullName, info.BasePath);
			var ast = mode.CodeToXml.GenerateFromFile(codeFile.FullName);
			var nodeIns = mode.NodeInserter;

			// 測定用コードの埋め込み
			var path = relativePath;
			CoverageInserter.InstrumentStatementAndPredicate(
					info, ast, mode, path);
			//CoverageInserter.InstrumentStatement(
			//        info, ast, CoverageMode, path);
			//CoverageInserter.InsertPredicate(
			//        info, ast, CoverageMode, path);

			// コード生成
			return mode.XmlToCode.Generate(ast);
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