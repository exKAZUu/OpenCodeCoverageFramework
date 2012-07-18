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
using Occf.Core.Operators.Inserters;
using Occf.Core.Operators.Selectors;
using Occf.Core.Operators.Taggers;
using Occf.Core.Profiles;
using Occf.Languages.Java.Operators.Inserters;
using Occf.Languages.Java.Operators.Selectors;
using Occf.Languages.Java.Operators.Taggers;
using Occf.Languages.Java.Properties;
using Paraiba.IO;

namespace Occf.Languages.Java.Profiles {
	[Export(typeof(CoverageProfile))]
	public class JavaCoverageProfile : CoverageProfile {
		private IEnumerable<string> _filePatterns;
		private NodeInserter _inserter;
		private Selector _functionSelector;
		private Selector _functionNameSelector;
		private Selector _statementSelector;
		private Selector _initializerSelector;
		private Selector _branchSelector;
		private Selector _conditionSelector;
		private Selector _switchSelector;
		private Selector _caseLableTailSelector;
		private Selector _foreachSelector;
		private Selector _foreachHeadSelector;
		private Selector _foreachTailSelector;
		private Selector _testCaseLableTailSelector;
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

		public override NodeInserter NodeInserter {
			get { return _inserter ?? (_inserter = new JavaNodeInserter()); }
		}

		public override Selector FunctionSelector {
			get { return _functionSelector ?? (_functionSelector = new JavaMethodSelector()); }
		}

		public override Selector FunctionNameSelector {
			get {
				return _functionNameSelector
				       ?? (_functionNameSelector = new JavaMethodNameSelector());
			}
		}

		public override Selector StatementSelector {
			get {
				return _statementSelector
				       ?? (_statementSelector = new JavaStatementSelector());
			}
		}

		public override Selector InitializerSelector {
			get {
				return _initializerSelector
				       ?? (_initializerSelector = new JavaInitializerSelector());
			}
		}

		public override Selector BranchSelector {
			get { return _branchSelector ?? (_branchSelector = new JavaBranchSelector()); }
		}

		public override Selector ConditionSelector {
			get {
				return _conditionSelector
				       ?? (_conditionSelector = new JavaConditionSelector());
			}
		}

		public override Selector SwitchSelector {
			get { return _switchSelector ?? (_switchSelector = new JavaSwitchSelector()); }
		}

		public override Selector CaseLabelTailSelector {
			get {
				return _caseLableTailSelector
				       ?? (_caseLableTailSelector = new JavaCaseLabelTailSelector());
			}
		}

		public override Selector ForeachSelector {
			get { return _foreachSelector ?? (_foreachSelector = new JavaForeachSelector()); }
		}

		public override Selector ForeachHeadSelector {
			get {
				return _foreachHeadSelector
				       ?? (_foreachHeadSelector = new JavaForeachHeadSelector());
			}
		}

		public override Selector ForeachTailSelector {
			get {
				return _foreachTailSelector
				       ?? (_foreachTailSelector = new JavaForeachTailSelector());
			}
		}

		public override Selector TestCaseLabelTailSelector {
			get {
				return _testCaseLableTailSelector
				       ?? (_testCaseLableTailSelector = new JavaTestCaseLabelTailSelector());
			}
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