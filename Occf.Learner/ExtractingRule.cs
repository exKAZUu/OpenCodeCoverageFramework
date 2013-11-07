using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core.Location;

namespace Occf.Learner.Core {
	public class ExtractingRule {
		public Dictionary<string, List<IFilter>> FilterDictionary { get; private set; }

		public IEnumerable<IFilter> Filters {
			get { return FilterDictionary.Values.SelectMany(f => f); }
		}

		public ExtractingRule(IEnumerable<IFilter> filters) {
			FilterDictionary = new Dictionary<string, List<IFilter>>();
			foreach (var filter in filters) {
				var name = filter.ElementName;
				List<IFilter> rules;
				if (!FilterDictionary.TryGetValue(name, out rules)) {
					rules = new List<IFilter>();
					FilterDictionary.Add(name, rules);
				}
				rules.Add(filter);
			}
		}

		public IEnumerable<XElement> ExtractElements(XElement ast) {
			foreach (var nameAndRules in FilterDictionary) {
				var elements = ast.DescendantsAndSelf(nameAndRules.Key);
				var rules = nameAndRules.Value;
				var results = elements.Where(e => rules.All(rule => rule.IsAcceptable(e)));
				foreach (var result in results) {
					yield return result;
				}
			}
		}

		public Dictionary<CodeRange, XElement> ExtractRange2Elements(XElement ast) {
			var elements = ExtractElements(ast);
			// Elements having the same range can appear so can't use ToDictiornary
			var ret = new Dictionary<CodeRange, XElement>();
			foreach (var element in elements) {
				var range = CodeRange.Locate(element);
				ret[range] = element;
			}
			return ret;
		}
	}
}