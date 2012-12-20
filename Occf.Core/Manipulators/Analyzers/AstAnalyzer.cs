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

namespace Occf.Languages.Java.Operators {
	public abstract class AstAnalyzer {
		/// <summary>
		/// Returns xml elements indicating function declarators.
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> FindFunctions(XElement root);

		/// <summary>
		/// Returns a string value indicating the function name.
		/// </summary>
		/// <param name="functionElement">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract string GettFunctionName(XElement functionElement);

		/// <summary>
		/// Returns xml elements indicating statements.
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> FindStatements(XElement root);

		/// <summary>
		/// Returns xml elements indicating variable initializers.
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> FindVariableInitializers(XElement root);

		/// <summary>
		/// Returns xml elements indicating branch expressions.
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> FindBranches(XElement root);

		/// <summary>
		/// Returns xml elements indicating condition expressions.
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> FindConditions(XElement root);

		/// <summary>
		/// Returns xml elements indicating switch statements.
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> FindSwitches(XElement root);

		/// <summary>
		/// Returns xml elements indicating the last elements of case labels. (e.g. ':')
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> FindCaseLabelTails(XElement root);

		/// <summary>
		/// Returns xml elements indicating the foreach statements.
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> FindForeach(XElement root);

		/// <summary>
		/// Returns xml elements indicating the head of the foreach statements.
		/// </summary>
		/// <param name="foreachElement">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> FindForeachHead(XElement foreachElement);

		/// <summary>
		/// Returns xml elements indicating the tail of the foreach statements.
		/// </summary>
		/// <param name="foreachElement">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> FindForeachTail(XElement foreachElement);

		/// <summary>
		/// Returns xml elements indicating the test cases.
		/// </summary>
		/// <param name="foreachElement">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> FindTestCases(XElement root);
	}
}