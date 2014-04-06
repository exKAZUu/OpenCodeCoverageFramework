#region License

// Copyright (C) 2011-2014 Kazunori Sakamoto
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
using System.Numerics;
using System.Xml.Linq;
using Code2Xml.Core;

namespace Occf.Learner.Core.Tests {
    public static class PredicateGenerator {
        public static string BigIntegerToString(BigInteger i, int width) {
            var ret = "";
            while (i != BigInteger.Zero) {
                if ((i & BigInteger.One) != BigInteger.Zero) {
                    ret = "1" + ret;
                } else {
                    ret = "0" + ret;
                }
                width--;
                i >>= 1;
            }
            while (--width >= 0) {
                ret = "0" + ret;
            }
            return ret;
        }

        public static BigInteger StringToBigInteger(string s) {
            var ret = BigInteger.Zero;
            foreach (var ch in s) {
                ret <<= 1;
                if (ch == '1') {
                    ret |= BigInteger.One;
                }
            }
            return ret;
        }

        public static HashSet<string> GetUnionKeys(
                this IEnumerable<XElement> targets, int length, bool inner = true, bool outer = true) {
            var commonKeys = new HashSet<string>();
            foreach (var target in targets) {
                var keys = target.GetSurroundingKeys(length, inner, outer);
                commonKeys.UnionWith(keys);
            }
            return commonKeys;
        }

        public static HashSet<string> GetCommonKeys(
                this IEnumerable<XElement> targets, int length, bool inner = true, bool outer = true) {
            HashSet<string> commonKeys = null;
            foreach (var target in targets) {
                var keys = target.GetSurroundingKeys(length, inner, outer);
                if (commonKeys == null) {
                    commonKeys = keys;
                } else {
                    commonKeys.IntersectWith(keys);
                }
            }
            return commonKeys;
        }

        public static HashSet<string> GetSurroundingKeys(
                this XElement element, int length, bool inner = true, bool outer = true) {
            //inner = outer = true;

            var ret = new HashSet<string>();
            var childElements = new List<Tuple<XElement, string>>();
            if (inner) {
                childElements.Add(Tuple.Create(element, element.Name()));
                var ancestorStr = "";
                foreach (var e in element.AncestorsOfOnlyChildAndSelf()) {
                    ancestorStr = ancestorStr + "<" + e.NameWithId();
                    ret.Add(ancestorStr);
                }
            }
            var i = 1;
            if (outer) {
                var parentElement = Tuple.Create(element, element.Name());
                var descendantStr = "";
                foreach (var e in element.DescendantsOfOnlyChildAndSelf()) {
                    descendantStr = descendantStr + "<" + e.NameWithId();
                    ret.Add(descendantStr);
                }
                // 自分自身の位置による区別も考慮する
                ret.Add(element.NameOrTokenWithId());
                for (; i <= length; i++) {
                    var newChildElements = new List<Tuple<XElement, string>>();
                    foreach (var t in childElements.Where(t2 => !t2.Item1.IsTokenSet())) {
                        foreach (var e in t.Item1.Elements()) {
                            var key = t.Item2 + ">" + e.NameOrTokenWithId();
                            newChildElements.Add(Tuple.Create(e, key));
                            // トークンが存在するかチェックする弱い条件
                            // for Preconditions.checkArguments()
                            ret.Add(t.Item2 + ">'" + e.TokenText() + "'");
                        }
                        foreach (var e in t.Item1.Descendants().Where(e => e.IsTokenSet())) {
                            // トークンが存在するかチェックする弱い条件
                            //ret.Add(t.Item2 + ">>'" + e.TokenText() + "'");
                        }
                    }
                    foreach (var e in parentElement.Item1.Siblings(10)) {
                        var key = parentElement.Item2 + "-" + e.NameOrTokenWithId();
                        newChildElements.Add(Tuple.Create(e, key));
                        // トークンが存在するかチェックする弱い条件
                        // for Preconditions.checkArguments()
                        ret.Add(parentElement.Item2 + "-'" + e.TokenText() + "'");
                        //// 先祖に存在するかチェックする弱い条件
                        //var iLastName = parentElement.Item2.LastIndexOf("<");
                        //var weakKey = "<<" + parentElement.Item2.Substring(iLastName + 1) + "-" + e.NameOrTokenWithId();
                        //newChildElements.Add(Tuple.Create(e, weakKey));
                    }
                    ret.UnionWith(newChildElements.Select(t => t.Item2));
                    childElements = newChildElements;

                    var newParentElement = parentElement.Item1.Parent;
                    if (newParentElement == null) {
                        break;
                    }
                    parentElement = Tuple.Create(
                            newParentElement,
                            parentElement.Item2 + "<" + newParentElement.NameOrTokenWithId());
                    ret.Add(parentElement.Item2);
                }
            }
            for (; i <= length; i++) {
                var newChildElements = new List<Tuple<XElement, string>>();
                foreach (var t in childElements.Where(t2 => !t2.Item1.IsTokenSet())) {
                    foreach (var e in t.Item1.Elements()) {
                        var key = t.Item2 + ">" + e.NameOrTokenWithId();
                        newChildElements.Add(Tuple.Create(e, key));
                        // トークンが存在するかチェックする弱い条件
                        // for Preconditions.checkArguments()
                        ret.Add(t.Item2 + ">'" + e.TokenText() + "'");
                    }
                }
                ret.UnionWith(newChildElements.Select(t => t.Item2));
                childElements = newChildElements;
            }
            return ret;
        }

        public static BigInteger GetSurroundingBits(
                this XElement element, int length, IDictionary<string, BigInteger> key2Bit,
                bool inner = true,
                bool outer = true) {
            //inner = outer = true;

            var ret = BigInteger.Zero;
            BigInteger bit;
            var childElements = new List<Tuple<XElement, string>>();
            if (inner) {
                childElements.Add(Tuple.Create(element, element.Name()));
                var parentStr = "";
                foreach (var e in element.AncestorsOfOnlyChildAndSelf()) {
                    parentStr = parentStr + "<" + e.NameWithId();
                    if (key2Bit.TryGetValue(parentStr, out bit)) {
                        ret |= bit;
                    }
                }
            }
            var i = 1;
            if (outer) {
                var parentElement = Tuple.Create(element, element.Name());
                var descendantStr = "";
                foreach (var e in element.DescendantsOfOnlyChildAndSelf()) {
                    descendantStr = descendantStr + "<" + e.NameWithId();
                    if (key2Bit.TryGetValue(descendantStr, out bit)) {
                        ret |= bit;
                    }
                }
                // 自分自身の位置による区別も考慮する
                if (key2Bit.TryGetValue(element.NameOrTokenWithId(), out bit)) {
                    ret |= bit;
                }
                for (; i <= length; i++) {
                    var newChildElements = new List<Tuple<XElement, string>>();
                    foreach (var t in childElements.Where(t2 => !t2.Item1.IsTokenSet())) {
                        foreach (var e in t.Item1.Elements()) {
                            var key = t.Item2 + ">" + e.NameOrTokenWithId();
                            if (key2Bit.TryGetValue(key, out bit)) {
                                newChildElements.Add(Tuple.Create(e, key));
                                ret |= bit;
                            }
                            // トークンが存在するかチェックする弱い条件
                            // for Preconditions.checkArguments()
                            if (key2Bit.TryGetValue(t.Item2 + ">'" + e.TokenText() + "'", out bit)) {
                                ret |= bit;
                            }
                        }
                        foreach (var e in t.Item1.Descendants().Where(e => e.IsTokenSet())) {
                            // トークンが存在するかチェックする弱い条件
                            //ret.Add(t.Item2 + ">>'" + e.TokenText() + "'");
                        }
                    }
                    foreach (var e in parentElement.Item1.Siblings(10)) {
                        var key = parentElement.Item2 + "-" + e.NameOrTokenWithId();
                        if (key2Bit.TryGetValue(key, out bit)) {
                            newChildElements.Add(Tuple.Create(e, key));
                            ret |= bit;
                        }
                        // トークンが存在するかチェックする弱い条件
                        // for Preconditions.checkArguments()
                        if (key2Bit.TryGetValue(
                                parentElement.Item2 + "-'" + e.TokenText() + "'", out bit)) {
                            ret |= bit;
                        }
                        //// 先祖に存在するかチェックする弱い条件
                        //var iLastName = parentElement.Item2.LastIndexOf("<");
                        //var weakKey = "<<" + parentElement.Item2.Substring(iLastName + 1) + "-" + e.NameOrTokenWithId();
                        //newChildElements.Add(Tuple.Create(e, weakKey));
                    }
                    childElements = newChildElements;

                    var newParentElement = parentElement.Item1.Parent;
                    if (newParentElement == null) {
                        break;
                    }
                    parentElement = Tuple.Create(
                            newParentElement,
                            parentElement.Item2 + "<" + newParentElement.NameOrTokenWithId());
                    if (key2Bit.TryGetValue(parentElement.Item2, out bit)) {
                        ret |= bit;
                    } else {
                        break;
                    }
                }
            }
            for (; i <= length; i++) {
                var newChildElements = new List<Tuple<XElement, string>>();
                foreach (var t in childElements.Where(t2 => !t2.Item1.IsTokenSet())) {
                    foreach (var e in t.Item1.Elements()) {
                        var key = t.Item2 + ">" + e.NameOrTokenWithId();
                        if (key2Bit.TryGetValue(key, out bit)) {
                            newChildElements.Add(Tuple.Create(e, key));
                            ret |= bit;
                        }
                        // トークンが存在するかチェックする弱い条件
                        // for Preconditions.checkArguments()
                        if (key2Bit.TryGetValue(t.Item2 + ">'" + e.TokenText() + "'", out bit)) {
                            ret |= bit;
                        }
                    }
                }
                childElements = newChildElements;
            }
            return ret;
        }

        public static IEnumerable<string> test(
                this XElement element, int length, IList<string> predicateKeys,
                List<List<Tuple<XElement, string>>> descendants) {
            var maxLevel = descendants.Count - 1;
            for (int i = 1; i < maxLevel; i++) {
                var tuples = descendants[i];
                foreach (var t in tuples) {
                    yield return t.Item2;
                }
            }
            for (int i = maxLevel; i < length; i++) {
                var tuples = descendants[i];
                foreach (var t in tuples) {
                    foreach (var e in t.Item1.Elements()) {
                        var key = t.Item2 + ">" + e.NameOrTokenWithId();
                        descendants[i + 1].Add(Tuple.Create(e, key));
                        yield return key;
                    }
                }
            }
        }

        public static Dictionary<string, List<XElement>> SurroundingElementsWithSelf(
                this XElement element, int length) {
            var ret = new Dictionary<string, List<XElement>>();
            var childKeys = new List<string> { "" };
            ret[""] = new List<XElement> { element };
            var parentKey = "";
            var parent = element;
            var i = 1;
            for (; i <= length; i++) {
                var newChildKeys = new List<string>();
                foreach (var childKey in childKeys) {
                    foreach (var e in ret[childKey].Where(e2 => !e2.IsTokenSet())) {
                        var key = childKey + e.NameWithId() + ">";
                        ret[key] = e.Elements().ToList();
                        newChildKeys.Add(key);
                    }
                }
                {
                    var key = parentKey + parent.NameWithId() + "-";
                    ret[key] = parent.Siblings(10).ToList();
                    newChildKeys.Add(key);
                }
                childKeys = newChildKeys;
                parentKey += parent.NameWithId() + "<";
                parent = parent.Parent;
                if (parent == null) {
                    break;
                }
                ret[parentKey] = new List<XElement> { parent };
            }
            for (; i <= length; i++) {
                var newChildKeys = new List<string>();
                foreach (var childKey in childKeys) {
                    foreach (var e in ret[childKey].Where(e2 => !e2.IsTokenSet())) {
                        var key = childKey + e.NameWithId() + ">";
                        ret[key] = e.Elements().ToList();
                        newChildKeys.Add(key);
                    }
                }
                childKeys = newChildKeys;
            }
            ret.Remove("");
            return ret;
        }

        public static HashSet<string> GetSurroundingKeys2(
                this XElement element, int length, bool inner = true, bool outer = true) {
            //inner = outer = true;

            var ret = new HashSet<string>();
            var childElements = new List<Tuple<XElement, string>>();
            if (inner) {
                childElements.Add(Tuple.Create(element, element.Name()));
                var ancestorStr = "";
                foreach (var e in element.AncestorsOfOnlyChildAndSelf()) {
                    ancestorStr = ancestorStr + "<" + e.NameWithId();
                    ret.Add(ancestorStr);
                }
            }
            var i = 1;
            if (outer) {
                var parentElement = Tuple.Create(element, element.Name());
                var descendantStr = "";
                foreach (var e in element.DescendantsOfOnlyChildAndSelf()) {
                    descendantStr = descendantStr + "<" + e.NameWithId();
                    ret.Add(descendantStr);
                }
                // 自分自身の位置による区別も考慮する
                ret.Add(element.NameOrTokenWithId());
                for (; i <= length; i++) {
                    var newChildElements = new List<Tuple<XElement, string>>();
                    foreach (var t in childElements.Where(t2 => !t2.Item1.IsTokenSet())) {
                        foreach (var e in t.Item1.Elements()) {
                            var key = t.Item2 + ">" + e.NameOrTokenWithId();
                            newChildElements.Add(Tuple.Create(e, key));
                            // トークンが存在するかチェックする弱い条件
                            // for Preconditions.checkArguments()
                            ret.Add(t.Item2 + ">'" + e.TokenText() + "'");
                        }
                        foreach (var e in t.Item1.Descendants().Where(e => e.IsTokenSet())) {
                            // トークンが存在するかチェックする弱い条件
                            //ret.Add(t.Item2 + ">>'" + e.TokenText() + "'");
                        }
                    }
                    foreach (var e in parentElement.Item1.Siblings(10)) {
                        var key = parentElement.Item2 + "-" + e.NameOrTokenWithId();
                        newChildElements.Add(Tuple.Create(e, key));
                        // トークンが存在するかチェックする弱い条件
                        // for Preconditions.checkArguments()
                        ret.Add(parentElement.Item2 + "-'" + e.TokenText() + "'");
                        //// 先祖に存在するかチェックする弱い条件
                        //var iLastName = parentElement.Item2.LastIndexOf("<");
                        //var weakKey = "<<" + parentElement.Item2.Substring(iLastName + 1) + "-" + e.NameOrTokenWithId();
                        //newChildElements.Add(Tuple.Create(e, weakKey));
                    }
                    ret.UnionWith(newChildElements.Select(t => t.Item2));
                    childElements = newChildElements;

                    var newParentElement = parentElement.Item1.Parent;
                    if (newParentElement == null) {
                        break;
                    }
                    parentElement = Tuple.Create(
                            newParentElement,
                            parentElement.Item2 + "<" + newParentElement.NameOrTokenWithId());
                    ret.Add(parentElement.Item2);
                }
            }
            for (; i <= length; i++) {
                var newChildElements = new List<Tuple<XElement, string>>();
                foreach (var t in childElements.Where(t2 => !t2.Item1.IsTokenSet())) {
                    foreach (var e in t.Item1.Elements()) {
                        var key = t.Item2 + ">" + e.NameOrTokenWithId();
                        newChildElements.Add(Tuple.Create(e, key));
                        // トークンが存在するかチェックする弱い条件
                        // for Preconditions.checkArguments()
                        ret.Add(t.Item2 + ">'" + e.TokenText() + "'");
                    }
                }
                ret.UnionWith(newChildElements.Select(t => t.Item2));
                childElements = newChildElements;
            }
            return ret;
        }
    }
}