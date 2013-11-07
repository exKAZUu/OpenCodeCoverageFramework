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
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Manipulators.Taggers;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Python3.Manipulators.Taggers {
	public class Python3Tagger : Tagger {
		public override List<string> Tag(XElement element) {
			return element.Ancestors()
					.Where(e => e.Name.LocalName == "classdef")
					.Select(e => e.NthElementOrDefault(1))
					.Where(e => e != null)
					.Select(e => e.Value)
					.ToList();
		}
	}
}