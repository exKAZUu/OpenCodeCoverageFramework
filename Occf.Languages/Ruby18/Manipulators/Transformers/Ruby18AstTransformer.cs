﻿#region License

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
using System.Xml.Linq;
using Occf.Core.Manipulators.Transformers;
using Occf.Core.TestInformation;

namespace Occf.Languages.Ruby18.Manipulators.Transformers {
    public class Ruby18AstTransformer : AstTransformer {
        public override void InsertImport(XElement target) {}

        protected override IEnumerable<XElement> CreateStatementNode(
                XElement target, long id, int value, ElementType type) {
            yield return new XElement("call",
                    new object[] {
                            new XElement("nil"),
                            new XElement("Symbol", "stmt"),
                            new XElement("lit", new[] {
                                    new XElement("Fixnum", id),
                            }),
                            new XElement("lit", new[] {
                                    new XElement("Fixnum", (int)type),
                            }),
                            new XElement("lit", new[] {
                                    new XElement("Fixnum", value),
                            }),
                    });
        }

        public override void InsertPredicate(XElement target, long id, ElementType type) {
            var node = new XElement("call", new object[] {
                    new XElement("nil"),
                    new XElement("Symbol", "branch"),
                    new XElement("lit", new[] {
                            new XElement("Fixnum", id),
                    }),
                    new XElement("lit", new[] {
                            new XElement("Fixnum", (int)type),
                    }),
                    target,
            });
            target.AddBeforeSelf(node);
            target.Remove();
        }

        public override void InsertInitializer(XElement target, long id, ElementType type) {
            throw new NotImplementedException();
        }

        public override void InsertEqual(XElement target, XElement left, XElement right, long id, ElementType type) {
            throw new NotImplementedException();
        }

        public override void InsertNotEqual(XElement target, XElement left, XElement right, long id, ElementType type) {
            throw new NotImplementedException();
        }

        public override void InsertLessThan(XElement target, XElement left, XElement right, long id, ElementType type) {
            throw new NotImplementedException();
        }

        public override void InsertGraterThan(XElement target, XElement left, XElement right, long id, ElementType type) {
            throw new NotImplementedException();
        }

        public override void SupplementBlock(XElement root) {}

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
    }
}