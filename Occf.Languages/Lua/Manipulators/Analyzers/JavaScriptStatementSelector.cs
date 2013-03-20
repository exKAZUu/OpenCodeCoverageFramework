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
using Occf.Core.Operators.Selectors;
using Paraiba.Xml.Linq;

namespace Occf.Languages.JavaScript.Operators.Selectors {
    public class JavaScriptStatementSelector : Selector {
        public override IEnumerable<XElement> Select(XElement root) {
            return root.Descendants("statement").Where(
                e => {
                    var localName = e.Firstelement().Name.LocalName;

                    // ブロック自身は意味を持たないステートメントで、中身だけが必要なので除外
                    if (localName == "statementBlock") {
                        return false;
                    }
                    // 空行は意味を持たないステートメントなので、除外
                    if (localName == "emptyStatement") {
                        return false;
                    }
                    // ラベル自身は意味を持たないステートメントで、中身だけが必要なので除外
                    if (localName == "labelledStatement") {
                        return false;
                    }
                    // try自信は意味を持たないステートメントで、中身だけが必要なので除外
                    if (localName == "tryStatement") {
                        return false;
                    }

                    return true;
                });
        }
    }
}