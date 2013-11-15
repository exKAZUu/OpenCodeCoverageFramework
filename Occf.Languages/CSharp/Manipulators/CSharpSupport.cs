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

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Code2Xml.Core;
using Occf.Core.Manipulators;
using Occf.Core.Manipulators.Analyzers;
using Occf.Core.Manipulators.Taggers;
using Occf.Core.Manipulators.Transformers;
using Occf.Languages.CSharp.Manipulators.Analyzers;
using Occf.Languages.CSharp.Manipulators.Taggers;
using Occf.Languages.CSharp.Manipulators.Transformers;

namespace Occf.Languages.CSharp.Manipulators {
	[Export(typeof(LanguageSupport))]
	public class CSharpSupport : LanguageSupport {
		private IEnumerable<string> _filePatterns;
		private AstTransformer _inserter;
		private Tagger _tagger;

		public override string Name {
			get { return "CSharp"; }
		}

		public override Processor Processor {
			get { return ProcessorLoader.CSharpUsingAntlr3; }
		}

		public override IEnumerable<string> FilePatterns {
			get { return _filePatterns ?? (_filePatterns = new[] { "*.cs" }); }
		}

		public override AstTransformer AstTransformer {
			get { return _inserter ?? (_inserter = new CSharpAstTransformer()); }
		}

		public override AstAnalyzer AstAnalyzer {
			get { return CSharpAstAnalyzer.Instance; }
		}

		public override Tagger Tagger {
			get { return _tagger ?? (_tagger = new CSharpTagger()); }
		}

		public override void CopyLibraries(DirectoryInfo outDirInfo, RecordingMode recordingMode) {}

		public override void RemoveLibraries(DirectoryInfo outDirInfo) {}
	}
}