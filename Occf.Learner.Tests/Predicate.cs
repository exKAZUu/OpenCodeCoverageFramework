using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core;
using Paraiba.Linq;

namespace Occf.Learner.Core.Tests {
	public class DepthInfo {
		public XElement Target { get; private set; }
		public IList<XElement> Elements { get; private set; }

		private string _location;

		public string Location {
			get {
				if (_location == null) {
					_location = LocatingPredicate.Locate(Target);
				}
				return _location;
			}
		}

		private ISet<string> _tokenTexts;

		public ISet<string> TokenTexts {
			get {
				return _tokenTexts ?? (_tokenTexts = ContainingTokenTextPredicate.GetTokenTexts(Elements));
			}
		}

		private ISet<string> _elementNames;

		public ISet<string> ElementNames {
			get {
				return _elementNames ?? (_elementNames = ContainingTokenTextPredicate.GetTokenTexts(Elements));
			}
		}

		private string _elementSequence;

		public string ElementSequence {
			get {
				return _elementSequence
				       ?? (_elementSequence = MatchingElementSequencePredicate.GetSequence(Elements));
			}
		}

		private string _beforeElementSequence;

		public string BeforeElementSequence {
			get {
				return _beforeElementSequence
				       ?? (_beforeElementSequence = MatchingBeforeElementSequencePredicate.GetSequence(Target));
			}
		}

		private string _afterElementSequence;

		public string AfterElementSequence {
			get {
				return _afterElementSequence
				       ?? (_afterElementSequence = MatchingAfterElementSequencePredicate.GetSequence(Target));
			}
		}

		private string _elementAndTokenSequence;

		public string ElementAndTokenSequence {
			get {
				return _elementAndTokenSequence
				       ?? (_elementAndTokenSequence =
						       MatchingElementAndTokenSequencePredicate.GetSequence(Elements));
			}
		}

		public DepthInfo(XElement root, int depth) {
			if (depth <= 0) {
				Target = root.AncestorsAndSelf(-depth).Last();
				Elements = Target.SiblingsAndSelf().ToList();
			} else {
				Elements = root.DescendantsElements(depth).Last();
			}
		}
	}

	public abstract class Predicate {
		public abstract bool Check(DepthInfo info);
	}

	public abstract class DepthBasedPredicate : Predicate {
		public int Depth { get; private set; }

		protected DepthBasedPredicate(int depth) {
			Depth = depth;
		}
	}

	public class LocatingPredicate : DepthBasedPredicate {
		public string Location { get; private set; }

		private LocatingPredicate(int depth, string location) : base(depth) {
			Location = location;
		}

		public override bool Check(DepthInfo info) {
			return info.Location == Location;
		}

		protected bool Equals(LocatingPredicate other) {
			return Depth == other.Depth && Location == other.Location;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}
			return Equals((LocatingPredicate)obj);
		}

		public override int GetHashCode() {
			unchecked {
				return (Depth * 397) ^ Location.GetHashCode();
			}
		}

		public static LocatingPredicate Create(int depth, XElement element) {
			return new LocatingPredicate(depth, Locate(element));
		}

		public static string Locate(XElement target) {
			return target.Attribute("id").Value;
		}
	}

	public class SurroundingElementsPredicate {
		public string Key { get; set; }
		public string Value { get; private set; }

		public SurroundingElementsPredicate(string key, string value) {
			Contract.Requires(value != null);
			Key = key;
			Value = value;
		}

		public bool Check(Dictionary<string, CountingSet<string>> dict) {
			CountingSet<string> values;
			if (!dict.TryGetValue(Key, out values)) {
				return false;
			}
			return values.Contains(Value);
		}

		public static IEnumerable<SurroundingElementsPredicate> Create(
				XElement target, int length) {
			return GetSurroundingElements(target, length)
					.SelectMany(kv => kv.Value.Select(value =>
							new SurroundingElementsPredicate(kv.Key, value)));
		}

		public static Dictionary<string, CountingSet<string>> GetSurroundingElements(
				XElement target, int length) {
			return GetSurroundingElements(Enumerable.Repeat(target, 1), length);
		}

		public static Dictionary<string, CountingSet<string>> GetSurroundingElements(
				IEnumerable<XElement> targets, int length) {
			var ret = new Dictionary<string, CountingSet<string>>();
			foreach (var target in targets) {
				var dict = target.SurroundingElementsWithSelf(length)
						.Select(kv => Tuple.Create(kv.Key,
								kv.Value.Select(e => e.NameOrTokenWithId()).ToCountingSet()))
						.ToDictionary(kv => kv.Item1, kv => kv.Item2);
				foreach (var kv in dict) {
					CountingSet<string> set;
					if (ret.TryGetValue(kv.Key, out set)) {
						set.UnionWith(kv.Value);
					} else {
						ret.Add(kv.Key, kv.Value);
					}
				}
			}
			return ret;
		}

		protected bool Equals(SurroundingElementsPredicate other) {
			return string.Equals(Key, other.Key) && string.Equals(Value, other.Value);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}
			return Equals((SurroundingElementsPredicate)obj);
		}

		public override int GetHashCode() {
			unchecked {
				return ((Key != null ? Key.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
			}
		}

		public override string ToString() {
			return string.Format("Key: {0}, Value: {1}", Key, Value);
		}
	}

	public class ContainingTokenTextPredicate : DepthBasedPredicate {
		public string Text { get; private set; }

		private ContainingTokenTextPredicate(int depth, string text) : base(depth) {
			Contract.Requires(text != null);
			Text = text;
		}

		public override bool Check(DepthInfo info) {
			return info.TokenTexts.Contains(Text);
		}

		protected bool Equals(ContainingTokenTextPredicate other) {
			return Depth == other.Depth && String.Equals(Text, other.Text);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}
			return Equals((ContainingTokenTextPredicate)obj);
		}

		public override int GetHashCode() {
			unchecked {
				return (Depth * 397) ^ (Text != null ? Text.GetHashCode() : 0);
			}
		}

		public static IEnumerable<ContainingTokenTextPredicate> Create(
				int depth, IList<XElement> elements) {
			return GetTokenTexts(elements)
					.Select(text => new ContainingTokenTextPredicate(depth, text));
		}

		public static HashSet<string> GetTokenTexts(IList<XElement> elements) {
			return elements.Descendants()
					.Where(e => e.IsTokenSet())
					.Take(30)
					.Select(e => e.TokenText())
					.ToHashSet();
		}
	}

	public class ContainingElementNamePredicate : DepthBasedPredicate {
		public string Name { get; private set; }

		private ContainingElementNamePredicate(int depth, string name) : base(depth) {
			Contract.Requires(name != null);
			Name = name;
		}

		public override bool Check(DepthInfo info) {
			return info.ElementNames.Contains(Name);
		}

		protected bool Equals(ContainingElementNamePredicate other) {
			return Depth == other.Depth && String.Equals(Name, other.Name);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}
			return Equals((ContainingElementNamePredicate)obj);
		}

		public override int GetHashCode() {
			unchecked {
				return (Depth * 397) ^ (Name != null ? Name.GetHashCode() : 0);
			}
		}

		public static IEnumerable<ContainingElementNamePredicate> Create(
				int depth, IList<XElement> elements) {
			return GetElementNames(elements)
					.Select(name => new ContainingElementNamePredicate(depth, name));
		}

		public static HashSet<string> GetElementNames(IList<XElement> elements) {
			return elements.Select(e => e.Name())
					.ToHashSet();
		}
	}

	public class MatchingElementSequencePredicate : DepthBasedPredicate {
		public string Sequence { get; private set; }

		private MatchingElementSequencePredicate(int depth, string sequence) : base(depth) {
			Contract.Requires(sequence != null);
			Sequence = sequence;
		}

		public override bool Check(DepthInfo info) {
			return info.ElementSequence == Sequence;
		}

		protected bool Equals(MatchingElementSequencePredicate other) {
			return Depth == other.Depth && String.Equals(Sequence, other.Sequence);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}
			return Equals((MatchingElementSequencePredicate)obj);
		}

		public override int GetHashCode() {
			unchecked {
				return (Depth * 397) ^ (Sequence != null ? Sequence.GetHashCode() : 0);
			}
		}

		public static MatchingElementSequencePredicate Create(int depth, IList<XElement> elements) {
			return new MatchingElementSequencePredicate(depth, GetSequence(elements));
		}

		public static string GetSequence(IList<XElement> elements) {
			return String.Join("/",
					elements.Select(
							e => e.Name() == Code2XmlConstants.TokenSetElementName ? e.TokenText() : e.Name()));
		}
	}

	public class MatchingBeforeElementSequencePredicate : DepthBasedPredicate {
		public string Sequence { get; private set; }

		private MatchingBeforeElementSequencePredicate(int depth, string sequence) : base(depth) {
			Contract.Requires(sequence != null);
			Sequence = sequence;
		}

		public override bool Check(DepthInfo info) {
			return info.BeforeElementSequence == Sequence;
		}

		protected bool Equals(MatchingBeforeElementSequencePredicate other) {
			return Depth == other.Depth && String.Equals(Sequence, other.Sequence);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}
			return Equals((MatchingBeforeElementSequencePredicate)obj);
		}

		public override int GetHashCode() {
			unchecked {
				return (Depth * 397) ^ (Sequence != null ? Sequence.GetHashCode() : 0);
			}
		}

		public static MatchingBeforeElementSequencePredicate Create(int depth, XElement element) {
			return new MatchingBeforeElementSequencePredicate(depth, GetSequence(element));
		}

		public static string GetSequence(XElement element) {
			return String.Join("/",
					element.ElementsBeforeSelf().Select(
							e => e.Name() == Code2XmlConstants.TokenSetElementName ? e.TokenText() : e.Name()));
		}
	}

	public class MatchingAfterElementSequencePredicate : DepthBasedPredicate {
		public string Sequence { get; private set; }

		private MatchingAfterElementSequencePredicate(int depth, string sequence) : base(depth) {
			Contract.Requires(sequence != null);
			Sequence = sequence;
		}

		public override bool Check(DepthInfo info) {
			return info.AfterElementSequence == Sequence;
		}

		protected bool Equals(MatchingAfterElementSequencePredicate other) {
			return Depth == other.Depth && String.Equals(Sequence, other.Sequence);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}
			return Equals((MatchingAfterElementSequencePredicate)obj);
		}

		public override int GetHashCode() {
			unchecked {
				return (Depth * 397) ^ (Sequence != null ? Sequence.GetHashCode() : 0);
			}
		}

		public static MatchingAfterElementSequencePredicate Create(int depth, XElement element) {
			return new MatchingAfterElementSequencePredicate(depth, GetSequence(element));
		}

		public static string GetSequence(XElement element) {
			return String.Join("/",
					element.ElementsAfterSelf().Select(
							e => e.Name() == Code2XmlConstants.TokenSetElementName ? e.TokenText() : e.Name()));
		}
	}

	public class MatchingElementAndTokenSequencePredicate : DepthBasedPredicate {
		public string Sequence { get; private set; }

		private MatchingElementAndTokenSequencePredicate(int depth, string sequence) : base(depth) {
			Contract.Requires(sequence != null);
			Sequence = sequence;
		}

		public override bool Check(DepthInfo info) {
			return info.ElementAndTokenSequence == Sequence;
		}

		protected bool Equals(MatchingElementAndTokenSequencePredicate other) {
			return Depth == other.Depth && string.Equals(Sequence, other.Sequence);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}
			return Equals((MatchingElementAndTokenSequencePredicate)obj);
		}

		public override int GetHashCode() {
			unchecked {
				return (Depth * 397) ^ (Sequence != null ? Sequence.GetHashCode() : 0);
			}
		}

		public static MatchingElementAndTokenSequencePredicate Create(int depth, IList<XElement> elements) {
			return new MatchingElementAndTokenSequencePredicate(depth, GetSequence(elements));
		}

		public static string GetSequence(IList<XElement> elements) {
			return String.Join("/", elements.Select(e => e.IsTokenSet() ? e.TokenText() : e.Name()));
		}
	}

	public static class PredicateGenerator {
		public static string NameWithId(this XElement element) {
			return element.Name() + element.Attribute("id").Value;
		}

		public static string NameOrTokenWithId(this XElement element) {
			return element.IsTokenSet()
					? element.Name() + element.Attribute("id").Value + element.TokenText()
					: element.Name() + element.Attribute("id").Value;
		}

		public static Dictionary<string, List<XElement>> SurroundingElementsWithSelf(
				this XElement element, int length) {
			var ret = new Dictionary<string, List<XElement>>();
			var childKeys = new List<string> { "" };
			ret[""] = new List<XElement> { };
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
			return ret;
		}

		public static IEnumerable<XElement> Siblings(this XElement element, int length) {
			foreach (var e in element.ElementsBeforeSelf().Reverse().Take(length).Reverse()) {
				yield return e;
			}
			foreach (var e in element.ElementsAfterSelf().Take(length)) {
				yield return e;
			}
		}

		public static IEnumerable<XElement> Siblings(this XElement element) {
			foreach (var e in element.ElementsBeforeSelf()) {
				yield return e;
			}
			foreach (var e in element.ElementsAfterSelf()) {
				yield return e;
			}
		}

		public static IEnumerable<XElement> SiblingsAndSelf(this XElement element) {
			var p = element.Parent;
			if (p == null) {
				return Enumerable.Repeat(element, 1);
			}
			return p.Elements();
		}

		public static IEnumerable<XElement> AncestorsAndSelf(this XElement element, int depthCount) {
			for (int count = depthCount; count >= 0 && element != null; count--) {
				yield return element;
				element = element.Parent;
			}
		}

		public static IEnumerable<List<XElement>> DescendantsElements(
				this XElement element, int depthCount) {
			var elements = new List<XElement> { element };
			for (int count = depthCount - 1; count >= 0; count--) {
				elements = elements.Where(e => !e.IsTokenSet()).Elements().ToList();
				yield return elements;
			}
		}

		public static IEnumerable<DepthBasedPredicate> InferDepthBasedPredicate(
				XElement root, int depthCount) {
			yield break;
			var depth = 0;
			//foreach (var element in root.AncestorsAndSelf(4)) {
			//	var elements = element.SiblingsAndSelf();
			//	foreach (var predicate in ContainingTokenTextPredicate.Create(depth, elements)) {
			//		yield return predicate;
			//	}
			//	depth--;
			//}
			//depth = 0;
			foreach (var element in root.AncestorsAndSelf(depthCount)) {
				var elements = element.SiblingsAndSelf().ToList();
				yield return LocatingPredicate.Create(depth, element);
				//yield return MatchingBeforeElementSequencePredicate.Create(depth, element);
				//yield return MatchingAfterElementSequencePredicate.Create(depth, element);
				foreach (var predicate in ContainingTokenTextPredicate.Create(depth, elements)) {
					yield return predicate;
				}
				foreach (var predicate in ContainingElementNamePredicate.Create(depth, elements)) {
					yield return predicate;
				}
				//yield return MatchingElementSequencePredicate.Create(depth, elements);
				//yield return MatchingElementAndTokenSequencePredicate.Create(depth, elements);
				depth--;
			}

			depth = 0;
			foreach (var elements in root.DescendantsElements(depthCount)) {
				depth++;
				foreach (var predicate in ContainingTokenTextPredicate.Create(depth, elements)) {
					yield return predicate;
				}
				foreach (var predicate in ContainingElementNamePredicate.Create(depth, elements)) {
					yield return predicate;
				}
				//yield return MatchingElementSequencePredicate.Create(depth, elements);
				//yield return MatchingElementAndTokenSequencePredicate.Create(depth, elements);
			}
		}
	}
}