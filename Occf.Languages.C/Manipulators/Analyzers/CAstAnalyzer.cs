using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Occf.Languages.Java.Operators;

namespace Occf.Languages.C.Manipulators.Analyzers
{
	public class CAstAnalyzer : AstAnalyzer
	{
		public override IEnumerable<XElement> FindFunctions(XElement root) {
			throw new NotImplementedException();
		}

		public override string GettFunctionName(XElement functionElement) {
			throw new NotImplementedException();
		}

		public override IEnumerable<XElement> FindStatements(XElement root) {
			throw new NotImplementedException();
		}

		public override IEnumerable<XElement> FindVariableInitializers(XElement root) {
			throw new NotImplementedException();
		}

		public override IEnumerable<XElement> FindBranches(XElement root) {
			throw new NotImplementedException();
		}

		public override IEnumerable<XElement> FindConditions(XElement root) {
			throw new NotImplementedException();
		}

		public override IEnumerable<XElement> FindSwitches(XElement root) {
			throw new NotImplementedException();
		}

		public override IEnumerable<XElement> FindCaseLabelTails(XElement root) {
			throw new NotImplementedException();
		}

		public override IEnumerable<XElement> FindForeach(XElement root) {
			throw new NotImplementedException();
		}

		public override IEnumerable<XElement> FindForeachHead(XElement foreachElement) {
			throw new NotImplementedException();
		}

		public override IEnumerable<XElement> FindForeachTail(XElement foreachElement) {
			throw new NotImplementedException();
		}

		public override IEnumerable<XElement> FindTestCases(XElement root) {
			throw new NotImplementedException();
		}
	}
}
