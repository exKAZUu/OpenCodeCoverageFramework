using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Occf.Core.CodeInformation;
using Occf.Core.Extensions;
using Occf.Core.Operators.Inserters;
using Paraiba.IO;

namespace Occf.Sample
{
    public class SampleInstrumenter : Instrumenter
    {
        public string OutDirPath { get; set; }
        public string InDirPath { get; set; }
        public int Id { get; private set; }

        /// <summary>
        /// Construct an instance with the specified paths of output and base directory.
        /// </summary>
        /// <param name="outputDirPath">A path of output directory.</param>
        /// <param name="baseDirPath">A path of input base directory.</param>
        public SampleInstrumenter(string outputDirPath, string baseDirPath) {
            OutDirPath = outputDirPath;
            InDirPath = Path.GetFullPath(baseDirPath);
        }

        public void WriteInstrumentedCode(CoverageProfile profile, string relativePath) {
            var inPath = Path.Combine(InDirPath, relativePath);
            var outPath = Path.Combine(OutDirPath, relativePath);
            var code = InstrumentStatementAndPredicate(profile, inPath);
            Directory.CreateDirectory(Path.GetDirectoryName(outPath));
            File.WriteAllText(outPath, code);
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

        protected override int RegisterFunction(int fileId, string functionName) {
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
    }
}
