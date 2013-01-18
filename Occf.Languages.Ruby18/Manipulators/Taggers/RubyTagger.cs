﻿#region License

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

using System.Xml.Linq;
using Occf.Core.Manipulators.Taggers;

namespace Occf.Languages.Ruby18.Manipulators.Taggers {
	public class RubyTagger : Tagger {
		public override string Tag(XElement elements) {
			var tag = "";
			//var classNodes = elements.Parents()
			//    .Where(e => e.Name.LocalName == "classdef");
			//foreach (var classNode in classNodes) {
			//    var node = classNode.ElementAtOrDefault(1);
			//    if (node == null)
			//        continue;
			//    tag += node.Value + '>';
			//}
			return tag;
		}
	}
}