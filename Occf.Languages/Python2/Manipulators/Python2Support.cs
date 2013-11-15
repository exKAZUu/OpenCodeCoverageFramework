using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Code2Xml.Core;
using Occf.Core.Manipulators;
using Occf.Core.Manipulators.Analyzers;
using Occf.Core.Manipulators.Taggers;
using Occf.Core.Manipulators.Transformers;
using Occf.Languages.Python2.Properties;
using Occf.Languages.Python3.Manipulators.Analyzers;
using Occf.Languages.Python3.Manipulators.Taggers;
using Occf.Languages.Python3.Manipulators.Transformers;
using Paraiba.IO;

namespace Occf.Languages.Python2.Manipulators {
	[Export(typeof(LanguageSupport))]
	public class Python2Support : LanguageSupport {
		private IEnumerable<string> _filePatterns;
		private AstTransformer _inserter;
		private Tagger _tagger;

		public override string Name {
			get { return "Python2"; }
		}

		public override Processor Processor {
			get { return ProcessorLoader.Python2; }
		}

		public override IEnumerable<string> FilePatterns {
			get { return _filePatterns ?? (_filePatterns = new[] { "*.py" }); }
		}

		public override AstTransformer AstTransformer {
			get { return _inserter ?? (_inserter = new Python3AstTransformer()); }
		}

		public override AstAnalyzer AstAnalyzer {
			get { return Python3AstAnalyzer.Instance; }
		}

		public override Tagger Tagger {
			get { return _tagger ?? (_tagger = new Python3Tagger()); }
		}

		public override void CopyLibraries(DirectoryInfo outDirInfo, RecordingMode recordingMode) {
			ParaibaFile.WriteIfDifferentSize(
					Path.Combine(outDirInfo.FullName, "_CoverageWriter.pyd"),
					Resources._CoverageWriter);
			ParaibaFile.WriteIfDifferentSize(
					Path.Combine(outDirInfo.FullName, "CoverageWriter.py"),
					Resources.CoverageWriter);
		}

		public override void RemoveLibraries(DirectoryInfo outDirInfo) {
			outDirInfo.GetFile("_CoverageWriter.pyd").Delete();
			outDirInfo.GetFile("CoverageWriter.py").Delete();
		}
	}
}