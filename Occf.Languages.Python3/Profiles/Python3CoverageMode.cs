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
using Occf.Core.Operators.Inserters;
using Occf.Core.Operators.Selectors;
using Occf.Core.Operators.Taggers;
using Occf.Core.Profiles;
using Occf.Languages.Python3.Operators.Inserters;
using Occf.Languages.Python3.Operators.Selectors;
using Occf.Languages.Python3.Operators.Taggers;

namespace Occf.Languages.Python3.Profiles {
	[Export(typeof(CoverageMode))]
	public class Python3CoverageMode : CoverageMode {
		private IEnumerable<string> _filePatterns;
		private NodeInserter _inserter;
		private Selector _statementSelector;
		private Selector _branchSelector;
		private Selector _conditionSelector;
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

		public override NodeInserter NodeInserter {
			get { return _inserter ?? (_inserter = new Python3NodeInserter()); }
		}

		public override Selector FunctionSelector {
			get { return NoSelector.Instance; }
		}

		public override Selector FunctionNameSelector {
			get { return NoSelector.Instance; }
		}

		public override Selector StatementSelector {
			get {
				return _statementSelector
				       ?? (_statementSelector = new Python3SimpleStatementSelector());
			}
		}

		public override Selector InitializerSelector {
			get { return NoSelector.Instance; }
		}

		public override Selector BranchSelector {
			get { return _branchSelector ?? (_branchSelector = new Python3BranchSelector()); }
		}

		public override Selector ConditionSelector {
			get {
				return _conditionSelector
				       ?? (_conditionSelector = new Python3ConditionSelector());
			}
		}

		public override Selector SwitchSelector {
			get { return NoSelector.Instance; }
		}

		public override Selector CaseLabelTailSelector {
			get { return NoSelector.Instance; }
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
			get { return _tagger ?? (_tagger = new Python3Tagger()); }
		}

		public override void CopyLibraries(DirectoryInfo outDirInfo) {
			throw new NotImplementedException();
		}

		public override void RemoveLibraries(DirectoryInfo outDirInfo) {
			throw new NotImplementedException();
		}
	}
}