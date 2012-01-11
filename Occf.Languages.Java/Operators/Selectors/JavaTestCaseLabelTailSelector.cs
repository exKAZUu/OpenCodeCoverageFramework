#region License

// Copyright (C) 2011-2012 Kazunori Sakamoto
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
using Occf.Core.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Java.Operators.Selectors {
    public class JavaTestCaseLabelTailSelector : Selector {
        public override IEnumerable<XElement> Select(XElement root) {
            return root.Descendants()
                    .Where(e => e.Name.LocalName == "methodDeclaration")
                    .Where(
                            e => {
                                var name = e.NthElement(2).Value;
                                if (name.StartsWith("test")) {
                                    return true;
                                }
                                var annotation =
                                        e.FirstElement().Element("annotation");
                                if (annotation != null
                                    && annotation.Value == "@Test") {
                                    return true;
                                }
                                return false;
                            });
        }
    }
}