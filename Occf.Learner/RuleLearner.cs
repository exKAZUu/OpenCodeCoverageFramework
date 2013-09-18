using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Paraiba.Linq;

namespace Occf.Learner.Core {
	public static class RuleLearner {
		public static IEnumerable<IFilter> Learn(XElement ast, IEnumerable<XElement> elements) {
			var name2Elements = new Dictionary<string, HashSet<XElement>>();
			foreach (var element in elements) {
				HashSet<XElement> set;
				if (!name2Elements.TryGetValue(element.Name.LocalName, out set)) {
					set = new HashSet<XElement>();
					name2Elements.Add(element.Name.LocalName, set);
				}
				set.Add(element);
			}

			foreach (var nameAndElements in name2Elements) {
				var name = nameAndElements.Key;
				var all = ast.DescendantsAndSelf(name).ToHashSet();
				var accepted = nameAndElements.Value;

				var filters = Learn(name, all, accepted).ToList();
				foreach (var filter in filters) {
					yield return filter;
				}

				var filtered = filters.Aggregate((IEnumerable<XElement>)all,
						(current, rule) => rule.Select(current));
				Console.WriteLine("ElementName: " + name + " (" + all.Count + ", " + accepted.Count + ", "
				                  + filtered.Count() + ")");

				foreach (var rule in filters) {
					var count = rule.CountRemovableTargets(all);
					Console.WriteLine(count + ": " + rule);
				}
			}
		}

		private static IEnumerable<IFilter> Learn(
				string elementName, HashSet<XElement> all, HashSet<XElement> accepted) {
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

			yield return new NopFilter(elementName);
			yield return LearnMustBeRule(elementName, new ChildrenCountExtractor(), accepted);
			yield return LearnMustNotBeRule(elementName, new ChildrenCountExtractor(), accepted, denied);
			yield return LearnMustBeRule(elementName, new ChildrenSequenceExtractor(), accepted);
			yield return LearnMustNotBeRule(elementName, new ChildrenSequenceExtractor(), accepted, denied);
			yield return LearnMustBeRule(elementName, new SelfSequenceExtractor(), accepted);
			yield return LearnMustNotBeRule(elementName, new SelfSequenceExtractor(), accepted, denied);

			foreach (var rule in LearnMustNotBeRule(elementName, new ChildrenSetExtractor(), accepted)) {
				yield return rule;
			}
			yield return LearnMustNotHaveRule(elementName, new ChildrenSetExtractor(), accepted, denied);
			foreach (var rule in LearnMustNotBeRule(elementName, new SelfSetExtractor(), accepted)) {
				yield return rule;
			}
			yield return LearnMustNotHaveRule(elementName, new SelfSetExtractor(), accepted, denied);
		}

		private static MustBeFilter<T> LearnMustBeRule<T>(
				string elementName, IPropertyExtractor<T> extractor, IEnumerable<XElement> accepted) {
			var acceptedProperties = accepted.Select(extractor.ExtractProperty).ToHashSet();
			return new MustBeFilter<T>(elementName, acceptedProperties, extractor);
		}

		private static MustNotBeFilter<T> LearnMustNotBeRule<T>(
				string elementName, IPropertyExtractor<T> extractor, IEnumerable<XElement> accepted,
				IEnumerable<XElement> denied) {
			var acceptedProperties = accepted.Select(extractor.ExtractProperty).ToHashSet();
			var deniedProperties = denied.Select(extractor.ExtractProperty).ToHashSet();
			deniedProperties.ExceptWith(acceptedProperties);
			return new MustNotBeFilter<T>(elementName, deniedProperties, extractor);
		}

		private static IEnumerable<MustHaveFilter<T>> LearnMustNotBeRule<T>(
				string elementName, IPropertyExtractor<IEnumerable<T>> extractor, IEnumerable<XElement> accepted) {
			var acceptedProperties = accepted.Select(extractor.ExtractProperty)
					.Aggregate((acc, values) => acc.Intersect(values))
					.ToHashSet();
			if (acceptedProperties.Count > 0) {
				yield return new MustHaveFilter<T>(elementName, acceptedProperties, extractor);
			}
		}

		private static MustNotHaveFilter<T> LearnMustNotHaveRule<T>(
				string elementName, IPropertyExtractor<IEnumerable<T>> extractor, IEnumerable<XElement> accepted,
				IEnumerable<XElement> denied) {
			var acceptedProperties = accepted.SelectMany(extractor.ExtractProperty).ToHashSet();
			var deniedProperties = denied.SelectMany(extractor.ExtractProperty).ToHashSet();
			deniedProperties.ExceptWith(acceptedProperties);
			return new MustNotHaveFilter<T>(elementName, deniedProperties, extractor);
		}

		public static string NameOrValue(this XElement element) {
			return element.Name.LocalName != "TOKEN" ? element.Name.LocalName : element.Value;
		}
	}
}