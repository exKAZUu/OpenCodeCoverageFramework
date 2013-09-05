using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Paraiba.Linq;
using Paraiba.Xml.Linq;

namespace Occf.Learner {
    public class FindRule {
        public IList<Step> Steps { get; set; }
    }

    public class NodeProperty {}

    public class ChildrenCountNodeProperty : NodeProperty {}

    public class NodeCondition {}

    public class Step {
        public IList<NodeCondition> Conditions { get; set; }
    }

    public class InclusionStep : Step {
        //public void DoStep(XElement ast, ) {

        //}
    }

    public class ExclusionStep : Step {}

    public static class FindLearner {
        public static void Learn(XElement ast, IEnumerable<XElement> elements) {
            var name2elements = new Dictionary<string, HashSet<XElement>>();
            foreach (var element in elements) {
                HashSet<XElement> set;
                if (!name2elements.TryGetValue(element.Name.LocalName, out set)) {
                    set = new HashSet<XElement>();
                    name2elements.Add(element.Name.LocalName, set);
                }
                set.Add(element);
            }

            foreach (var nameAndElements in name2elements) {
                Learn(ast, nameAndElements.Key, nameAndElements.Value);
            }
        }

        public static void Learn(XElement ast, string name, HashSet<XElement> targets) {
            var others = ast.DescendantsAndSelf(name).ToHashSet();
            others.ExceptWith(targets);

            {
                Func<XElement, int> selector = e => e.Elements().Count();
                Learn(targets, selector, others);
            }

            {
                Func<XElement, string> selector =
                        e => string.Join("/", e.Parent.Elements().Select(e2 => e2.NameOrValue()));
                Learn(targets, selector, others);
            }

            {
                Func<XElement, IEnumerable<string>> selector =
                        e => e.Elements().Select(e2 => e2.Name.LocalName);
                Learn2(targets, selector, others);
            }

            {
                Func<XElement, IEnumerable<string>> selector =
                        e => e.Parent.Elements().Select(e2 => e2.Name.LocalName);
                Learn2(targets, selector, others);
            }

            Console.WriteLine(others.Count);
        }

        private static void Learn<T>(IEnumerable<XElement> targets, Func<XElement, T> selector, HashSet<XElement> others) {
            var targetProps = targets.Select(selector).ToHashSet();
            var otherProps = others.Select(selector).ToHashSet();
            otherProps.ExceptWith(targetProps);
            others.RemoveWhere(e => otherProps.Contains(selector(e)));
        }

        private static void Learn2<T>(HashSet<XElement> targets, Func<XElement, IEnumerable<T>> selector, HashSet<XElement> others) {
            var targetProps = targets.SelectMany(selector).ToHashSet();
            var otherProps = others.SelectMany(selector).ToHashSet();
            var requiredProps = targets.Select(selector)
                    .Aggregate((acc, values) => acc.Intersect(values))
                    .ToHashSet();
            requiredProps.ExceptWith(otherProps);
            if (requiredProps.Count > 0) {
                others.RemoveWhere(e => !selector(e).IsIntersect(requiredProps));
            }
            otherProps.ExceptWith(targetProps);
            others.RemoveWhere(e => selector(e).IsIntersect(otherProps));
        }

        public static string NameOrValue(this XElement element) {
            return element.Name.LocalName != "TOKEN" ? element.Name.LocalName : element.Value;
        }
    }
}