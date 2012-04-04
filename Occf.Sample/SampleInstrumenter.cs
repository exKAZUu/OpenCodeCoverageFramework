#region License

// Copyright (C) 2011-2012 Kazunori Sakamoto
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
using Occf.Core.CoverageProfiles;
using Occf.Core.Operators.Inserters;
using Occf.Tools.Core;
using Paraiba.IO;

namespace Occf.Sample {
	public class SampleInstrumenter : Instrumenter {
		public DirectoryInfo OutDirInfo { get; set; }
		public DirectoryInfo BaseDirInfo { get; set; }
		public int Id { get; private set; }
		public int TestCaseId { get; private set; }

		/// <summary>
		/// Construct an instance with the specified paths of output and base directory.
		/// </summary>
		/// <param name="outDirInfo">A DirectoryInfo for output directory.</param>
		/// <param name="inDirInfo">A DirectoryInfo for input base directory.</param>
		public SampleInstrumenter(
				DirectoryInfo outDirInfo, DirectoryInfo inDirInfo) {
			OutDirInfo = outDirInfo;
			BaseDirInfo = inDirInfo;
		}

		public void WriteInstrumentedProductionCode(
				CoverageProfile profile, FileInfo inFileInfo) {
			var relativePath = XPath.GetRelativePath(
					inFileInfo.FullName, BaseDirInfo.FullName);
			var outFileInfo = OutDirInfo.GetFile(relativePath);
			var code = InstrumentStatementAndPredicate(profile, inFileInfo);
			outFileInfo.Directory.Create();
			File.WriteAllText(outFileInfo.FullName, code);
		}

		public void WriteInstrumentedTestCode(
				CoverageProfile profile, FileInfo inFileInfo) {
			var relativePath = XPath.GetRelativePath(
					inFileInfo.FullName, BaseDirInfo.FullName);
			var outFileInfo = OutDirInfo.GetFile(relativePath);
			var code = InstrumentTestCase(profile, inFileInfo, BaseDirInfo);
			outFileInfo.Directory.Create();
			File.WriteAllText(outFileInfo.FullName, code);
		}

		public void CopyLibraries(CoverageProfile profile) {
			foreach (var name in profile.LibraryNames) {
				var srcPath = Path.Combine(Names.Library, name);
				File.Copy(srcPath, OutDirInfo.GetFile(name).FullName, true);
			}
		}

		public void CopyFile(FileInfo inFileInfo) {
			var relativePath = XPath.GetRelativePath(
					inFileInfo.FullName, BaseDirInfo.FullName);
			var outFileInfo = OutDirInfo.GetFile(relativePath);
			outFileInfo.Directory.Create();
			inFileInfo.CopyTo(outFileInfo.FullName, true);
		}

		protected override int RegisterFile(FileInfo fileInfo) {
			var relativePath = XPath.GetRelativePath(
					fileInfo.FullName, BaseDirInfo.FullName);
			Console.WriteLine("Relative path: " + relativePath);
			return Id++;
		}

		protected override int RegisterFunction(
				int fileId, string functionName, CodePosition position) {
			Console.WriteLine("Function name: " + functionName);
			return Id++;
		}

		protected override int RegisterStatement(
				int fileId, int funcId, CodePosition position) {
			Console.WriteLine("Statement position: " + position.SmartPosition);
			return Id++;
		}

		protected override int RegisterBranch(
				int fileId, int funcId, CodePosition position) {
			Console.WriteLine("Branch position: " + position.SmartPosition);
			return Id++;
		}

		protected override int RegisterTestCase(int fileId) {
			Console.WriteLine("TestCase (fileId: " + fileId + ")");
			return TestCaseId++;
		}
	}
}