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
using System.Linq;
using Code2Xml.Core.Position;

namespace Occf.Core.CoverageInformation.Elements {
	[Serializable]
	public class CoverageElementGroup : ICoverageElement {
		/// <summary>
		///   Initializes a instance for one coverage element. For example,
		/// </summary>
		/// <param name="parentElement"> </param>
		public CoverageElementGroup(CoverageElement parentElement) {
			ParentElement = parentElement;
			Targets = new List<CoverageElement>();
		}

		public CoverageElementGroup(
				CoverageElement parentElement,
				IList<CoverageElement> targets) {
			ParentElement = parentElement;
			Targets = targets;
		}

		public CoverageElement ParentElement { get; private set; }

		public IList<CoverageElement> Targets { get; private set; }

		public CoverageState StateChildrenOrParent {
			get {
				return Targets.Count > 0
						? Targets.Aggregate(CoverageState.Done, (s, t) => s & t.State)
						: ParentElement.State;
			}
		}

		#region ICoverageElement Members

		public CodePosition Position {
			get { return ParentElement.Position; }
		}

		public CoverageState State {
			get { return Targets.Aggregate(ParentElement.State, (s, t) => s & t.State); }
		}

		public string Tag {
			get { return ParentElement.Tag; }
		}

		public string RelativePath {
			get { return ParentElement.RelativePath; }
		}

		public void Execute(CoverageElement lastElement, CoverageState state) {}

		#endregion
	}
}