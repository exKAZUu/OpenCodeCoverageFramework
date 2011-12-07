using System.Xml.Linq;
using Occf.Core.Operators.Selectors;
using Occf.Core.TestInfos;

namespace Occf.Core.Operators.Inserters {
	public static class TestCaseInserter {
		public static void Insert(TestInfo info, XElement root,
		                          Selector selector, NodeInserter nodeGen,
		                          string relativePath) {
			var targets = selector.Select(root);
			foreach (var target in targets) {
				var id = info.TestCases.Count;
				var testCase = nodeGen.InsertTestCaseId(target, id, relativePath);
				info.TestCases.Add(testCase);
			}
		}
	}
}