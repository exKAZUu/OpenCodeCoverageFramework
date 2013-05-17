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
using Code2Xml.Languages.Java.CodeToXmls;
using Code2Xml.Languages.Java.XmlToCodes;
using Occf.Core.Manipulators.Transformers;
using Occf.Core.TestInformation;
using Occf.Languages.Java.Manipulators.Analyzers;
using Occf.Languages.Java.Manipulators.Taggers;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Java.Manipulators.Transformers {
    public class JavaAstTransformer : AntlrAstTransformer<JavaParser> {
        protected override string MethodPrefix {
            get { return ""; }
        }

        protected override AntlrCodeToXml<JavaParser> CodeToXml {
            get { return JavaCodeToXml.Instance; }
        }

        protected override XmlToCode XmlToCode {
            get { return JavaXmlToCode.Instance; }
        }

        protected override Func<JavaParser, XAstParserRuleReturnScope> ParseStatementFunc {
            get { return p => p.statement(); }
        }

        public override void InsertImport(XElement target) {
            var ast = new XElement("TOKEN",
                    "import static jp.ac.waseda.cs.washi.CoverageWriter.*;\r\n");
            target.AddFirst(ast);
        }

        public override void SupplementBlock(XElement root) {
            SupplementBlock(root, "block", "{", "}");
        }

        public override void SupplementDefaultCase(XElement root) {
            var targets = FindLackingDefaultCaseNodes(root);
            foreach (var target in targets) {
                //var node = JavaCodeToXml.Instance.GenerateWithoutPosition(
                //        "default:", p => p.switchBlockStatementGroup());
                target.AddAfterSelf(new XElement("TOKEN", "default:"));
            }
        }

        private static IEnumerable<XElement> FindLackingDefaultCaseNodes(XElement root) {
            foreach (var switchNode in JavaAstAnalyzer.Instance.FindSwitches(root)) {
                // ケース文がないときは分岐していないと見なす
                var last = JavaAstAnalyzer.Instance.FindCaseLabelTails(switchNode).LastOrDefault();
                if (last != null && last.Parent.FirstElement().Value != "default") {
                    yield return last.Parent.Parent;
                }
            }
        }

        public override void SupplementDefaultConstructor(XElement root) {
            throw new NotImplementedException();
        }

        public override TestCase InsertTestCaseId(XElement target, long id, string relativePath) {
            var testCase = new TestCase(
                    relativePath, string.Join(".", JavaTagger.GetTag(target)), target);
            var blockElement = target.Element("block").NthElement(1);
            var code = CreateTestCaseIdentifierCode(target, id, 2, ElementType.TestCase);
            var node = JavaCodeToXml.Instance.GenerateWithoutPosition(code, p => p.statement());
            if (blockElement.Name.LocalName == "blockStatement") {
                blockElement.AddFirst(node);
            } else {
                blockElement.AddBeforeSelf(node);
            }
            return testCase;
        }

        protected override IEnumerable<XElement> FindLackingBlockNodes(XElement root) {
            SupplementEmptyMethod(root);

            var ifs = JavaElements.If(root)
                    .SelectMany(JavaElements.IfAndElseProcesses);
            var whiles = JavaElements.While(root)
                    .Select(JavaElements.WhileProcess);
            var dos = JavaElements.DoWhile(root)
                    .Select(JavaElements.DoWhileProcess);
            var fors = JavaElements.For(root)
                    .Select(JavaElements.ForProcess);

            return ifs.Concat(whiles)
                    .Concat(dos)
                    .Concat(fors);
        }

        private static void SupplementEmptyMethod(XElement root) {
            var methods = root.Descendants("methodDeclaration")
                    .Where(e => e.Elements().Any(e2 => e2.Value == "void"))
                    .Select(e => e.Element("block"))
                    .Where(e => e != null)
                    .Where(e => !(e.Descendants("statement").Any() &&
                            e.Descendants("statement")
                                    .Last()
                                    .FirstElement()
                                    .Value == "throw"))
                    .Select(e => e.LastElement());
            var node = JavaCodeToXml.Instance.GenerateWithoutPosition("return;", p => p.statement());
            foreach (var method in methods) {
                method.AddBeforeSelf(node);
            }
        }
    }
}