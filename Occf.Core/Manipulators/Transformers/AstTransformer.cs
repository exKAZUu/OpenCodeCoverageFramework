﻿#region License

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
using System.Xml.Linq;
using Occf.Core.TestInformation;
using Paraiba.Collections.Generic;
using Paraiba.Utility;
using Paraiba.Xml.Linq;

namespace Occf.Core.Manipulators.Transformers {
	public abstract class AstTransformer {
		public abstract void InsertImport(XElement target);

		protected abstract IEnumerable<XElement> CreateStatementNode(
				XElement target, long id, int value, ElementType type);

		public void InsertStatementFirstChildren(
				XElement target, long id, int value, ElementType type) {
			target.AddFirst(CreateStatementNode(target, id, value, type));
		}

		public void InsertStatementLastChildren(
				XElement target, long id, int value, ElementType type) {
			target.Add(CreateStatementNode(target, id, value, type));
		}

		public void InsertStatementBefore(
				XElement target, long id, int value, ElementType type) {
			target.AddBeforeSelf(CreateStatementNode(target, id, value, type));
		}

		public void InsertStatementAfter(
				XElement target, long id, int value, ElementType type) {
			target.AddAfterSelf(CreateStatementNode(target, id, value, type));
		}

		public abstract void InsertPredicate(
				XElement target, long id, ElementType type);

		public abstract void InsertInitializer(
				XElement target, long id, ElementType type);

		public abstract void InsertEqual(
				XElement target, XElement left, XElement right, long id, ElementType type);

		public abstract void InsertNotEqual(
				XElement target, XElement left, XElement right, long id, ElementType type);

		public abstract void InsertLessThan(
				XElement target, XElement left, XElement right, long id, ElementType type);

		public abstract void InsertGraterThan(
				XElement target, XElement left, XElement right, long id, ElementType type);

		public abstract void SupplementBlock(XElement root);

		public abstract void SupplementDefaultCase(XElement root);

		public abstract void SupplementDefaultConstructor(XElement root);

		public abstract TestCase InsertTestCaseId(
				XElement target, long id, string relativePath);

		/// <summary>
		/// Replace the specified nodes with nodes generated by the specified function 
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="createNodeFunc"></param>
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