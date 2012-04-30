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
using Occf.Core.Operators.Inserters;
using Occf.Core.Operators.Selectors;
using Occf.Core.Operators.Taggers;
using Occf.Core.Profiles;
using Occf.Languages.C.Operators.Inserters;
using Occf.Languages.C.Operators.Selectors;
using Occf.Languages.C.Operators.Taggers;
using Occf.Languages.C.Properties;
using Paraiba.IO;

namespace Occf.Languages.C.Profiles {
	[Export(typeof(CoverageProfile))]
	public class CCoverageProfile : CoverageProfile {
		private IEnumerable<string> _filePatterns;
		private NodeInserter _inserter;
		private Selector _statementSelector;
		private Selector _initializerSelector;
		private Selector _branchSelector;
		private Selector _conditionSelector;
		private Selector _switchSelector;
		private Selector _caseLableTailSelector;
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

		public override NodeInserter NodeInserter {
			get { return _inserter ?? (_inserter = new CNodeInserter()); }
		}

		public override Selector FunctionSelector {
			get { return NoSelector.Instance; }
		}

		public override Selector FunctionNameSelector {
			get { return NoSelector.Instance; }
		}

		public override Selector StatementSelector {
			get { return _statementSelector ?? (_statementSelector = new CStatementSelector()); }
		}

		public override Selector InitializerSelector {
			get {
				return _initializerSelector
				       ?? (_initializerSelector = new CInitializerSelector());
			}
		}

		public override Selector BranchSelector {
			get { return _branchSelector ?? (_branchSelector = new CBranchSelector()); }
		}

		public override Selector ConditionSelector {
			get { return _conditionSelector ?? (_conditionSelector = new CConditionSelector()); }
		}

		public override Selector SwitchSelector {
			get { return _switchSelector ?? (_switchSelector = new CSwitchSelector()); }
		}

		public override Selector CaseLabelTailSelector {
			get {
				return _caseLableTailSelector
				       ?? (_caseLableTailSelector = new CCaseLabelTailSelector());
			}
		}

		public override Selector ForeachSelector {
			get { return NoSelector.Instance; }
		}

		public override Selector ForeachHeadSelector {
			get { return NoSelector.Instance; }
		}

		public override Selector ForeachTailSelector {
			get { return NoSelector.Instance; }
		}

		public override Selector TestCaseLabelTailSelector {
			get { return NoSelector.Instance; }
		}

		public override Tagger Tagger {
			get { return _tagger ?? (_tagger = new CTagger()); }
		}

		public override void CopyLibraries(DirectoryInfo outDirInfo) {
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