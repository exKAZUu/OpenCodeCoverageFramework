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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Languages.Python3.CodeToXmls;
using Code2Xml.Languages.Python3.XmlToCodes;
using Occf.Core.Manipulators.Transformers;
using Occf.Core.TestInformation;

namespace Occf.Languages.Python3.Manipulators.Transformers {
	public class Python3AstTransformer : AstTransformer {
		public override void InsertImport(XElement target) {
			var code = "import CoverageWriter";
			var ast = Python3CodeToXml.Instance.Generate(code);
			target.AddFirst(ast);
		}

		protected override IEnumerable<XElement> CreateStatementNode(
				XElement target, long id, int value, ElementType type) {
			var code = "CoverageWriter.WriteStatement(" + id + "," + (int)type + ","
					+ value + ");";
			if (target.Name.LocalName == "small_stmt") {
				yield return Python3CodeToXml.Instance.Generate(code)
						.Descendants(target.Name)
						.First();
				yield return new XElement("SEMI", ";");
			} else {
				var node = Python3CodeToXml.Instance.Generate(code)
						.Descendants("simple_stmt")
						.First();
				yield return node;
			}
		}

		public override void InsertPredicate(
				XElement target, long id, ElementType type) {
			var oldcode = Python3XmlToCode.Instance.Generate(target);
			var code = "CoverageWriter.WritePredicate(" + id + "," + (int)type + ","
					+ oldcode + ")";
			var node = Python3CodeToXml.Instance.Generate(code)
					.Descendants(target.Name)
					.First();
			target.AddBeforeSelf(node);
			target.Remove();
		}

		public override void InsertInitializer(
				XElement target, long id, ElementType type) {}

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

		public override void SupplementDefaultCase(XElement root) {}

		public override void SupplementDefaultConstructor(XElement root) {}

		public override TestCase InsertTestCaseId(
				XElement target, long id, string relativePath) {
			throw new NotImplementedException();
		}
	}
}