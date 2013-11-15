using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Code2Xml.Core;
using Code2Xml.Languages.ExternalProcessors.Processors.SrcML;
using Occf.Core.Manipulators;
using Occf.Core.Manipulators.Analyzers;
using Occf.Core.Manipulators.Taggers;
using Occf.Core.Manipulators.Transformers;
using Occf.Languages.Cpp.Manipulators.Analyzers;
using Occf.Languages.Cpp.Manipulators.Taggers;
using Occf.Languages.Cpp.Manipulators.Transformers;

namespace Occf.Languages.Cpp.Manipulators {
	[Export(typeof(LanguageSupport))]
	public class CppSupport : LanguageSupport {
		private IEnumerable<string> _filePatterns;
		private AstTransformer _inserter;
		private Tagger _tagger;

		public override string Name {
			get { return "C++"; }
		}

		public override Processor Processor {
			get { return new SrcMLForCppProcessor(); }
		}

		public override IEnumerable<string> FilePatterns {
			get { return _filePatterns ?? (_filePatterns = new[] { "*.cpp", "*.cxx", "*.c++" }); }
		}

		public override AstTransformer AstTransformer {
			get { return _inserter ?? (_inserter = new CppAstTransformer()); }
		}

		public override AstAnalyzer AstAnalyzer {
			get { return CppAstAnalyzer.Instance; }
		}

		public override Tagger Tagger {
			get { return _tagger ?? (_tagger = new CppTagger()); }
		}

		public override void CopyLibraries(DirectoryInfo outDirInfo, RecordingMode recordingMode) {}

		public override void RemoveLibraries(DirectoryInfo outDirInfo) {}
	}
}