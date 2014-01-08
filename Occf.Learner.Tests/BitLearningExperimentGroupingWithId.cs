using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;
using Code2Xml.Core;
using Paraiba.Collections.Generic;
using Paraiba.Linq;

namespace Occf.Learner.Core.Tests {
	public abstract class BitLearningExperimentGroupingWithId {
		internal class NextTarget {
			public int Differential { get; set; }
			public KeyValuePair<BigInteger, string> FeatureAndClassifier { get; set; }
			public BigInteger DiffPattern { get; set; }
		}

		public int WrongCount { get; set; }
		protected abstract Processor Processor { get; }
		private readonly ISet<string> _elementNames;
		private Dictionary<BigInteger, string> _idealAccepted;
		private Dictionary<BigInteger, string> _idealRejected;
		private Dictionary<BigInteger, string> _accepted;
		private Dictionary<BigInteger, string> _rejected;
		private HashSet<BigInteger> _seedAccepted;
		private int _predicateDepth = 10;
		private List<string> _masterPredicates;
		private BigInteger _mask;
		private const int LearningCount = 200;
		private const int AncestorCount = 5;
		private List<string> _classifiers;

		protected BitLearningExperimentGroupingWithId(params string[] elementNames) {
			_elementNames = elementNames.ToHashSet();
		}

		private string CommonSuffix(string s1, string s2) {
			int count = Math.Min(s1.Length, s2.Length);
			var ret = "";
			for (int i = 0; i < count; i++) {
				var ch = s1[i];
				if (ch == s2[i]) {
					ret += ch;
				} else {
					break;
				}
			}
			return ret;
		}

		private void UpdateDict(Dictionary<BigInteger, string> dict, XElement element) {
			var bits = GetBits(element, _masterPredicates);
			var existingAncestorsStr = dict.GetValueOrDefault(bits);
			var ancestors = element.Ancestors().Take(AncestorCount)
					.Select(e => e.NameOrTokenWithId());
			var ancestorsStr = ">" + string.Join(">", ancestors) + ">";
			if (existingAncestorsStr == null) {
				dict.Add(bits, ancestorsStr);
			} else {
				dict[bits] = CommonSuffix(existingAncestorsStr, ancestorsStr);
			}
		}

		public void LearnUntilBeStable(ICollection<string> allPaths, ICollection<string> seedPaths) {
			_idealAccepted = new Dictionary<BigInteger, string>();
			_idealRejected = new Dictionary<BigInteger, string>();
			_accepted = new Dictionary<BigInteger, string>();
			_rejected = new Dictionary<BigInteger, string>();
			_seedAccepted = new HashSet<BigInteger>();
			_classifiers = new List<string> { ">" };

			var seedAcceptedElements = new List<XElement>();
			var seedRejectedElements = new List<XElement>();
			foreach (var path in seedPaths) {
				Console.Write(".");
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile, null, true);
				foreach (var e in GetAllElements(ast)) {
					(IsAccepted(e) ? seedAcceptedElements : seedRejectedElements).Add(e);
				}
			}
			_masterPredicates = seedAcceptedElements.GetUnionKeys(_predicateDepth)
					.ToList();
			_mask = (BigInteger.One << _masterPredicates.Count) - BigInteger.One;

			foreach (var e in seedAcceptedElements) {
				UpdateDict(_accepted, e);
				UpdateDict(_idealAccepted, e);
				var bits = GetBits(e, _masterPredicates);
				_seedAccepted.Add(bits);
			}
			foreach (var e in seedRejectedElements) {
				UpdateDict(_rejected, e);
				UpdateDict(_idealRejected, e);
			}

			foreach (var path in allPaths) {
				Console.Write(".");
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				foreach (var e in GetAllElements(ast)) {
					UpdateDict(IsAccepted(e) ? _idealAccepted : _idealRejected, e);
				}
			}

			if (_idealAccepted.Keys.ToHashSet().Overlaps(_idealRejected.Keys.ToHashSet())) {
				throw new Exception("Master predicates can't classify elements!");
			}

			var count = 2;
			while (true) {
				var time = Environment.TickCount;
				if (LearnAndApply()) {
					if (--count == 0) {
						break;
					}
					//LearnPredicates();
				}
				Console.WriteLine("Time: " + (Environment.TickCount - time));
			}
			Console.WriteLine("Required elements: " + (_accepted.Count + _rejected.Count));
		}

		private string CanReject(IList<BigInteger> predicates, Dictionary<BigInteger, string> rejected) {
			foreach (var featureAndClassifier in rejected) {
				var feature = featureAndClassifier.Key;
				var classifier = featureAndClassifier.Value;
				var iClassifier = DetermineClassifierIndex(classifier);
				var p = predicates[iClassifier];
				if ((feature & p) == p) {
					return classifier;
				}
			}
			return null;
		}

		private int DetermineClassifierIndex(string classifier) {
			for (int i = _classifiers.Count - 1; i >= 0; i--) {
				if (classifier.StartsWith(_classifiers[i])) {
					return i;
				}
			}
			throw new Exception("Can't find the given classifier: " + classifier);
		}

		private List<BigInteger> LearnPredicates() {
			while (true) {
				var predicates = Enumerable.Repeat(_mask, _classifiers.Count).ToList();
				string failedClassifier = null;
				string classifier = null;
				int iClassifier = 0;
				foreach (var featureAndClassifier in _accepted) {
					var feature = featureAndClassifier.Key;
					classifier = featureAndClassifier.Value;
					iClassifier = DetermineClassifierIndex(classifier);
					predicates[iClassifier] &= feature;
					failedClassifier = CanReject(predicates, _rejected);
					if (failedClassifier != null) {
						break;
					}
				}
				if (failedClassifier == null) {
					return predicates;
				}
				var count = _classifiers.Count;
				if (classifier != _classifiers[iClassifier]) {
					var i = classifier.IndexOf('>', _classifiers[iClassifier].Length + 1);
					var newClassifier = classifier.Substring(0, i + 1);
					if (!_classifiers.Contains(newClassifier)) {
						_classifiers.Add(newClassifier);
					}
				}
				var failedIndex = DetermineClassifierIndex(failedClassifier);
				if (failedClassifier != _classifiers[failedIndex]) {
					var i = failedClassifier.IndexOf('>', _classifiers[failedIndex].Length + 1);
					var newClassifier = classifier.Substring(0, i + 1);
					if (!_classifiers.Contains(newClassifier)) {
						_classifiers.Add(newClassifier);
					}
				}
				if (_classifiers.Count == count) {
					throw new Exception("Fail to learn rules");
				}
			}
		}

		private bool LearnAndApply() {
			var predicates = LearnPredicates();
			BigInteger diffPattern;

			var correctlyAccepted = 0;
			var correctlyRejected = 0;
			var wronglyAccepted = 0;
			var wronglyRejected = 0;
			var nextAcceptedList = new List<List<NextTarget>>();
			for (int i = 0; i < _classifiers.Count; i++) {
				nextAcceptedList.Add(new List<NextTarget>());
			}
			var nextRejectedList = new List<List<NextTarget>>();
			for (int i = 0; i < _classifiers.Count; i++) {
				nextRejectedList.Add(new List<NextTarget>());
			}
			var difAccepted = new List<NextTarget>();
			var difRejected = new List<NextTarget>();

			var index = 0;

			foreach (var featureAndClassifier in _idealAccepted) {
				var feature = featureAndClassifier.Key;
				var classifier = featureAndClassifier.Value;
				var iClassifier = DetermineClassifierIndex(classifier);

				var differential = GetClassifierInput(feature, predicates[iClassifier], out diffPattern);
				var actual = differential == 0;
				index++;
				if (actual) {
					correctlyAccepted++;
				} else {
					if (wronglyRejected == 0) {
						//Console.WriteLine("WR (" + prob + "): " + e.SafeParent().SafeParent().SafeParent());
					}
					wronglyRejected++;
				}
				if (!actual && !_accepted.ContainsKey(feature)) {
					UpdateNextElements(nextAcceptedList[iClassifier], featureAndClassifier, differential,
							diffPattern);
				}
			}

			foreach (var featureAndClassifier in _idealRejected) {
				var feature = featureAndClassifier.Key;
				var classifier = featureAndClassifier.Value;
				var iClassifier = DetermineClassifierIndex(classifier);

				var differential = GetClassifierInput(feature, predicates[iClassifier], out diffPattern);
				var actual = differential == 0;
				index++;
				if (actual) {
					if (wronglyAccepted == 0) {
						//Console.WriteLine("WA (" + prob + "): " + e.SafeParent().SafeParent().SafeParent());
					}
					wronglyAccepted++;
				} else {
					correctlyRejected++;
				}
				if (!actual && !_rejected.ContainsKey(feature)) {
					UpdateNextElements(nextRejectedList[iClassifier], featureAndClassifier, differential,
							diffPattern);
				}
			}
			var nextAcceptedCount = nextAcceptedList.Select(l => l.Count).Sum();
			var nextRejectedCount = nextRejectedList.Select(l => l.Count).Sum();

			Console.WriteLine("done");
			Console.WriteLine("DA: " + difAccepted.Count + ", DR: " + difRejected.Count);
			Console.WriteLine("WA: " + wronglyAccepted + ", WR: " + wronglyRejected + ", CA: "
			                  + correctlyAccepted + ", CR: " + correctlyRejected + ", L: "
			                  + (_accepted.Count + _rejected.Count) + ", P: "
			                  + string.Join(", ", predicates.Select(CountBits)));
			Console.WriteLine("Accepted: " + _accepted.Count + " + " + nextAcceptedCount
			                  + " / " + _idealAccepted.Count);
			Console.WriteLine("Rejected: " + _rejected.Count + " + " + nextRejectedCount
			                  + " / " + _idealRejected.Count);
			WrongCount = wronglyAccepted + wronglyRejected;
			foreach (var nextTargets in nextAcceptedList) {
				foreach (var nextTarget in nextTargets) {
					_accepted.Add(nextTarget.FeatureAndClassifier.Key, nextTarget.FeatureAndClassifier.Value);
				}
			}
			foreach (var nextTargets in nextRejectedList) {
				foreach (var nextTarget in nextTargets) {
					_rejected.Add(nextTarget.FeatureAndClassifier.Key, nextTarget.FeatureAndClassifier.Value);
				}
			}
			return nextAcceptedCount == 0;
		}

		private void UpdateNextElements(
				List<NextTarget> nextTargets, KeyValuePair<BigInteger, string> featureAndClassifier,
				int differential, BigInteger diffPattern) {
			nextTargets.Add(new NextTarget {
				Differential = differential,
				DiffPattern = diffPattern,
				FeatureAndClassifier = featureAndClassifier,
			});
			nextTargets.Sort((t1, t2) => t1.Differential.CompareTo(t2.Differential));
			if (nextTargets.Count > LearningCount) {
				nextTargets.RemoveAt(LearningCount);
			}
		}

		private int GetClassifierInput(BigInteger feature, BigInteger predicate, out BigInteger diffs) {
			diffs = (feature & predicate) ^ predicate;
			var count = CountBits(diffs);
			return count;
		}

		private static int CountBits(BigInteger bits) {
			var count = 0;
			while (bits != BigInteger.Zero) {
				count += (int)(bits & BigInteger.One);
				bits >>= 1;
			}
			return count;
		}

		private BigInteger GetBits(XElement e, IEnumerable<string> predicateKeys) {
			var keys = e.GetSurroundingKeys(_predicateDepth);
			var ret = BigInteger.Zero;
			foreach (var predicateKey in predicateKeys) {
				if (keys.Contains(predicateKey)) {
					ret++;
				}
				ret <<= 1;
			}
			return ret;
		}

		protected IEnumerable<XElement> GetAllElements(XElement ast) {
			return ast.DescendantsAndSelf().Where(e => _elementNames.Contains(e.Name()));
		}

		protected abstract bool IsAccepted(XElement e);
	}
}