using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Code2Xml.Core;
using Occf.Core.Manipulators;
using Occf.Core.Manipulators.Analyzers;
using Occf.Core.Manipulators.Taggers;
using Occf.Core.Manipulators.Transformers;
using Occf.Languages.JavaScript.Manipulators.Analyzers;
using Occf.Languages.JavaScript.Manipulators.Taggers;
using Occf.Languages.JavaScript.Manipulators.Transformers;

namespace Occf.Languages.JavaScript.Manipulators {
	[Export(typeof(LanguageSupport))]
	public class JavaScriptSupport : LanguageSupport {
		private IEnumerable<string> _filePatterns;
		private AstTransformer _inserter;
		private Tagger _tagger;

		public override string Name {
			get { return "JavaScript"; }
		}

		public override Processor Processor {
			get { return ProcessorLoader.JavaScriptUsingAntlr3; }
		}

		public override IEnumerable<string> FilePatterns {
			get { return _filePatterns ?? (_filePatterns = new[] { "*.js" }); }
		}

		public override AstTransformer AstTransformer {
			get { return _inserter ?? (_inserter = new JavaScriptAstTransformer()); }
		}

		public override AstAnalyzer AstAnalyzer {
			get { return JavaScriptAstAnalyzer.Instance; }
		}

		public override Tagger Tagger {
			get { return _tagger ?? (_tagger = new JavaScriptTagger()); }
		}

		public override void CopyLibraries(DirectoryInfo outDirInfo, RecordingMode recordingMode) {}

		public override void RemoveLibraries(DirectoryInfo outDirInfo) {}
	}
}