﻿#region License

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
using Occf.Core.Manipulators;
using Occf.Core.Manipulators.Transformers;
using Occf.Core.TestInformation;
using Paraiba.IO;
using Paraiba.Text;

namespace Occf.Core.Utils {
	public static class OccfCodeGenerator {
		public static string AnalyzeAndWriteIdentifiedTest(
				LanguageSupport support, TestInfo info, FileInfo fullPath, DirectoryInfo outDirPath) {
			string relativePath;
			var code = GetIdentifiedTest(fullPath, info, support, out relativePath);
			return WriteCode(relativePath, outDirPath, code);
		}

		public static string GetIdentifiedTest(FileInfo testFile, TestInfo info, LanguageSupport support) {
			string relativePath;
			return GetIdentifiedTest(testFile, info, support, out relativePath);
		}

		public static string GetIdentifiedTest(
				FileInfo testFile, TestInfo info, LanguageSupport support, out string relativePath) {
			relativePath = ParaibaPath.GetRelativePath(testFile.FullName, info.BasePath);
			var ast = support.CodeToXml.GenerateFromFile(testFile.FullName);

			// テストケース識別用コードの埋め込み
			CodeTransformer.InsertIntoTestCase(info, ast, support, relativePath);

			// コード生成
			return support.XmlToCode.Generate(ast);
		}

		public static string WriteCoveragedCode(
				LanguageSupport support, CoverageInfo info, FileInfo codeFile, DirectoryInfo outDir) {
			string relativePath;
			var code = GetCoveragedCode(codeFile, info, support, out relativePath);
			return WriteCode(relativePath, outDir, code);
		}

		public static string GetCoveragedCode(
				FileInfo codeFile, CoverageInfo info, LanguageSupport support) {
			string relativePath;
			return GetCoveragedCode(codeFile, info, support, out relativePath);
		}

		public static string GetCoveragedCode(
				FileInfo codeFile, CoverageInfo info, LanguageSupport support, out string relativePath) {
			relativePath = ParaibaPath.GetRelativePath(codeFile.FullName, info.BasePath);
			var ast = support.CodeToXml.GenerateFromFile(codeFile.FullName);

			// 測定用コードの埋め込み
			var path = relativePath;
			CodeTransformer.InstrumentStatementAndPredicate(info, ast, support, path);

			// コード生成
			return support.XmlToCode.Generate(ast);
		}

		private static string WriteCode(string relativePath, DirectoryInfo outDir, string code) {
			var outPath = ParaibaPath.GetFullPath(relativePath, outDir.FullName);
			Directory.CreateDirectory(Path.GetDirectoryName(outPath));
			using (var writer = new StreamWriter(outPath, false, XEncoding.SJIS)) {
				writer.Write(code);
			}
			return outPath;
		}
	}
}