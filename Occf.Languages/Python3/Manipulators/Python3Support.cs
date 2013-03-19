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

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.XmlToCodes;
using Code2Xml.Languages.Python3.CodeToXmls;
using Code2Xml.Languages.Python3.XmlToCodes;
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

		public override IEnumerable<string> FilePatterns {
			get { return _filePatterns ?? (_filePatterns = new[] { "*.py" }); }
		}

		public override CodeToXml CodeToXml {
			get { return Python3CodeToXml.Instance; }
		}

		public override XmlToCode XmlToCode {
			get { return Python3XmlToCode.Instance; }
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