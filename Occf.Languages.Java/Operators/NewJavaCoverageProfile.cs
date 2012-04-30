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
using System.Xml.Linq;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.XmlToCodes;
using Code2Xml.Languages.Java.CodeToXmls;
using Code2Xml.Languages.Java.XmlToCodes;
using Occf.Core.Operators.Inserters;
using Occf.Core.Operators.Selectors;
using Occf.Core.Operators.Taggers;
using Occf.Languages.Java.Operators.Inserters;

namespace Occf.Languages.Java.Operators {
	public abstract class NewJavaCoverageProfile {
		public string FilePattern {
			get { return "*.java"; }
		}

		public string Name {
			get { return "Java"; }
		}

		public CodeToXml CodeToXml {
			get { return JavaCodeToXml.Instance; }
		}

		public XmlToCode XmlToCode {
			get { return JavaXmlToCode.Instance; }
		}

		private NodeInserter _nodeInserter;

		public NodeInserter NodeInserter {
			get { return _nodeInserter = new JavaNodeInserter(); }
		}

		/// <summary>
		/// Returns xml elements indicating function declarators.
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> SelectFunctions(XElement root);

		/// <summary>
		/// Returns a string value indicating the function name.
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract string SelectFunctionName(XElement function);

		/// <summary>
		/// Returns xml elements indicating statements.
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> SelectStatements(XElement root);

		/// <summary>
		/// Returns xml elements indicating variable initializers.
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> SelectVariableInitializers(
				XElement root);

		/// <summary>
		/// Returns xml elements indicating branch expressions.
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> SelectBranches(XElement root);

		/// <summary>
		/// Returns xml elements indicating condition expressions.
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> SelectConditions(XElement root);

		/// <summary>
		/// Returns xml elements indicating switch statements.
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> SelectSwitches(XElement root);

		/// <summary>
		/// Returns xml elements indicating the last elements of case labels. (e.g. ':')
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> SelectCaseLabelTails(XElement root);

		/// <summary>
		/// Returns xml elements indicating the foreach statements. (e.g. ':')
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> SelectForeach(XElement root);

		/// <summary>
		/// Returns xml elements indicating the head of the foreach statements. (e.g. ':')
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> SelectForeachHead(XElement root);

		public abstract Selector BranchSelector { get; }
		public abstract Selector ConditionSelector { get; }
		public abstract Selector SwitchSelector { get; }
		public abstract Selector CaseLabelTailSelector { get; }
		public abstract Selector ForeachSelector { get; }
		public abstract Selector ForeachHeadSelector { get; }
		public abstract Selector ForeachTailSelector { get; }

		public abstract Selector TestCaseLabelTailSelector { get; }

		public abstract Tagger Tagger { get; }
		public abstract IEnumerable<string> LibraryNames { get; }
	}
}