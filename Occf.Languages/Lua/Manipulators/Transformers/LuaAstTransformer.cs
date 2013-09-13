#region License

// Copyright (C) 2009-2013 Kazunori Sakamoto
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
using System.Xml.Linq;
using Code2Xml.Core.Antlr;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.XmlToCodes;
using Code2Xml.Languages.Lua.CodeToXmls;
using Code2Xml.Languages.Lua.XmlToCodes;
using Occf.Core.Manipulators.Transformers;
using Occf.Core.TestInformation;
using Occf.Languages.Lua.Manipulators.Analyzers;

namespace Occf.Languages.Lua.Manipulators.Transformers {
    public class LuaAstTransformer : AntlrAstTransformer<LuaParser> {
        protected override string MethodPrefix {
            get { return ""; }
        }

        protected override AntlrCodeToXml<LuaParser> CodeToXml {
            get { return LuaCodeToXml.Instance; }
        }

        protected override XmlToCode XmlToCode {
            get { return LuaXmlToCode.Instance; }
        }

        protected override Func<LuaParser, XAstParserRuleReturnScope> ParseStatementFunc {
            get { return p => p.stat(); }
        }

        public override void InsertImport(XElement target) {}

        public override void SupplementBlock(XElement root) {
        }

        public override void SupplementDefaultCase(XElement root) {
            // TODO: Implement
        }

        public override void SupplementDefaultConstructor(XElement root) {
            // TODO: Implement
        }

        public override TestCase InsertTestCaseId(XElement target, long id, string relativePath) {
            // TODO: Implement
            throw new NotImplementedException();
        }

        protected override IEnumerable<XElement> FindLackingBlockNodes(XElement root) {
            yield break;
        }
    }
}