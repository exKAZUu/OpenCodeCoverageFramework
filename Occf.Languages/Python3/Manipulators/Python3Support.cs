using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Code2Xml.Core;
using Occf.Core.Manipulators;
using Occf.Core.Manipulators.Analyzers;
using Occf.Core.Manipulators.Taggers;
using Occf.Core.Manipulators.Transformers;
using Occf.Languages.Python3.Manipulators.Analyzers;
using Occf.Languages.Python3.Manipulators.Taggers;
using Occf.Languages.Python3.Manipulators.Transformers;

namespace Occf.Languages.Python3.Manipulators {
	[Export(typeof(LanguageSupport))]
	public class Python3Support : LanguageSupport {
		private IEnumerable<string> _filePatterns;
		private AstTransformer _inserter;
		private Tagger _tagger;

		public override string Name {
			get { return "Python3"; }
		}

		public override Processor Processor {
			get { return ProcessorLoader.Python3; }
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
			throw new NotImplementedException();
		}

		public override void RemoveLibraries(DirectoryInfo outDirInfo) {
			throw new NotImplementedException();
		}
	}
}