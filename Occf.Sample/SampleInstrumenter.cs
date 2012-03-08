using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Code2Xml.Core.Position;
using Occf.Core.CoverageProfiles;
using Occf.Core.Operators.Inserters;
using Occf.Tools.Core;
using Paraiba.IO;

namespace Occf.Sample
{
	public class SampleInstrumenter : Instrumenter
	{
		public string OutDirPath { get; set; }
		public string InDirPath { get; set; }
		public int Id { get; private set; }
		public int TestCaseId { get; private set; }

		/// <summary>
		/// Construct an instance with the specified paths of output and base directory.
		/// </summary>
		/// <param name="outputDirPath">A path of output directory.</param>
		/// <param name="baseDirPath">A path of input base directory.</param>
		public SampleInstrumenter(string outputDirPath, string baseDirPath) {
			OutDirPath = outputDirPath;
			InDirPath = Path.GetFullPath(baseDirPath);
		}

		public void WriteInstrumentedProductionCode(CoverageProfile profile, string relativePath) {
			var inPath = Path.Combine(InDirPath, relativePath);
			var outPath = Path.Combine(OutDirPath, relativePath);
			var code = InstrumentStatementAndPredicate(profile, inPath);
			Directory.CreateDirectory(Path.GetDirectoryName(outPath));
			File.WriteAllText(outPath, code);
		}

		public void WriteInstrumentedTestCode(CoverageProfile profile, string relativePath) {
			var inPath = Path.Combine(InDirPath, relativePath);
			var outPath = Path.Combine(OutDirPath, relativePath);
			var code = InstrumentTestCase(profile, inPath);
			Directory.CreateDirectory(Path.GetDirectoryName(outPath));
			File.WriteAllText(outPath, code);
		}

		public void CopyLibraries(CoverageProfile profile) {
			foreach (var name in profile.LibraryNames) {
				var srcPath = Path.Combine(Names.Library, name);
				var dstPath = Path.Combine(OutDirPath, name);
				File.Copy(srcPath, dstPath, true);
			}
		}

		public void CopyFile(string relativePath) {
			var inPath = Path.Combine(InDirPath, relativePath);
			var outPath = Path.Combine(OutDirPath, relativePath);
			Directory.CreateDirectory(Path.GetDirectoryName(outPath));
			File.Copy(inPath, outPath, true);
		}

		protected override int RegisterFile(string filePath) {
			var fullPath = Path.GetFullPath(filePath);
			var relativePath = XPath.GetRelativePath(fullPath, InDirPath);
			Console.WriteLine("Relative path: " + relativePath);
			return Id++;
		}

		protected override int RegisterFunction(int fileId, string functionName, CodePosition position) {
			Console.WriteLine("Function name: " + functionName);
			return Id++;
		}

		protected override int RegisterStatement(int fileId, int funcId, CodePosition position) {
			Console.WriteLine("Statement position: " + position.SmartPosition);
			return Id++;
		}

		protected override int RegisterBranch(int fileId, int funcId, CodePosition position) {
			Console.WriteLine("Branch position: " + position.SmartPosition);
			return Id++;
		}

	    protected override int RegisterTestCase(int fileId) {
			Console.WriteLine("TestCase (fileId: " + fileId + ")");
			return TestCaseId++;
	    }
	}
}
