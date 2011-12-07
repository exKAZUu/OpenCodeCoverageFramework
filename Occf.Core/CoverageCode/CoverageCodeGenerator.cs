using System.IO;
using Occf.Core.CoverageInfos;
using Occf.Core.Extensions;
using Occf.Core.Operators.Inserters;
using Occf.Core.TestInfos;
using Paraiba.IO;
using Paraiba.Text;

namespace Occf.Core.CoverageCode {
	public static class CoverageCodeGenerator {
		public static string WriteIdentifiedTest(
				CoverageProfile coverageProfile,
				TestInfo info,
				string fullPath, string outDirPath) {
			string relativePath;
			var code = GetIdentifiedTest(
					fullPath, info, coverageProfile,
					out relativePath);
			return WriteCode(relativePath, outDirPath, code);
		}

		public static string GetIdentifiedTest(
				string fullPath, TestInfo info,
				CoverageProfile coverageProfile) {
			return GetIdentifiedTest(fullPath, info, coverageProfile, out fullPath);
		}

		public static string GetIdentifiedTest(
				string fullPath, TestInfo info,
				CoverageProfile coverageProfile,
				out string relativePath) {
			relativePath = XPath.GetRelativePath(fullPath, info.BasePath);
			var ast = coverageProfile.CodeToXml.GenerateFromFile(fullPath);

			// テストケース識別用コードの埋め込み
			TestCaseInserter.Insert(
					info, ast, coverageProfile.TestCaseLabelTailSelector,
					coverageProfile.NodeInserter, relativePath);

			// コード生成
			return coverageProfile.XmlToCode.Generate(ast);
		}

		public static string WriteCoveragedCode(
				CoverageProfile coverageProfile,
				CoverageInfo info,
				string fullPath, string outDirPath) {
			string relativePath;
			var code = GetCoveragedCode(
					fullPath, info, coverageProfile, out relativePath);
			return WriteCode(relativePath, outDirPath, code);
		}

		public static string GetCoveragedCode(
				string fullPath, CoverageInfo info,
				CoverageProfile coverageProfile) {
			return GetCoveragedCode(fullPath, info, coverageProfile, out fullPath);
		}

		public static string GetCoveragedCode(
				string fullPath, CoverageInfo info,
				CoverageProfile coverageProfile,
				out string relativePath) {
			relativePath = XPath.GetRelativePath(fullPath, info.BasePath);
			var ast = coverageProfile.CodeToXml.GenerateFromFile(fullPath);
			var nodeIns = coverageProfile.NodeInserter;

			// 測定用コードの埋め込み
			var path = relativePath;
			CoverageInserter.InsertStatementAndBranchAndCondition(
					info, ast, coverageProfile, path);
			//CoverageInserter.InsertStatement(
			//        info, ast, coverageProfile, path);
			//CoverageInserter.InsertBranchAndCondition(
			//        info, ast, coverageProfile, path);
			CoverageInserter.InsertImport(ast, nodeIns);

			// コード生成
			return coverageProfile.XmlToCode.Generate(ast);
		}

		private static string WriteCode(
				string relativePath, string outDirPath,
				string code) {
			var outPath = XPath.GetFullPath(relativePath, outDirPath);
			Directory.CreateDirectory(Path.GetDirectoryName(outPath));
			using (var writer = new StreamWriter(outPath, false, XEncoding.SJIS)) {
				writer.Write(code);
			}
			return outPath;
		}
	}
}