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
using System.Xml.Linq;
using Code2Xml.Core.Position;
using Occf.Core.Operators.Taggers;

namespace Occf.Core.CoverageInformation {
	[Serializable]
	public class CoverageElement : ICoverageElement {
		public CoverageElement(string relativePath, XElement node, Tagger tagger) {
			RelativePath = relativePath;
			Position = CodePositions.New(node);
			var tag = relativePath.Replace('\\', '>') + '>' + tagger.Tag(node);
			Tag = tag.EndsWith(">") ? tag : tag + ">";
		}

		#region ICoverageElement Members

		public CodePosition Position { get; private set; }

		public CoverageState State { get; protected set; }

		public string Tag { get; private set; }

		public string RelativePath { get; private set; }

		public virtual void UpdateState(CoverageState state) {
			State |= state;
		}

		#endregion
	}
}