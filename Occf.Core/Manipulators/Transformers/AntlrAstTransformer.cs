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
using Antlr.Runtime;
using Code2Xml.Core.Antlr;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.XmlToCodes;
using Paraiba.Xml.Linq;

namespace Occf.Core.Manipulators.Transformers {
    public abstract class AntlrAstTransformer<TParser> : AstTransformer
            where TParser : Parser, IAntlrParser {
        protected abstract string MethodPrefix { get; }
        protected abstract AntlrCodeToXml<TParser> CodeToXml { get; }
        protected abstract XmlToCode XmlToCode { get; }
        protected abstract Func<TParser, XAstParserRuleReturnScope> ParseStatementFunc { get; }

        protected virtual string CreateTestCaseIdentifierCode(
                XElement target, long id, int value, ElementType type) {
            return MethodPrefix + "WriteTestCase(" + id + "," + (int)type + "," + value + ");";
        }

        protected virtual string CreateStatementCoverageCode(
                XElement target, long id, int value, ElementType type) {
            return MethodPrefix + "WriteStatement(" + id + "," + (int)type + "," + value + ");";
        }

        protected virtual Tuple<string, string> CreatePredicateCoverageCode(
                XElement target, long id, ElementType type) {
            return Tuple.Create(MethodPrefix + "WritePredicate(" + id + "," + (int)type + ",", ")");
        }

        protected virtual Tuple<string, string, string> CreateInitializerCoverageCode(
                XElement target, long id, ElementType type) {
            var stmt = CreateStatementCoverageCode(target, id, 2, type);
            return Tuple.Create(stmt.Substring(0, stmt.Length - 1) + " ? (", ") : (", ")");
        }

        protected override IEnumerable<XElement> CreateStatementNode(
                XElement target, long id, int value, ElementType type) {
            var code = CreateStatementCoverageCode(target, id, value, type);
            //var node = CodeToXml.Generate(code, ParseStatementFunc);
            var node = new XElement("TOKEN", code);
            yield return node;
        }

        public override void InsertPredicate(XElement target, long id, ElementType type) {
            var code = CreatePredicateCoverageCode(target, id, type);
            //var node = AntlrNodeGenerator.GenerateWrappedNode(
            //        target,
            //        CodeToXml, XmlToCode,
            //        code.Item1, code.Item2);
            //target.AddBeforeSelf(node);
            //target.Remove();
            target.AddBeforeSelf(new XElement("TOKEN", code.Item1));
            target.AddAfterSelf(new XElement("TOKEN", code.Item2));
        }

        public override void InsertInitializer(XElement target, long id, ElementType type) {
            var code = CreateInitializerCoverageCode(target, id, type);
            //var node = AntlrNodeGenerator.GenerateWrappedNode(
            //        target,
            //        CodeToXml,
            //        XmlToCode,
            //        code.Item1, code.Item2, code.Item3);
            //target.AddBeforeSelf(node);
            //target.Remove();
            target.AddBeforeSelf(new XElement("TOKEN", code.Item1));
            target.AddAfterSelf(new XElement("TOKEN", code.Item2));
        }

        public override void InsertEqual(XElement target, XElement left, XElement right, long id, ElementType type) {
            var code = MethodPrefix + "WriteEqual(" + id + "," + (int)type + "," + left.Value + ","
                    + right.Value + ")";
            target.AddBeforeSelf(new XElement("TOKEN", code));
            target.Remove();
        }

        public override void InsertNotEqual(XElement target, XElement left, XElement right, long id, ElementType type) {
            var code = MethodPrefix + "WriteNotEqual(" + id + "," + (int)type + "," + left.Value + ","
                    + right.Value + ")";
            target.AddBeforeSelf(new XElement("TOKEN", code));
            target.Remove();
        }

        public override void InsertLessThan(XElement target, XElement left, XElement right, long id, ElementType type) {
            var code = MethodPrefix + "WriteLessThan(" + id + "," + (int)type + "," + left.Value + ","
                    + right.Value + ")";
            target.AddBeforeSelf(new XElement("TOKEN", code));
            target.Remove();
        }

        public override void InsertGraterThan(XElement target, XElement left, XElement right, long id, ElementType type) {
            var code = MethodPrefix + "WriteGraterThan(" + id + "," + (int)type + "," + left.Value + ","
                    + right.Value + ")";
            target.AddBeforeSelf(new XElement("TOKEN", code));
            target.Remove();
        }

        public void SupplementBlock(
                XElement root, string elementName, string begin, string end) {
            var nodes = FindLackingBlockNodes(root)
                .Where(e => e.Name.LocalName != elementName)
                .Where(e => e.FirstElementOrDefault(elementName) == null || e.Elements().Skip(1).Any());
            ReplaceSafely(
                    nodes,
                    node =>
                            new XElement(elementName, new XElement("TOKEN", begin), node,
                                    new XElement("TOKEN", end)));
        }

        protected abstract IEnumerable<XElement> FindLackingBlockNodes(XElement root);
    }
}