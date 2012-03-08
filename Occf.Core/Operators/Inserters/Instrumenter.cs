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

using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core.Position;
using Occf.Core.CoverageProfiles;

namespace Occf.Core.Operators.Inserters {
	/// <summary>
	///   A class to instrument for measureing code coverage.
	/// </summary>
	public abstract class Instrumenter {
		/// <summary>
		///   Registers a file with the specified path and returns the file id.
		/// </summary>
		/// <param name="filePath"> A file path to be instrumented. </param>
		/// <returns> The file id. </returns>
		protected abstract int RegisterFile(string filePath);

		/// <summary>
		///   Registers a function with the specified name and returns the function id.
		/// </summary>
		/// <param name="fileId"> A file id which contains the function. </param>
		/// <param name="functionName"> A function name to be registered. </param>
		/// <param name="position"> The position of the function. </param>
		/// <returns> The function id. </returns>
		protected abstract int RegisterFunction(
				int fileId, string functionName, CodePosition position);

		/// <summary>
		///   Registers a statement with the specified file and function id which contains the statement and returns the branch id.
		/// </summary>
		/// <param name="fileId"> A file id which contains the function. </param>
		/// <param name="funcId"> A function id which contains the branch. </param>
		/// <param name="position"> The position of the statement. </param>
		/// <returns> The statement id. </returns>
		protected abstract int RegisterStatement(
				int fileId, int funcId, CodePosition position);

		/// <summary>
		///   Registers a branch such as if, while, do-while, for and ? (ternary expression) with the specified file and function id which contains the branch and returns the branch id.
		/// </summary>
		/// <param name="fileId"> A file id which contains the function. </param>
		/// <param name="funcId"> A function id which contains the branch. </param>
		/// <param name="position"> The position of the branch. </param>
		/// <returns> The branch id. </returns>
		protected abstract int RegisterBranch(
				int fileId, int funcId, CodePosition position);

		/// <summary>
		///   Registers a test case and returns the test case id.
		/// </summary>
		/// <param name="fileId"> A file id which contains the function. </param>
		/// <returns> The test case id. </returns>
		protected abstract int RegisterTestCase(int fileId);

		/// <summary>
		///   Instruments test code for measuring code coverage and returns the modifieid code.
		/// </summary>
		/// <param name="profile"> A profile to measure coverage. </param>
		/// <param name="filePath"> A file to be instrumented. </param>
		/// <returns> The modified test code. </returns>
		public string InstrumentTestCase(
				CoverageProfile profile, string filePath) {
			var root = profile.CodeToXml.GenerateFromFile(filePath);
			var inserter = profile.NodeInserter;

			var fileId = RegisterFile(filePath);

			var targets = profile.TestCaseLabelTailSelector.Select(root);
			foreach (var target in targets) {
				var testCaseId = RegisterTestCase(fileId);
				inserter.InsertTestCaseId(target, testCaseId, filePath);
			}

			// Add import for logging executed items
			inserter.InsertImport(root);

			return profile.XmlToCode.Generate(root);
		}

		/// <summary>
		///   Instruments production code for measuring code coverage and returns the modifieid code.
		/// </summary>
		/// <param name="profile"> A profile to measure coverage. </param>
		/// <param name="filePath"> A file to be instrumented. </param>
		/// <returns> The modified production code. </returns>
		public string InstrumentStatementAndPredicate(
				CoverageProfile profile, string filePath) {
			var root = profile.CodeToXml.GenerateFromFile(filePath);
			var inserter = profile.NodeInserter;

			var fileId = RegisterFile(filePath);

			// ステートメントを挿入できるようにブロックを補う
			inserter.SupplementBlock(root);

			// switch文を正しく測定できるようにdefault節を追加する
			inserter.SupplementDefaultCase(root);

			var funcs = profile.FunctionSelector.Select(root);
			foreach (var func in funcs) {
				var funcName = profile.FunctionNameSelector.Select(func)
						.First()
						.Value;
				var funcId = RegisterFunction(
						fileId, funcName, CodePositionAnalyzer.Create(func));

				InstrumentStatement(profile, fileId, funcId, inserter, func);
				InstrumentBranch(profile, fileId, funcId, inserter, func);
			}

			// Add import for logging executed items
			inserter.InsertImport(root);

			return profile.XmlToCode.Generate(root);
		}

		private void InstrumentStatement(
				CoverageProfile profile, int fileId, int funcId, NodeInserter inserter,
				XElement func) {
			var stmts = profile.StatementSelector.Select(func);
			foreach (var stmt in stmts) {
				var position = CodePositionAnalyzer.Create(stmt);
				var stmtId = RegisterStatement(fileId, funcId, position);
				inserter.InsertStatementBefore(
						stmt, stmtId, CoverageInserter.Done,
						ElementType.Statement);
			}
		}

		private void InstrumentBranch(
				CoverageProfile profile, int fileId, int funcId, NodeInserter inserter,
				XElement func) {
			var branches = profile.BranchSelector.Select(func);
			foreach (var branch in branches) {
				var position = CodePositionAnalyzer.Create(branch);
				var branchId = RegisterBranch(fileId, funcId, position);
				inserter.InsertPredicate(
						branch, branchId, ElementType.Decision);
			}
		}
	}
}