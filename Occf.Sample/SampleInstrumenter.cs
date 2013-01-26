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

using System;
using System.IO;
using Code2Xml.Core.Position;
using Occf.Core.Manipulators;
using Occf.Core.Manipulators.Transformers;
using Paraiba.IO;

namespace Occf.Sample {
	public class SampleInstrumenter : Instrumenter {
		public DirectoryInfo OutDirInfo { get; set; }
		public DirectoryInfo BaseDirInfo { get; set; }
		public long Id { get; private set; }
		public int TestCaseId { get; private set; }

		/// <summary>
		/// Construct an instance with the specified paths of output and base directory.
		/// </summary>
		/// <param name="outDirInfo">A DirectoryInfo for output directory.</param>
		/// <param name="inDirInfo">A DirectoryInfo for input base directory.</param>
		public SampleInstrumenter( DirectoryInfo outDirInfo, DirectoryInfo inDirInfo) {
			OutDirInfo = outDirInfo;
			BaseDirInfo = inDirInfo;
		}

		public void WriteInstrumentedProductionCode( LanguageSupport mode, FileInfo inFileInfo) {
			var relativePath = ParaibaPath.GetRelativePath( inFileInfo.FullName, BaseDirInfo.FullName);
			var outFileInfo = OutDirInfo.GetFile(relativePath);
			var code = InstrumentStatementAndPredicate(mode, inFileInfo);
			outFileInfo.Directory.Create();
			File.WriteAllText(outFileInfo.FullName, code);
		}

		public void WriteInstrumentedTestCode( LanguageSupport mode, FileInfo inFileInfo) {
			var relativePath = ParaibaPath.GetRelativePath( inFileInfo.FullName, BaseDirInfo.FullName);
			var outFileInfo = OutDirInfo.GetFile(relativePath);
			var code = InstrumentTestCase(mode, inFileInfo, BaseDirInfo);
			outFileInfo.Directory.Create();
			File.WriteAllText(outFileInfo.FullName, code);
		}

		public void CopyFile(FileInfo inFileInfo) {
			var relativePath = ParaibaPath.GetRelativePath(
					inFileInfo.FullName, BaseDirInfo.FullName);
			var outFileInfo = OutDirInfo.GetFile(relativePath);
			outFileInfo.Directory.Create();
			inFileInfo.CopyTo(outFileInfo.FullName, true);
		}

		protected override long RegisterFile(FileInfo fileInfo) {
			var relativePath = ParaibaPath.GetRelativePath(
					fileInfo.FullName, BaseDirInfo.FullName);
			Console.WriteLine("Relative path: " + relativePath);
			return Id++;
		}

		protected override long RegisterFunction(
				long fileId, string functionName, CodePosition position) {
			Console.WriteLine("Function name: " + functionName + ", pos: " + position);
			return Id++;
		}

		protected override long RegisterStatement(
				long fileId, long funcId, CodePosition position) {
			Console.WriteLine("Statement position: " + position);
			return Id++;
		}

		protected override long RegisterBranch(
				long fileId, long funcId, CodePosition position) {
			Console.WriteLine("Branch position: " + position.SmartPositionString);
			return Id++;
		}

		protected override long RegisterTestCase(long fileId) {
			Console.WriteLine("TestCase (fileId: " + fileId + ")");
			return TestCaseId++;
		}
	}
}