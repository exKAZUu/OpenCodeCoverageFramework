using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Code2Xml.Core.Processors;
using Code2Xml.Objects;
using Occf.Core.Manipulators;
using Occf.Core.Manipulators.Analyzers;
using Occf.Core.Manipulators.Taggers;
using Occf.Core.Manipulators.Transformers;
using Occf.Languages.Lua.Manipulators.Analyzers;
using Occf.Languages.Lua.Manipulators.Taggers;
using Occf.Languages.Lua.Manipulators.Transformers;

namespace Occf.Languages.Lua.Manipulators {
	[Export(typeof(LanguageSupport))]
	public class LuaSupport : LanguageSupport {
		private IEnumerable<string> _filePatterns;
		private AstTransformer _inserter;
		private Tagger _tagger;

		public override string Name {
			get { return "Lua"; }
		}

		public override Processor Processor {
			get { return Processors.LuaUsingAntlr3; }
		}

		public override IEnumerable<string> FilePatterns {
			get { return _filePatterns ?? (_filePatterns = new[] { "*.lua" }); }
		}

		public override AstTransformer AstTransformer {
			get { return _inserter ?? (_inserter = new LuaAstTransformer()); }
		}

		public override AstAnalyzer AstAnalyzer {
			get { return LuaAstAnalyzer.Instance; }
		}

		public override Tagger Tagger {
			get { return _tagger ?? (_tagger = new LuaTagger()); }
		}

		public override void CopyLibraries(DirectoryInfo outDirInfo, RecordingMode recordingMode) {}

		public override void RemoveLibraries(DirectoryInfo outDirInfo) {}
	}
}