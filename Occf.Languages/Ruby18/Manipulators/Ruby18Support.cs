using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Code2Xml.Core.Processors;
using Code2Xml.Objects;
using Occf.Core.Manipulators;
using Occf.Core.Manipulators.Analyzers;
using Occf.Core.Manipulators.Taggers;
using Occf.Core.Manipulators.Transformers;
using Occf.Languages.Ruby18.Manipulators.Analyzers;
using Occf.Languages.Ruby18.Manipulators.Taggers;
using Occf.Languages.Ruby18.Manipulators.Transformers;

namespace Occf.Languages.Ruby18.Manipulators {
	[Export(typeof(LanguageSupport))]
	public class Ruby18Support : LanguageSupport {
		private IEnumerable<string> _filePatterns;
		private AstTransformer _inserter;
		private Tagger _tagger;

		public override string Name {
			get { return "Ruby18"; }
		}

		public override Processor Processor {
			get { return Processors.Ruby18; }
		}

		public override IEnumerable<string> FilePatterns {
			get { return _filePatterns ?? (_filePatterns = new[] { "*.rb" }); }
		}

		public override AstTransformer AstTransformer {
			get { return _inserter ?? (_inserter = new Ruby18AstTransformer()); }
		}

		public override AstAnalyzer AstAnalyzer {
			get { return Ruby18AstAnalyzer.Instance; }
		}

		public override Tagger Tagger {
			get { return _tagger ?? (_tagger = new Ruby18Tagger()); }
		}

		public override void CopyLibraries(DirectoryInfo outDirInfo, RecordingMode recordingMode) {}

		public override void RemoveLibraries(DirectoryInfo outDirInfo) {}
	}
}