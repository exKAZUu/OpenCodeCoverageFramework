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
using Occf.Core.Manipulators;
using Occf.Core.Manipulators.Analyzers;
using Paraiba.Xml.Linq;

namespace Occf.Languages.Ruby18.Manipulators.Analyzers {
    public class Ruby18AstAnalyzer : AstAnalyzer<Ruby18AstAnalyzer> {
        private static readonly string[] TargetNames = {
                "conditionalOrExpression", "conditionalAndExpression",
        };

        private static readonly string[] ParentNames = {
                "conditionalOrExpression", "conditionalAndExpression",
                "parExpression",
        };

        public override IEnumerable<XElement> FindFunctions(XElement root) {
            // TODO: Implement
            yield break;
        }

        public override string GetFunctionName(XElement functionElement) {
            // TODO: Implement
            throw new NotImplementedException();
        }

        public override IEnumerable<XElement> FindStatements(XElement root) {
            return root.DescendantsAndSelf("block")
                    .SelectMany(e => e.Elements())
                    .Where(e => e.Name() != "block");
        }

        public override IEnumerable<XElement> FindVariableInitializers(XElement root) {
            // TODO: Implement
            yield break;
        }

        public override IEnumerable<XElement> FindBranches(XElement root) {
            return root.Descendants("if")
                    .Select(e => e.FirstElement());
        }

        protected override bool IsConditionalTerm(XElement element) {
            return TargetNames.Contains(element.Name());
        }

        protected override bool IsAvailableParent(XElement element) {
            return element.Elements().Count() == 1 ||
                    ParentNames.Contains(element.Name());
        }

        public override Tuple<XElement, XElement, ComparatorType> GetComparedElements(XElement root) {
            throw new NotImplementedException();
        }

        public override IEnumerable<XElement> FindSwitches(XElement root) {
            // TODO: Implement
            yield break;
        }

        public override IEnumerable<XElement> FindCaseLabelTails(XElement root) {
            // TODO: Implement
            yield break;
        }

        public override IEnumerable<XElement> FindForeach(XElement root) {
            // TODO: Implement
            yield break;
        }

        public override IEnumerable<XElement> FindForeachHead(XElement foreachElement) {
            // TODO: Implement
            yield break;
        }

        public override IEnumerable<XElement> FindForeachTail(XElement foreachElement) {
            // TODO: Implement
            yield break;
        }

        public override IEnumerable<XElement> FindTestCases(XElement root) {
            // TODO: Implement
            yield break;
        }
    }
}