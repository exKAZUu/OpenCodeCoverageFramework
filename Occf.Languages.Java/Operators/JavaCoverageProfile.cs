using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.XmlToCodes;
using Occf.Core.Operators.Inserters;
using Occf.Core.Operators.Selectors;
using Occf.Core.Operators.Taggers;

namespace Occf.Languages.Java.Operators
{
	public abstract class JavaCoverageProfile
	{
		public abstract string FilePattern { get; }
		public abstract string Name { get; }

		public abstract CodeToXml CodeToXml { get; }
		public abstract XmlToCode XmlToCode { get; }

		public abstract NodeInserter NodeInserter { get; }

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
		public abstract IEnumerable<XElement> SelectVariableInitializers(XElement root);

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
