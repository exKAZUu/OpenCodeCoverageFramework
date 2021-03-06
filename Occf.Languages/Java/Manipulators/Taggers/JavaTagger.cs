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

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.Manipulators.Taggers;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Java.Manipulators.Taggers {
	public class JavaTagger : Tagger {
		public static List<string> GetTag(XElement element) {
			var result = element.Ancestors("compilationUnit")
					.Elements("packageDeclaration")
					.Select(e => e.ElementAt(1).Value)
					.ToList();
			result.AddRange(
					element.AncestorsAndSelf()
							.Where(
									e => e.Name.LocalName == "normalClassDeclaration" ||
											e.Name.LocalName == "methodDeclaration")
							// ReSharper disable PossibleNullReferenceException
							.Select(e => e.Element("IDENTIFIER").Value)
							// ReSharper restore PossibleNullReferenceException
							.Reverse());
			return result;
		}

		public override List<string> Tag(XElement element) {
            return GetTag(element);
		}
	}
}