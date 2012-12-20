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
using Code2Xml.Languages.Java.CodeToXmls;
using Code2Xml.Languages.Java.XmlToCodes;
using Occf.Core.Manipulators;
using Occf.Core.Manipulators.Analyzers;
using Occf.Core.Manipulators.Taggers;
using Occf.Core.Manipulators.Transformers;
using Occf.Languages.Java.Manipulators.Analyzers;
using Occf.Languages.Java.Manipulators.Taggers;
using Occf.Languages.Java.Manipulators.Transformers;
using Occf.Languages.Java.Properties;
using Paraiba.IO;

namespace Occf.Languages.Java.Manipulators {
	[Export(typeof(LanguageSupport))]
	public class JavaSupport : LanguageSupport {
		private IEnumerable<string> _filePatterns;
		private AstTransformer _inserter;
		private Tagger _tagger;

		public override string Name {
			get { return "Java"; }
		}

		public override IEnumerable<string> FilePatterns {
			get { return _filePatterns ?? (_filePatterns = new[] { "*.java" }); }
		}

		public override CodeToXml CodeToXml {
			get { return JavaCodeToXml.Instance; }
		}

		public override XmlToCode XmlToCode {
			get { return JavaXmlToCode.Instance; }
		}

		public override AstTransformer AstTransformer {
			get { return _inserter ?? (_inserter = new JavaAstTransformer()); }
		}

		public override AstAnalyzer AstAnalyzer {
			get { return JavaAstAnalyzer.Instance; }
		}

		public override Tagger Tagger {
			get { return _tagger ?? (_tagger = new JavaTagger()); }
		}

		public override void CopyLibraries(DirectoryInfo outDirInfo) {
			ParaibaFile.WriteIfDifferentSize(
					Path.Combine(outDirInfo.FullName, "CoverageWriter.File.jar"),
					Resources.CoverageWriter_File);
			ParaibaFile.WriteIfDifferentSize(
					Path.Combine(outDirInfo.FullName, "junit-4.8.2.jar"),
					Resources.junit_4_8_2);
			ParaibaFile.WriteIfDifferentSize(
					Path.Combine(
							outDirInfo.FullName, "Occf.Writer.File.Java.dll"),
					Environment.Is64BitOperatingSystem
							? Resources.Occf_Writer_File_Java_x64
							: Resources.Occf_Writer_File_Java_x86);
		}

		public override void RemoveLibraries(DirectoryInfo outDirInfo) {
			outDirInfo.GetFile("CoverageWriter.File.jar");
			outDirInfo.GetFile("junit-4.8.2.jar");
			outDirInfo.GetFile("Occf.Writer.File.Java.dll");
		}
	}
}