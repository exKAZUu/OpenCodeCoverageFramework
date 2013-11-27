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

		private int _location;

		public int Location {
			get {
				if (_location < 0) {
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
				Elements = Target.SiblingsAndSelf();
			} else {
				Elements = root.DescendantsElements(depth).Last();
			}
			_location = -1;
		}
	}

	public abstract class Predicate {
		public int Depth { get; private set; }

		protected Predicate(int depth) {
			Depth = depth;
		}

		public abstract bool Check(DepthInfo info);
	}

	public class LocatingPredicate : Predicate {
		public int Location { get; private set; }

		private LocatingPredicate(int depth, int location) : base(depth) {
			Contract.Requires(location <= 0);
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
				return (Depth * 397) ^ Location;
			}
		}

		public static LocatingPredicate Create(int depth, XElement element) {
			return new LocatingPredicate(depth, Locate(element));
		}

		public static int Locate(XElement target) {
			var name = target.Name();
			return target.ElementsBeforeSelf().Count(e => e.Name() == name);
		}
	}

	public class ContainingTokenTextPredicate : Predicate {
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
			return elements.Where(e => e.IsTokenSet())
					.Select(e => e.TokenText())
					.ToHashSet();
		}
	}

	public class ContainingElementNamePredicate : Predicate {
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

	public class MatchingElementSequencePredicate : Predicate {
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
			return String.Join("/", elements.Select(e => e.Name()));
		}
	}

	public class MatchingElementAndTokenSequencePredicate : Predicate {
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
		public static List<XElement> SiblingsAndSelf(this XElement element) {
			var elemens = element.ElementsBeforeSelf().ToList();
			elemens.Add(element);
			elemens.AddRange(element.ElementsAfterSelf());
			return elemens;
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

		public static IEnumerable<Predicate> GeneratePredicates(XElement root, int depthCount) {
			var depth = 0;
			foreach (var element in root.AncestorsAndSelf(depthCount)) {
				var elements = element.SiblingsAndSelf();
				yield return LocatingPredicate.Create(depth, element);
				foreach (var predicate in ContainingTokenTextPredicate.Create(depth, elements)) {
					yield return predicate;
				}
				foreach (var predicate in ContainingElementNamePredicate.Create(depth, elements)) {
					yield return predicate;
				}
				yield return MatchingElementSequencePredicate.Create(depth, elements);
				yield return MatchingElementAndTokenSequencePredicate.Create(depth, elements);
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
				yield return MatchingElementSequencePredicate.Create(depth, elements);
				yield return MatchingElementAndTokenSequencePredicate.Create(depth, elements);
			}
		}
	}
}