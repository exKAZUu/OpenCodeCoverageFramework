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
using System.Xml.Linq;
using Code2Xml.Core.Position;
using Paraiba.IO;

namespace Occf.Core.Manipulators.Transformers {
	/// <summary>
	///   A class to instrument test coverage.
	/// </summary>
	public abstract class Instrumenter {
		/// <summary>
		///   Registers a file with the specified <c>FileInfo</c> instance and returns the file id.
		/// </summary>
		/// <param name="fileInfo"> A <c>FileInfo</c> instance to be instrumented. </param>
		/// <returns> The file id. </returns>
		protected abstract long RegisterFile(FileInfo fileInfo);

		/// <summary>
		///   Registers a function with the specified name and returns the function id.
		/// </summary>
		/// <param name="fileId"> A file id which contains the function. </param>
		/// <param name="functionName"> A function name to be registered. </param>
		/// <param name="position"> The position of the function. </param>
		/// <returns> The function id. </returns>
		protected abstract long RegisterFunction(
				long fileId, string functionName, CodePosition position);

		/// <summary>
		///   Registers a statement with the specified file and function id which contains the statement and returns the branch id.
		/// </summary>
		/// <param name="fileId"> A file id which contains the function. </param>
		/// <param name="funcId"> A function id which contains the branch. </param>
		/// <param name="position"> The position of the statement. </param>
		/// <returns> The statement id. </returns>
		protected abstract long RegisterStatement(
				long fileId, long funcId, CodePosition position);

		/// <summary>
		///   Registers a branch such as if, while, do-while, for and ? (ternary expression) with the specified file and function id which contains the branch and returns the branch id.
		/// </summary>
		/// <param name="fileId"> A file id which contains the function. </param>
		/// <param name="funcId"> A function id which contains the branch. </param>
		/// <param name="position"> The position of the branch. </param>
		/// <returns> The branch id. </returns>
		protected abstract long RegisterBranch(
				long fileId, long funcId, CodePosition position);

		/// <summary>
		///   Registers a test case and returns the test case id.
		/// </summary>
		/// <param name="fileId"> A file id which contains the function. </param>
		/// <returns> The test case id. </returns>
		protected abstract long RegisterTestCase(long fileId);

		/// <summary>
		///   Instruments test code for measuring code coverage and returns the modifieid code.
		/// </summary>
		/// <param name="support">A support to measure coverage.</param>
		/// <param name="fileInfo">A <c>FileInfo</c> instance to be instrumented.</param>
		/// <param name="baseDirInfo">A <c>DirectoryInfo</c> instance of base directory.</param>
		/// <returns>The modified test code.</returns>
		public string InstrumentTestCase(
				LanguageSupport support, FileInfo fileInfo, DirectoryInfo baseDirInfo) {
			var relativePath = ParaibaPath.GetRelativePath( fileInfo.FullName, baseDirInfo.FullName);

			var root = support.CodeToXml.GenerateFromFile(fileInfo.FullName);
			var inserter = support.AstTransformer;

			var fileId = RegisterFile(fileInfo);

			var targets = support.AstAnalyzer.FindTestCases(root);
			foreach (var target in targets) {
				var testCaseId = RegisterTestCase(fileId);
				inserter.InsertTestCaseId(target, testCaseId, relativePath);
			}

			// Add import for logging executed items
			inserter.InsertImport(root);

			return support.XmlToCode.Generate(root);
		}

		/// <summary>
		///   Instruments production code for measuring code coverage and returns the modifieid code.
		/// </summary>
		/// <param name="support">A support to measure coverage.</param>
		/// <param name="fileInfo">A <c>FileInfo</c> instance to be instrumented.</param>
		/// <returns>The modified production code.</returns>
		public string InstrumentStatementAndPredicate(
				LanguageSupport support, FileInfo fileInfo) {
			var root = support.CodeToXml.GenerateFromFile(fileInfo.FullName);
			var inserter = support.AstTransformer;

			var fileId = RegisterFile(fileInfo);

			// ステートメントを挿入できるようにブロックを補う
			inserter.Align(root);

			// switch文を正しく測定できるようにdefault節を追加する
			inserter.SupplementDefaultCase(root);

			var analyzer = support.AstAnalyzer;
			var funcs = analyzer.FindFunctions(root);
			foreach (var func in funcs) {
				var funcName = analyzer.GetFunctionName(func);
				var funcId = RegisterFunction(fileId, funcName, CodePositions.Create(func));

				InstrumentStatement(support, fileId, funcId, inserter, func);
				InstrumentBranch(support, fileId, funcId, inserter, func);
			}

			// Add import for logging executed items
			inserter.InsertImport(root);

			return support.XmlToCode.Generate(root);
		}

		private void InstrumentStatement(
				LanguageSupport support, long fileId, long funcId, AstTransformer inserter, XElement func) {
			var stmts = support.AstAnalyzer.FindStatements(func);
			foreach (var stmt in stmts) {
				var position = CodePositions.Create(stmt);
				var stmtId = RegisterStatement(fileId, funcId, position);
				inserter.InsertStatementBefore(stmt, stmtId, CodeTransformer.Done, ElementType.Statement);
			}
		}

		private void InstrumentBranch(
				LanguageSupport support, long fileId, long funcId, AstTransformer inserter, XElement func) {
			var branches = support.AstAnalyzer.FindBranches(func);
			foreach (var branch in branches) {
				var position = CodePositions.Create(branch);
				var branchId = RegisterBranch(fileId, funcId, position);
				inserter.InsertPredicate(branch, branchId, ElementType.Decision);
			}
		}
	}
}