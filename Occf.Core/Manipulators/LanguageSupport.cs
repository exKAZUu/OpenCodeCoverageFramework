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
using System.IO;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.XmlToCodes;
using Occf.Core.Manipulators.Analyzers;
using Occf.Core.Manipulators.Taggers;
using Occf.Core.Manipulators.Transformers;

namespace Occf.Core.Manipulators {
	/// <summary>
	/// A support (method/way) for masuring test coverage with respect to languages.
	/// </summary>
	public abstract class LanguageSupport {
		public abstract IEnumerable<string> FilePatterns { get; }
		public abstract string Name { get; }

		public abstract CodeToXml CodeToXml { get; }
		public abstract XmlToCode XmlToCode { get; }

		public abstract AstTransformer AstTransformer { get; }
		public abstract AstAnalyzer AstAnalyzer { get; }

		public abstract Tagger Tagger { get; }

		public abstract void CopyLibraries(DirectoryInfo outDirInfo, RecordingMode recordingMode);
		public abstract void RemoveLibraries(DirectoryInfo outDirInfo);
	}
}