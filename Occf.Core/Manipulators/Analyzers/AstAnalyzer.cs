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
using Paraiba.Linq;
using Paraiba.Xml.Linq;

namespace Occf.Core.Manipulators.Analyzers {
	public abstract class AstAnalyzer<TAstAnalyzer> : AstAnalyzer
			where TAstAnalyzer : AstAnalyzer<TAstAnalyzer>, new() {
		private static TAstAnalyzer _analyzer;

		public static TAstAnalyzer Instance {
			get { return (_analyzer = _analyzer ?? new TAstAnalyzer()); }
		}
	}

	/// <summary>
	/// A class for finding the elements where instrumentation code is inserted. 
	/// </summary>
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
		public abstract string GetFunctionName(XElement functionElement);

		/// <summary>
		/// Returns xml elements indicating statements.
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> FindStatements(XElement root);

        /// <summary>
        /// Returns xml element for locating a proper position of the specified statement.
        /// </summary>
        /// <param name="statement">The root element where start to find elements.</param>
        /// <returns>The selected xml element.</returns>
	    public virtual XElement GetBaseElementForStatement(XElement statement) {
	        return statement;
	    }

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

        ///// <summary>
        ///// Returns xml elements indicating a condition expression.
        ///// </summary>
        ///// <param name="root">The root element where start to find elements.</param>
        ///// <returns>The selected xml elements.</returns>
        //public abstract IEnumerable<XElement> FindCondition(XElement root); 
        
		/// <summary>
		/// Returns xml elements indicating condition expressions.
		/// </summary>
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public IEnumerable<XElement> FindConditions(XElement root) {
			var targetParents = root.Descendants()
					.Where(IsConditionalTerm)
					.Where(e => e.Elements().Count() >= 3)
					.Where(e => e.ParentsWhile(root).All(IsAvailableParent));
			var targets = targetParents
					.SelectMany(e => e.Elements().OddIndexElements());

			// 他の項の要素を含まない項の要素のみを抽出
			// a == b && (a == c || a == d) => a == b, a == c, a == d
			var atomicTargets = targets.Independents().ToList();

			// XML要素の位置でソーティング
			atomicTargets.Sort((e1, e2) => e1.IsBefore(e2) ? -1 : 1);

			return atomicTargets;
		}

		/// <summary>
		/// Returns the value indicating whether the specified element is a conditional term.
		/// </summary>
		/// <param name="element">The element to be judged.</param>
		/// <returns>The value indicating whether the specified element is a conditional term.</returns>
		protected abstract bool IsConditionalTerm(XElement element);

		/// <summary>
		/// Returns the value indicating whether the specified element is an available parent.
		/// </summary>
		/// <param name="element">The element to be judged.</param>
		/// <returns>The value indicating whether the specified element is an available parent.</returns>
		protected abstract bool IsAvailableParent(XElement element);

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
		/// <param name="root">The root element where start to find elements.</param>
		/// <returns>The selected xml elements.</returns>
		public abstract IEnumerable<XElement> FindTestCases(XElement root);
	}
}