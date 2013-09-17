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
using System.Xml.Linq;
using Code2Xml.Core.Location;
using Occf.Core.Manipulators.Taggers;

namespace Occf.Core.CoverageInformation.Elements {
	/// <summary>
	/// A class for a program element to measure coverage.
	/// This should be class instead of struct
	/// because 
	/// </summary>
	[Serializable]
	public class CoverageElement : ICoverageElement {
		public CoverageElement(string relativePath, XElement node, Tagger tagger) {
			RelativePath = relativePath;
			Position = CodeRange.Locate(node);
			Qualifiers = tagger.Tag(node);
		}

		#region ICoverageElement Members

		public CodeRange Position { get; private set; }

		public CoverageState State { get; protected set; }

		public List<string> Qualifiers { get; private set; }

		public string Tag {
			get { return RelativePath.Replace('\\', '>') + '>' + string.Join(">", Qualifiers) + ">"; }
		}

		public string RelativePath { get; private set; }

		public virtual void UpdateState(CoverageState state) {
			State |= state;
		}

		#endregion
	}
}