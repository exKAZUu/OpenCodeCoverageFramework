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
using Code2Xml.Languages.JavaScript.CodeToXmls;
using Code2Xml.Languages.JavaScript.XmlToCodes;
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

        public override IEnumerable<string> FilePatterns {
            get { return _filePatterns ?? (_filePatterns = new[] { "*.js" }); }
        }

        public override CodeToXml CodeToXml {
            get { return JavaScriptCodeToXml.Instance; }
        }

        public override XmlToCode XmlToCode {
            get { return JavaScriptXmlToCode.Instance; }
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