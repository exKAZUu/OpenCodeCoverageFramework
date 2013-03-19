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
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.XmlToCodes;
using Code2Xml.Languages.C.CodeToXmls;
using Code2Xml.Languages.C.XmlToCodes;
using Occf.Core.Manipulators;
using Occf.Core.Manipulators.Analyzers;
using Occf.Core.Manipulators.Taggers;
using Occf.Core.Manipulators.Transformers;
using Occf.Languages.C.Manipulators.Analyzers;
using Occf.Languages.C.Manipulators.Taggers;
using Occf.Languages.C.Manipulators.Transformers;
using Occf.Languages.C.Properties;
using Paraiba.IO;

namespace Occf.Languages.C.Manipulators {
	[Export(typeof(LanguageSupport))]
	public class CSupport : LanguageSupport {
		private IEnumerable<string> _filePatterns;
		private AstTransformer _inserter;
		private Tagger _tagger;

		public override string Name {
			get { return "C"; }
		}

		public override IEnumerable<string> FilePatterns {
			get { return _filePatterns ?? (_filePatterns = new[] { "*.c" }); }
		}

		public override CodeToXml CodeToXml {
			get { return CCodeToXml.Instance; }
		}

		public override XmlToCode XmlToCode {
			get { return CXmlToCode.Instance; }
		}

		public override AstTransformer AstTransformer {
			get { return _inserter ?? (_inserter = new CAstTransformer()); }
		}

		public override AstAnalyzer AstAnalyzer {
			get { return CAstAnalyzer.Instance; }
		}

		public override Tagger Tagger {
			get { return _tagger ?? (_tagger = new CTagger()); }
		}

		public override void CopyLibraries(DirectoryInfo outDirInfo, RecordingMode recordingMode) {
			ParaibaFile.WriteIfDifferentSize(
					Path.Combine(outDirInfo.FullName, "covman.c"),
					Resources.covman_c);
			ParaibaFile.WriteIfDifferentSize(
					Path.Combine(outDirInfo.FullName, "covman.h"),
					Resources.covman_h);
		}

		public override void RemoveLibraries(DirectoryInfo outDirInfo) {
			outDirInfo.GetFile("covman.c").Delete();
			outDirInfo.GetFile("covman.h").Delete();
		}
	}
}