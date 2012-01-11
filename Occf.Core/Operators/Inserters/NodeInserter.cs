using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Occf.Core.TestInfos;
using Paraiba.Collections.Generic;
using Paraiba.Utility;
using Paraiba.Xml.Linq;

namespace Occf.Core.Operators.Inserters {
	public abstract class NodeInserter {
		public abstract void InsertImport(XElement target);

		protected abstract IEnumerable<XElement> CreateStatementNode(
				XElement target, int id, int value, ElementType type);

		public void InsertStatementFirstChildren(
				XElement target, int id, int value, ElementType type) {
			target.AddFirst(CreateStatementNode(target, id, value, type));
		}

		public void InsertStatementLastChildren(
				XElement target, int id, int value, ElementType type) {
			target.Add(CreateStatementNode(target, id, value, type));
		}

		public void InsertStatementBefore(
				XElement target, int id, int value, ElementType type) {
			target.AddBeforeSelf(CreateStatementNode(target, id, value, type));
		}

		public void InsertStatementAfter(
				XElement target, int id, int value, ElementType type) {
			target.AddAfterSelf(CreateStatementNode(target, id, value, type));
		}

		public abstract void InsertPredicate(
				XElement target, int id, ElementType type);

		public abstract void InsertInitializer(
				XElement target, int id, ElementType type);

		public abstract void SupplementBlock(XElement root);

		public abstract void SupplementDefaultCase(XElement root);

		public abstract void SupplementDefaultConstructor(XElement root);

	    public abstract TestCase InsertTestCaseId(
	            XElement target, int id, string relativePath);

		public static void ReplaceSafely(
				IEnumerable<XElement> nodes,
				Func<XElement, XElement> createNodeFunc) {
			var sortedDict = GetElementListsOrderedByDepth(nodes);

			foreach (var list in sortedDict.Values) {
				foreach (var node in list) {
					node.AddAfterSelf(createNodeFunc(node));
					node.Remove();
				}
			}
		}

		private static SortedDictionary<int, List<XElement>>
				GetElementListsOrderedByDepth(IEnumerable<XElement> nodes) {
			var cmp = Make.Comparer<int>((v1, v2) => v2 - v1);
			var sortedDict = new SortedDictionary<int, List<XElement>>(cmp);
			foreach (var element in nodes) {
				var depth = element.Depth();
				var list = sortedDict.GetValueOrDefault(depth);
				if (list == null) {
					list = new List<XElement>();
					sortedDict.Add(depth, list);
				}
				list.Add(element);
			}
			return sortedDict;
		}
	}
}