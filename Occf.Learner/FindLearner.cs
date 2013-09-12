using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Paraiba.Linq;

namespace Occf.Learner {
	public static class FindLearner {
		public static Dictionary<string, List<IFilteringRule>> Learn(
				XElement ast, IEnumerable<XElement> elements) {
			var name2elements = new Dictionary<string, HashSet<XElement>>();
			var name2rules = new Dictionary<string, List<IFilteringRule>>();
			foreach (var element in elements) {
				HashSet<XElement> set;
				if (!name2elements.TryGetValue(element.Name.LocalName, out set)) {
					set = new HashSet<XElement>();
					name2elements.Add(element.Name.LocalName, set);
				}
				set.Add(element);
			}

			foreach (var nameAndElements in name2elements) {
				var name = nameAndElements.Key;
				var all = ast.DescendantsAndSelf(name).ToHashSet();
				var accepted = nameAndElements.Value;

				var rules = Learn(all, accepted).ToList();
				name2rules.Add(name, rules);

				var filtered = rules.Aggregate((IEnumerable<XElement>)all,
						(current, rule) => rule.Filter(current));
				Console.WriteLine("ElementName: " + name + " (" + all.Count + ", " + accepted.Count + ", "
				                  + filtered.Count() + ")");

				foreach (var rule in rules) {
					var count = rule.CountRemovableTargets(all);
					Console.WriteLine(count + ": " + rule);
				}
			}
			return name2rules;
		}

		public static IEnumerable<IFilteringRule> Learn(HashSet<XElement> all, HashSet<XElement> accepted) {
			var denied = all.ToHashSet();
			denied.ExceptWith(accepted);

			/*
			 * C: FirstElement
			 * 
			return root.Descendants("statement")
					.Where(
							e => e.FirstElement().Name.LocalName != "labeled_statement"
									&& e.FirstElement().Name.LocalName != "compound_statement");
			 */

			/*
			 * Java: FirstElement
			 * 
			return root.Descendants("statement")
					.Where(e => {
						// ブロック自身は意味を持たないステートメントで、中身だけが必要なので除外
						if (e.FirstElement().Name() == "block") {
							return false;
						}
						// ラベルはループ文に付くため，ラベルの中身は除外
						var second = e.Parent.NthElementOrDefault(1);
						if (second != null && second.Value == ":"
						    && e.Parent.Name() == "statement") {
							return false;
						}
						if (e.FirstElement().Value == ";") {
							return false;
						}
						return true;
					});
			 */

			/*
			 * C#: FirstElement
			 * 
            var decls = root.Descendants("declaration_statement");
            var stmts = root.Descendants("embedded_statement")
                    .Where(e => e.FirstElement().Name() != "block");
            return stmts.Concat(decls);
			 */

			/*
			 * Ruby: Elements()
			 * 
            return root.DescendantsAndSelf("block")
                    .SelectMany(e => e.Elements())
                    .Where(e => e.Name() != "block");

			 */

			/*
			 * Lua: None
			 * 
            return root.Descendants("stat")
                    .Concat(root.Descendants("laststat"));
			 */

			/*
			 * Python: None
			 * 
            return root
                    .Descendants()
                    .Where(e => e.Name.LocalName == "small_stmt"
                            || e.Name.LocalName == "compound_stmt");
			 */

			yield return LearnMustBeRule(accepted, CountChildren);
			yield return LearnMustNotBeRule(accepted, CountChildren, denied);

			foreach (var rule in LearnMustNotBeRule(accepted, GetChildrenSet)) {
				yield return rule;
			}
			yield return LearnMustNotHaveRule(accepted, GetChildrenSet, denied);
		}

		private static int CountChildren(XElement e) {
			return e.Elements().Count();
		}

		private static string GetChildrenSequence(XElement e) {
			return string.Join("/", e.Elements().Select(e2 => e2.NameOrValue()));
		}

		private static string GetSelfSequence(XElement e) {
			return string.Join("/", e.Parent.Elements().Select(e2 => e2.NameOrValue()));
		}

		private static IEnumerable<string> GetChildrenSet(XElement e) {
			return e.Elements().Select(e2 => e2.NameOrValue());
		}

		private static IEnumerable<string> GetSelfSet(XElement e) {
			return e.Parent.Elements().Select(e2 => e2.NameOrValue());
		}

		private static MustBeRule<T> LearnMustBeRule<T>(
				IEnumerable<XElement> accepted, Func<XElement, T> selector) {
			var acceptedSet = accepted.Select(selector).ToHashSet();
			return new MustBeRule<T>(acceptedSet, selector);
		}

		private static MustNotBeRule<T> LearnMustNotBeRule<T>(
				IEnumerable<XElement> accepted, Func<XElement, T> selector, IEnumerable<XElement> denied) {
			var acceptedSet = accepted.Select(selector).ToHashSet();
			var deniedSet = denied.Select(selector).ToHashSet();
			deniedSet.ExceptWith(acceptedSet);
			return new MustNotBeRule<T>(deniedSet, selector);
		}

		private static IEnumerable<MustHaveRule<T>> LearnMustNotBeRule<T>(
				IEnumerable<XElement> accepted, Func<XElement, IEnumerable<T>> selector) {
			var requiredSet = accepted.Select(selector)
					.Aggregate((acc, values) => acc.Intersect(values))
					.ToHashSet();
			if (requiredSet.Count > 0) {
				yield return new MustHaveRule<T>(requiredSet, selector);
			}
		}

		private static MustNotHaveRule<T> LearnMustNotHaveRule<T>(
				IEnumerable<XElement> accepted, Func<XElement, IEnumerable<T>> selector,
				IEnumerable<XElement> denied) {
			var acceptedSet = accepted.SelectMany(selector).ToHashSet();
			var deniedSet = denied.SelectMany(selector).ToHashSet();
			deniedSet.ExceptWith(acceptedSet);
			return new MustNotHaveRule<T>(deniedSet, selector);
		}

		public static string NameOrValue(this XElement element) {
			return element.Name.LocalName != "TOKEN" ? element.Name.LocalName : element.Value;
		}
	}
}