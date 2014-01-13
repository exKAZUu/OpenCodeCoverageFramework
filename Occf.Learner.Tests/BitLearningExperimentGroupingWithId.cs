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
using System.IO;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;
using Code2Xml.Core;
using Paraiba.Collections.Generic;
using Paraiba.Linq;

namespace Occf.Learner.Core.Tests {
	public abstract class BitLearningExperimentGroupingWithId {
		public class SuspiciousTarget {
			public int BitsCount { get; set; }
			public BigInteger Feature { get; set; }
			public string Classifier { get; set; }
		}

		public int WrongCount { get; set; }
		protected abstract Processor Processor { get; }
		protected abstract bool IsInner { get; }

		public Dictionary<BigInteger, XElement> Feature2Element {
			get { return _feature2Element; }
		}

		public IList<XElement> WronglyAcceptedElements {
			get {
				return _wronglyAcceptedFeatures.Select(kv => kv.Key).Select(f => _feature2Element[f]).ToList();
			}
		}

		public IList<XElement> WronglyRejectedElements {
			get {
				return _wronglyRejectedFeatures.Select(kv => kv.Key).Select(f => _feature2Element[f]).ToList();
			}
		}

		public IList<XElement> IdealAcceptedElements {
			get { return _idealAccepted.Keys.Select(f => _feature2Element[f]).ToList(); }
		}

		public IList<XElement> IdealRejectedElements {
			get { return _idealRejected.Keys.Select(f => _feature2Element[f]).ToList(); }
		}

		private ISet<string> _elementNames;
		private Dictionary<BigInteger, string> _idealAccepted;
		private Dictionary<BigInteger, string> _idealRejected;
		private Dictionary<BigInteger, string> _accepted;
		private Dictionary<BigInteger, string> _rejected;
		private Dictionary<BigInteger, XElement> _feature2Element;

		private readonly List<KeyValuePair<BigInteger, string>> _wronglyAcceptedFeatures =
				new List<KeyValuePair<BigInteger, string>>();

		private readonly List<KeyValuePair<BigInteger, string>> _wronglyRejectedFeatures =
				new List<KeyValuePair<BigInteger, string>>();

		private HashSet<BigInteger> _seedAccepted;
		private int _predicateDepth = 7;
		private IDictionary<string, BigInteger> _masterPredicates;
		private const int LearningCount = 5;
		private const int AncestorCount = 5;
		private List<string> _classifiers;
		private int _acceptingPredicatesCount;
		private BigInteger _acceptingMask;
		private BigInteger _rejectingMask;
		private BigInteger _mask;
		public List<SuspiciousTarget> SuspiciousAcceptedInAccepting { get; private set; }
		public List<SuspiciousTarget> SuspiciousRejectedInAccepting { get; private set; }
		public List<SuspiciousTarget> SuspiciousRejectedInRejecting  { get; private set; }
		private List<BigInteger> _acceptingPredicates;
		private List<BigInteger> _rejectingPredicates;

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
			var index = ret.LastIndexOf(">");
			return ret.Substring(0, index + 1);
		}

		private void UpdateDict(Dictionary<BigInteger, string> dict, BigInteger bits, XElement element) {
			var existingClassifiersStr = dict.GetValueOrDefault(bits);
			IEnumerable<string> classifiers;
			if (IsInner) {
				var descendants = element.FirstDescendants().TakeWhile(e => !e.Parent.IsTerminal())
						.Take(AncestorCount)
						.ToList();
				classifiers = descendants.Select(e => e.NameWithId());
				if (descendants[descendants.Count - 1].IsTerminal()) {
					classifiers = classifiers.Concat(descendants[descendants.Count - 1].TokenText());
				}
			} else {
				classifiers = element.AncestorsAndSelf()
						.Take(AncestorCount)
						.Select(e => e.NameWithId());
			}
			var classifiersStre = ">" + element.Name() + ">" + string.Join(">", classifiers) + ">";
			if (existingClassifiersStr == null) {
				dict.Add(bits, classifiersStre);
			} else {
				dict[bits] = CommonSuffix(existingClassifiersStr, classifiersStre);
			}
		}

		public void AutomaticallyLearnUntilBeStable(
				ICollection<string> allPaths, ICollection<string> seedPaths, StreamWriter writer) {
			var allAsts = allPaths.Select(path => Processor.GenerateXml(new FileInfo(path), null, true))
					.ToList();
			var seedAsts = seedPaths.Select(path => Processor.GenerateXml(new FileInfo(path), null, true))
					.ToList();
			var seedAcceptedElements = seedAsts.SelectMany(GetAllElements).Where(IsAccepted);
			LearnUntilBeStable(allAsts, seedAsts, seedAcceptedElements, _elementNames, writer);

			var count = 0;
			var sumTime = Environment.TickCount;
			while (true) {
				var time = Environment.TickCount;
				count = LearnAndApply(count);
				var ret = AddNewElements(_idealAccepted.Keys);
				if (!ret) {
					count = 0;
				}
				else if (count < 0) {
					break;
				}

				Console.WriteLine("Time: " + (Environment.TickCount - time));
			}
			Console.WriteLine();
			Console.WriteLine("Sum time: " + (Environment.TickCount - sumTime));
			Console.WriteLine(
					"Required elements: " + (_accepted.Count + _rejected.Count) + " / "
					+ (_idealAccepted.Count + _idealRejected.Count));
			if (writer != null) {
				writer.WriteLine(_accepted.Count + _rejected.Count);
				writer.Flush();
			}

			//ShowBitsInfo();
		}

		public void ManuallyLearnUntilBeStable(
				IEnumerable<XElement> allAsts, IEnumerable<XElement> seedAsts, IEnumerable<XElement> seedElements) {
			var elementNames = seedElements.Select(e => e.Name());
			LearnUntilBeStable(allAsts, seedAsts, seedElements, elementNames, null);
		}

		public void LearnUntilBeStable(
				IEnumerable<XElement> allAsts, IEnumerable<XElement> seedAsts,
				IEnumerable<XElement> seedElements, IEnumerable<string> elementNames,
				StreamWriter writer) {
			var preparingTime = Environment.TickCount;

			_elementNames = elementNames.ToHashSet();
			_idealAccepted = new Dictionary<BigInteger, string>();
			_idealRejected = new Dictionary<BigInteger, string>();
			_accepted = new Dictionary<BigInteger, string>();
			_rejected = new Dictionary<BigInteger, string>();
			_feature2Element = new Dictionary<BigInteger, XElement>();
			_seedAccepted = new HashSet<BigInteger>();
			_classifiers = new List<string> { ">" };
			_masterPredicates = new Dictionary<string, BigInteger>();

			var seedAcceptedElements = seedElements.ToHashSet();
			var seedRejectedElements = new List<XElement>();
			foreach (var ast in seedAsts) {
				Console.Write(".");
				foreach (var e in GetAllElements(ast)) {
					if (!seedAcceptedElements.Contains(e)) {
						seedRejectedElements.Add(e);
					}
					//(IsAccepted(e) ? seedAcceptedElements : seedRejectedElements).Add(e);
				}
			}
			var acceptingPredicates = seedAcceptedElements
					.GetUnionKeys(_predicateDepth, IsInner, !IsInner)
					.ToHashSet()
					.ToList();
			acceptingPredicates.Sort((s1, s2) => s1.Length.CompareTo(s2.Length));

			var masterBit = BigInteger.One;
			foreach (var predicate in acceptingPredicates) {
				_masterPredicates.Add(predicate, masterBit);
				masterBit <<= 1;
			}
			_acceptingPredicatesCount = _masterPredicates.Count;

			var rejectingPredicateSet = seedRejectedElements
					.GetUnionKeys(_predicateDepth, IsInner, !IsInner)
					.ToHashSet();
			rejectingPredicateSet.ExceptWith(acceptingPredicates);
			var rejectingPredicates = rejectingPredicateSet.ToList();
			rejectingPredicates.Sort((s1, s2) => s1.Length.CompareTo(s2.Length));

			foreach (var predicate in rejectingPredicates) {
				_masterPredicates.Add(predicate, masterBit);
				masterBit <<= 1;
			}

			_mask = (BigInteger.One << _masterPredicates.Count) - BigInteger.One;
			_acceptingMask = (BigInteger.One << _acceptingPredicatesCount) - BigInteger.One;
			_rejectingMask = _mask ^ _acceptingMask;

			foreach (var e in seedAcceptedElements) {
				var bits = e.GetSurroundingBits(_predicateDepth, _masterPredicates, IsInner, !IsInner);
				UpdateDict(_idealAccepted, bits, e);
				_seedAccepted.Add(bits);
				_feature2Element[bits] = e;
			}
			foreach (var e in seedRejectedElements) {
				var bits = e.GetSurroundingBits(_predicateDepth, _masterPredicates, IsInner, !IsInner);
				UpdateDict(_idealRejected, bits, e);
				_feature2Element[bits] = e;
			}

			foreach (var ast in allAsts) {
				Console.Write(".");
				foreach (var e in GetAllElements(ast)) {
					var bits = e.GetSurroundingBits(_predicateDepth, _masterPredicates, IsInner, !IsInner);
					if (IsAccepted(e)) {
						//if (_idealRejected.ContainsKey(bits)) {
						//	Console.WriteLine(e.Text());
						//	Console.WriteLine(_feature2Element[bits].Text());
						//}
						UpdateDict(_idealAccepted, bits, e);
					} else {
						//if (_idealAccepted.ContainsKey(bits)) {
						//	Console.WriteLine(e.Text());
						//	Console.WriteLine(_feature2Element[bits].Text());
						//}
						UpdateDict(_idealRejected, bits, e);
					}
					_feature2Element[bits] = e;
				}
			}

			foreach (var e in seedAcceptedElements) {
				var bits = e.GetSurroundingBits(_predicateDepth, _masterPredicates, IsInner, !IsInner);
				_accepted[bits] = _idealAccepted[bits];
			}
			foreach (var e in seedRejectedElements) {
				var bits = e.GetSurroundingBits(_predicateDepth, _masterPredicates, IsInner, !IsInner);
				_rejected[bits] = _idealRejected[bits];
			}

			if (_idealAccepted.Keys.ToHashSet().Overlaps(_idealRejected.Keys.ToHashSet())) {
				throw new Exception("Master predicates can't classify elements!");
			}

			LearnPredicates();	// for the first time

			Console.WriteLine("Preparing time: " + (Environment.TickCount - preparingTime));
		}

		private int DetermineClassifierIndex(string classifier) {
			for (int i = _classifiers.Count - 1; i >= 0; i--) {
				if (classifier.StartsWith(_classifiers[i])) {
					return i;
				}
			}
			throw new Exception("Can't find the given classifier: " + classifier);
		}

		private void LearnPredicates() {
			while (true) {
				var acceptingPredicates = Enumerable.Repeat(_mask, _classifiers.Count).ToList();
				var rejectingPredicates = Enumerable.Repeat(BigInteger.Zero, _classifiers.Count).ToList();
				string failedClassifier = null;
				string classifier = null;
				int iClassifier = 0;
				foreach (var featureAndClassifier in _accepted) {
					var feature = featureAndClassifier.Key;
					classifier = featureAndClassifier.Value;
					iClassifier = DetermineClassifierIndex(classifier);
					rejectingPredicates[iClassifier] |= feature;
				}
				for (int i = 0; i < rejectingPredicates.Count; i++) {
					// ëSÇƒÇÃAcceptedÇ™îıÇ¶ÇƒÇ¢Ç»Ç¢ê´éøÇ1Ç…
					rejectingPredicates[i] ^= _rejectingMask;
					rejectingPredicates[i] &= _rejectingMask;
				}

				foreach (var featureAndClassifier in _accepted) {
					var feature = featureAndClassifier.Key;
					classifier = featureAndClassifier.Value;
					iClassifier = DetermineClassifierIndex(classifier);
					acceptingPredicates[iClassifier] &= feature;
					failedClassifier = CanReject(acceptingPredicates, rejectingPredicates, _rejected);
					if (failedClassifier != null) {
						break;
					}
				}
				if (failedClassifier == null) {
					_acceptingPredicates = acceptingPredicates;
					_rejectingPredicates = rejectingPredicates;
					return;
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
					var newClassifier = failedClassifier.Substring(0, i + 1);
					if (!_classifiers.Contains(newClassifier)) {
						_classifiers.Add(newClassifier);
					}
				}
				if (_classifiers.Count == count) {
					throw new Exception("Fail to learn rules");
				}
				Console.WriteLine("Classifiers: " + _classifiers.Count + " (" + count + ")");
			}
		}

		public int LearnAndApply(int count) {
			var acceptingPredicates = _acceptingPredicates;
			var rejectingPredicates = _rejectingPredicates;
			var correctlyAccepted = 0;
			var correctlyRejected = 0;
			var wronglyAccepted = 0;
			var wronglyRejected = 0;
			var correctlyRejectedInRejecting = 0;
			var wronglyRejectedInRejecting = 0;
			var rejectedInRejecting = new List<List<SuspiciousTarget>>();
			for (int i = 0; i < _classifiers.Count; i++) {
				rejectedInRejecting.Add(new List<SuspiciousTarget>());
			}
			var rejectedInAccepting = new List<List<SuspiciousTarget>>();
			for (int i = 0; i < _classifiers.Count; i++) {
				rejectedInAccepting.Add(new List<SuspiciousTarget>());
			}
			var acceptedInAccepting = new List<List<SuspiciousTarget>>();
			for (int i = 0; i < _classifiers.Count; i++) {
				acceptedInAccepting.Add(new List<SuspiciousTarget>());
			}
			_wronglyAcceptedFeatures.Clear();
			_wronglyRejectedFeatures.Clear();

			var index = 0;

			foreach (var featureAndClassifier in _idealAccepted) {
				var feature = featureAndClassifier.Key;
				var classifier = featureAndClassifier.Value;
				var iClassifier = DetermineClassifierIndex(classifier);
				var rejected = IsRejected(feature, rejectingPredicates[iClassifier]);
				var accepted = IsAccepted(feature, acceptingPredicates[iClassifier]);
				index++;
				if (!accepted) {
					wronglyRejected++;
					_wronglyRejectedFeatures.Add(featureAndClassifier);
					if (!_accepted.ContainsKey(feature) && !_rejected.ContainsKey(feature)) {
						// AcceptedÇ∆ã§í çÄÇ™ëΩÇ¢Ç‡ÇÃÇë_Ç§
						rejectedInAccepting[iClassifier].Add(
								new SuspiciousTarget {
									BitsCount = -CountAcceptingBits(feature),
									Feature = feature,
									Classifier = classifier,
								});
					}
				} else if (!rejected) {
					correctlyAccepted++;
					if (!_accepted.ContainsKey(feature) && !_rejected.ContainsKey(feature)) {
						// AcceptedÇ∆ã§í çÄÇ™è≠Ç»Ç¢Ç‡ÇÃÇë_Ç§
						acceptedInAccepting[iClassifier].Add(
								new SuspiciousTarget {
									BitsCount = CountAcceptingBits(feature),
									Feature = feature,
									Classifier = classifier,
								});
					}
				} else {
					wronglyRejectedInRejecting++;
					_wronglyRejectedFeatures.Add(featureAndClassifier);
					if (!_accepted.ContainsKey(feature) && !_rejected.ContainsKey(feature)) {
						// RejectedÇ∆ã§í çÄÇ™è≠Ç»Ç¢Ç‡ÇÃÇë_Ç§
						rejectedInRejecting[iClassifier].Add(
								new SuspiciousTarget {
									BitsCount = CountRejectingBits(feature & rejectingPredicates[iClassifier]),
									Feature = feature,
									Classifier = classifier,
								});
					}
				}
			}

			foreach (var featureAndClassifier in _idealRejected) {
				var feature = featureAndClassifier.Key;
				var classifier = featureAndClassifier.Value;
				var iClassifier = DetermineClassifierIndex(classifier);
				var rejected = IsRejected(feature, rejectingPredicates[iClassifier]);
				var accepted = IsAccepted(feature, acceptingPredicates[iClassifier]);
				index++;
				if (!accepted) {
					correctlyRejected++;
					if (!_accepted.ContainsKey(feature) && !_rejected.ContainsKey(feature)) {
						// AcceptedÇ∆ã§í çÄÇ™ëΩÇ¢Ç‡ÇÃÇë_Ç§
						rejectedInAccepting[iClassifier].Add(
								new SuspiciousTarget {
									BitsCount = -CountAcceptingBits(feature),
									Feature = feature,
									Classifier = classifier,
								});
					}
				} else if (!rejected) {
					wronglyAccepted++;
					_wronglyAcceptedFeatures.Add(featureAndClassifier);
					if (!_accepted.ContainsKey(feature) && !_rejected.ContainsKey(feature)) {
						// AcceptedÇ∆ã§í çÄÇ™è≠Ç»Ç¢Ç‡ÇÃÇë_Ç§
						acceptedInAccepting[iClassifier].Add(
								new SuspiciousTarget {
									BitsCount = CountAcceptingBits(feature),
									Feature = feature,
									Classifier = classifier,
								});
					}
				} else {
					correctlyRejectedInRejecting++;
					if (!_accepted.ContainsKey(feature) && !_rejected.ContainsKey(feature)) {
						// RejectedÇ∆ã§í çÄÇ™è≠Ç»Ç¢Ç‡ÇÃÇë_Ç§
						rejectedInRejecting[iClassifier].Add(
								new SuspiciousTarget {
									BitsCount = CountRejectingBits(feature & rejectingPredicates[iClassifier]),
									Feature = feature,
									Classifier = classifier,
								});
					}
				}
			}

			SuspiciousAcceptedInAccepting = new List<SuspiciousTarget>();
			SuspiciousRejectedInAccepting = new List<SuspiciousTarget>();
			SuspiciousRejectedInRejecting = new List<SuspiciousTarget>();
			switch (count) {
			case 0:
				//suspiciousAcceptedByAccepting = SelectSuspiciousElements(
				//		suspiciousAcceptedListByAccepting, acceptingPredicates, BigInteger.Zero, BigInteger.Zero);
				//		//suspiciousAcceptedListByAccepting, acceptingPredicates, BigInteger.Zero, BigInteger.Zero);
				//suspiciousRejectedByAccepting = SelectSuspiciousElements(
				//		suspiciousRejectedListByAccepting, acceptingPredicates, BigInteger.Zero, BigInteger.Zero);
				//suspiciousRejectedByRejecting = SelectSuspiciousElements(
				//		suspiciousRejectedListByRejecting, rejectingPredicates, _rejectingMask, _rejectingMask);
				//suspiciousAcceptedInAccepting = FlattenSuspiciousTargetsList(acceptedInAccepting);
				//suspiciousRejectedInAccepting = FlattenSuspiciousTargetsList(rejectedInAccepting);
				SuspiciousRejectedInRejecting = FlattenSuspiciousTargetsList(rejectedInRejecting);
				SuspiciousAcceptedInAccepting = SelectVariousElements(acceptedInAccepting, _acceptingMask);
				SuspiciousRejectedInAccepting = SelectVariousElements(rejectedInAccepting, _acceptingMask);
				//suspiciousRejectedInRejecting = SelectVariousElements(rejectedInRejecting, _rejectingMask);
				break;
			case 1:
				SuspiciousAcceptedInAccepting = SelectSuspiciousElementsWithMask(
						acceptedInAccepting, BigInteger.Zero, _acceptingMask);
				SuspiciousRejectedInAccepting = SelectSuspiciousElementsWithMask(
						rejectedInAccepting, BigInteger.Zero, _acceptingMask);
				SuspiciousRejectedInRejecting = SelectSuspiciousElementsWithMask(
						rejectedInRejecting, _rejectingMask, _rejectingMask);
				//suspiciousAcceptedByAccepting = SelectSuspiciousElements(
				//		suspiciousAcceptedListByAccepting, acceptingPredicates, _acceptingMask, BigInteger.Zero);
				//suspiciousRejectedByAccepting = SelectSuspiciousElements(
				//		suspiciousRejectedListByAccepting, acceptingPredicates, BigInteger.Zero, BigInteger.Zero);
				//suspiciousRejectedByRejecting = SelectSuspiciousElements(
				//		suspiciousRejectedListByRejecting, rejectingPredicates, _rejectingMask, _rejectingMask);
				break;
			case 2:
				SuspiciousAcceptedInAccepting = SelectSuspiciousElementsWithMask(
						acceptedInAccepting, BigInteger.Zero, _acceptingMask);
				SuspiciousRejectedInAccepting = SelectSuspiciousElementsWithMask(
						rejectedInAccepting, BigInteger.Zero, _acceptingMask);
				SuspiciousRejectedInRejecting = SelectSuspiciousElementsWithMask(
						rejectedInRejecting, _rejectingMask, _rejectingMask);
				break;
			default:
				return -1;
			}
			Console.WriteLine(
					"SA(A): " + SuspiciousAcceptedInAccepting.Count
					+ ", SR(A): " + SuspiciousRejectedInAccepting.Count
					+ ", SR(R): " + SuspiciousRejectedInRejecting.Count);
			Console.WriteLine("done");
			Console.WriteLine(
					"WA: " + wronglyAccepted + ", WR: " + wronglyRejected + "/"
					+ wronglyRejectedInRejecting + ", CA: "
					+ correctlyAccepted + ", CR: " + correctlyRejected + "/"
					+ correctlyRejectedInRejecting);
			WrongCount = wronglyAccepted + wronglyRejected + wronglyRejectedInRejecting;

			return count + 1;
		}

		public bool AddNewElements(ICollection<BigInteger> actuallyAcceptedElements) {
			var additionalAccepted =
					SuspiciousAcceptedInAccepting.Concat(SuspiciousRejectedInAccepting)
							.Concat(SuspiciousRejectedInRejecting)
							.Where(t => actuallyAcceptedElements.Contains(t.Feature))
							.ToList();
			var additionalRejected =
					SuspiciousAcceptedInAccepting.Concat(SuspiciousRejectedInAccepting)
							.Concat(SuspiciousRejectedInRejecting)
							.Where(t => !actuallyAcceptedElements.Contains(t.Feature))
							.ToList();
			var foundWronglyRejected = SuspiciousRejectedInAccepting.Concat(SuspiciousRejectedInRejecting)
					.Count(t => actuallyAcceptedElements.Contains(t.Feature));
			var foundWronglyAccepted = SuspiciousAcceptedInAccepting
					.Count(t => !actuallyAcceptedElements.Contains(t.Feature));

			var additionalAcceptedCount = additionalAccepted.Count;
			var additionalRejectedCount = additionalRejected.Count;

			Console.WriteLine(
					"L: " + (_accepted.Count + _rejected.Count) + ", AP: "
					+ string.Join(", ", _acceptingPredicates.Select(CountBits)) + ", RP: "
					+ string.Join(", ", _rejectingPredicates.Select(CountRejectingBits)));
			Console.WriteLine(
					"Accepted: " + _accepted.Count + " + " + additionalAcceptedCount
					+ " / " + _idealAccepted.Count);
			Console.WriteLine(
					"Rejected: " + _rejected.Count + " + " + additionalRejectedCount
					+ " / " + _idealRejected.Count);
			foreach (var suspiciousTarget in additionalAccepted) {
				_accepted.Add(suspiciousTarget.Feature, suspiciousTarget.Classifier);
			}
			foreach (var suspiciousTarget in additionalRejected) {
				_rejected.Add(suspiciousTarget.Feature, suspiciousTarget.Classifier);
			}

			var lastAcceptingPredicates = _acceptingPredicates.ToList();
			var lastRejectingPredicates = _rejectingPredicates.ToList();
			LearnPredicates();
			var ret = foundWronglyRejected + foundWronglyAccepted == 0
			          && lastAcceptingPredicates.SequenceEqual(_acceptingPredicates)
			          && lastRejectingPredicates.SequenceEqual(_rejectingPredicates);
			return ret;
		}

		private List<SuspiciousTarget> SelectSuspiciousElements(
				List<List<SuspiciousTarget>> suspiciousElementsList, List<BigInteger> predicates,
				BigInteger predXor, BigInteger featureXor) {
			var suspiciousElements = new List<SuspiciousTarget>();
			for (int i = 0; i < suspiciousElementsList.Count; i++) {
				var feature = BigInteger.Zero;
				var list = suspiciousElementsList[i];
				var pred = predicates[i] ^ predXor;
				list.Sort((t1, t2) => t1.BitsCount.CompareTo(t2.BitsCount));
				foreach (var t in list) {
					var newFeature = (feature | (t.Feature ^ featureXor)) & pred;
					if (newFeature != feature) {
						feature = newFeature;
						suspiciousElements.Add(t);
					}
				}
			}
			return suspiciousElements;
		}

		private List<SuspiciousTarget> SelectSuspiciousElementsWithMask(
				List<List<SuspiciousTarget>> suspiciousElementsList, BigInteger xor, BigInteger mask) {
			var suspiciousElements = new List<SuspiciousTarget>();
			for (int i = 0; i < suspiciousElementsList.Count; i++) {
				var feature = BigInteger.Zero;
				var list = suspiciousElementsList[i];
				list.Sort((t1, t2) => t1.BitsCount.CompareTo(t2.BitsCount));
				foreach (var t in list) {
					var newFeature = (feature | (t.Feature ^ xor)) & mask;
					if (newFeature != feature) {
						feature = newFeature;
						suspiciousElements.Add(t);
					}
				}
			}
			return suspiciousElements;
		}

		private List<SuspiciousTarget> FlattenSuspiciousTargetsList(
				List<List<SuspiciousTarget>> targetsList) {
			var ret = new List<SuspiciousTarget>();
			var indices = Enumerable.Repeat(0, targetsList.Count).ToList();
			foreach (List<SuspiciousTarget> list in targetsList) {
				list.Sort((t1, t2) => t1.BitsCount.CompareTo(t2.BitsCount));
			}
			while (ret.Count < LearningCount) {
				var added = false;
				for (int i = 0; i < targetsList.Count; i++) {
					var list = targetsList[i];
					if (indices[i] < list.Count) {
						ret.Add(list[indices[i]++]);
						added = true;
					}
				}
				if (!added) {
					break;
				}
			}
			return ret;
		}

		private SuspiciousTarget SelectMostDifferentElement(
				IList<SuspiciousTarget> existings, IEnumerable<SuspiciousTarget> targets, BigInteger mask) {
			if (existings.Count == 0) {
				return targets.FirstOrDefault();
			}
			var maxDiff = 0;
			SuspiciousTarget ret = null;
			foreach (var t in targets) {
				var feature = t.Feature & mask;
				var diff = existings.Min(e => CountBits((e.Feature & mask) ^ feature));
				if (maxDiff < diff) {
					maxDiff = diff;
					ret = t;
				}
			}
			return ret;
		}

		private List<SuspiciousTarget> SelectVariousElements(
				List<List<SuspiciousTarget>> targetsList, BigInteger mask) {
			var ret = new List<SuspiciousTarget>();
			foreach (List<SuspiciousTarget> list in targetsList) {
				list.Sort((t1, t2) => t1.BitsCount.CompareTo(t2.BitsCount));
			}
			while (ret.Count < LearningCount) {
				var added = false;
				foreach (var list in targetsList) {
					var e = SelectMostDifferentElement(ret, list, mask);
					if (e != null) {
						ret.Add(e);
						added = true;
					}
				}
				if (!added) {
					break;
				}
			}
			return ret;
		}

		private string CanReject(
				IList<BigInteger> acceptingPredicates, List<BigInteger> rejectingPredicates,
				Dictionary<BigInteger, string> rejected) {
			foreach (var featureAndClassifier in rejected) {
				var feature = featureAndClassifier.Key;
				var classifier = featureAndClassifier.Value;
				var iClassifier = DetermineClassifierIndex(classifier);
				if (IsAccepted(feature, acceptingPredicates[iClassifier])
				    && !IsRejected(feature, rejectingPredicates[iClassifier])) {
					return classifier;
				}
			}
			return null;
		}

		private bool IsAccepted(BigInteger feature, BigInteger acceptingPredicate) {
			// ëSÇƒÇÃê´éøÇîıÇ¶ÇƒÇ¢ÇÍÇŒAccepted
			return (feature & acceptingPredicate) == acceptingPredicate;
		}

		private bool IsRejected(BigInteger feature, BigInteger rejectingPredicate) {
			// Ç¢Ç∏ÇÍÇ©ÇÃê´éøÇîıÇ¶ÇƒÇ¢ÇÍÇŒRejected
			// RejectÇ™Ç»Ç¢èÍçáÇ‡ê≥èÌÇ…ìÆçÏ
			return (feature & rejectingPredicate) != BigInteger.Zero;
		}

		private static int CountBits(BigInteger bits) {
			var count = 0;
			while (bits != BigInteger.Zero) {
				count += (int)(bits & BigInteger.One);
				bits >>= 1;
			}
			return count;
		}

		private int CountAcceptingBits(BigInteger bits) {
			return CountBits(bits & _acceptingMask);
		}

		private int CountRejectingBits(BigInteger bits) {
			return CountBits(bits >> _acceptingPredicatesCount);
		}

		private void ShowBitsInfo() {
			LearnPredicates();

			Console.WriteLine("---------------------------------------------------");
			Console.Write("A(A): ");
			foreach (var f in _idealAccepted.Keys) {
				Console.Write(CountAcceptingBits(f) + ", ");
			}
			Console.WriteLine();
			Console.Write("R(A): ");
			foreach (var f in _idealRejected.Keys) {
				Console.Write(CountAcceptingBits(f) + ", ");
			}
			Console.WriteLine();
			Console.Write("WA(A): ");
			foreach (var f in _wronglyAcceptedFeatures.Select(kv => kv.Key)) {
				Console.Write(CountAcceptingBits(f) + ", ");
			}
			Console.WriteLine();
			Console.Write("WR(A): ");
			foreach (var f in _wronglyRejectedFeatures.Select(kv => kv.Key)) {
				Console.Write(CountAcceptingBits(f) + ", ");
			}
			Console.WriteLine();

			Console.WriteLine("---------------------------------------------------");
			Console.Write("A(R): ");
			foreach (var f in _idealAccepted.Keys) {
				Console.Write(CountRejectingBits(f) + ", ");
			}
			Console.WriteLine();
			Console.Write("R(R): ");
			foreach (var f in _idealRejected.Keys) {
				Console.Write(CountRejectingBits(f) + ", ");
			}
			Console.WriteLine();
			Console.Write("WA(R): ");
			foreach (var f in _wronglyAcceptedFeatures.Select(kv => kv.Key)) {
				Console.Write(CountRejectingBits(f) + ", ");
			}
			Console.WriteLine();
			Console.Write("WR(R): ");
			foreach (var f in _wronglyRejectedFeatures.Select(kv => kv.Key)) {
				Console.Write(CountRejectingBits(f) + ", ");
			}
			Console.WriteLine();

			Console.WriteLine("---------------------------------------------------");
			Console.Write("A(A): ");
			foreach (var featureAndClassifier in _idealAccepted) {
				var feature = featureAndClassifier.Key;
				var classifier = featureAndClassifier.Value;
				var iClassifier = DetermineClassifierIndex(classifier);
				Console.Write(CountAcceptingBits(feature & _acceptingPredicates[iClassifier]) + ", ");
			}
			Console.WriteLine();
			Console.Write("R(A): ");
			foreach (var featureAndClassifier in _idealRejected) {
				var feature = featureAndClassifier.Key;
				var classifier = featureAndClassifier.Value;
				var iClassifier = DetermineClassifierIndex(classifier);
				Console.Write(CountAcceptingBits(feature & _acceptingPredicates[iClassifier]) + ", ");
			}
			Console.WriteLine();
			Console.Write("WA(A): ");
			foreach (var featureAndClassifier in _wronglyAcceptedFeatures) {
				var feature = featureAndClassifier.Key;
				var classifier = featureAndClassifier.Value;
				var iClassifier = DetermineClassifierIndex(classifier);
				Console.Write(CountAcceptingBits(feature & _acceptingPredicates[iClassifier]) + ", ");
			}
			Console.WriteLine();
			Console.Write("WR(A): ");
			foreach (var featureAndClassifier in _wronglyRejectedFeatures) {
				var feature = featureAndClassifier.Key;
				var classifier = featureAndClassifier.Value;
				var iClassifier = DetermineClassifierIndex(classifier);
				Console.Write(CountAcceptingBits(feature & _acceptingPredicates[iClassifier]) + ", ");
			}
			Console.WriteLine();

			Console.WriteLine("---------------------------------------------------");
			Console.Write("A(R): ");
			foreach (var featureAndClassifier in _idealAccepted) {
				var feature = featureAndClassifier.Key;
				var classifier = featureAndClassifier.Value;
				var iClassifier = DetermineClassifierIndex(classifier);
				Console.Write(CountRejectingBits(feature & _rejectingPredicates[iClassifier]) + ", ");
			}
			Console.WriteLine();
			Console.Write("R(R): ");
			foreach (var featureAndClassifier in _idealRejected) {
				var feature = featureAndClassifier.Key;
				var classifier = featureAndClassifier.Value;
				var iClassifier = DetermineClassifierIndex(classifier);
				Console.Write(CountRejectingBits(feature & _rejectingPredicates[iClassifier]) + ", ");
			}
			Console.WriteLine();
			Console.Write("WA(R): ");
			foreach (var featureAndClassifier in _wronglyAcceptedFeatures) {
				var feature = featureAndClassifier.Key;
				var classifier = featureAndClassifier.Value;
				var iClassifier = DetermineClassifierIndex(classifier);
				Console.Write(CountRejectingBits(feature & _rejectingPredicates[iClassifier]) + ", ");
			}
			Console.WriteLine();
			Console.Write("WR(R): ");
			foreach (var featureAndClassifier in _wronglyRejectedFeatures) {
				var feature = featureAndClassifier.Key;
				var classifier = featureAndClassifier.Value;
				var iClassifier = DetermineClassifierIndex(classifier);
				Console.Write(CountRejectingBits(feature & _rejectingPredicates[iClassifier]) + ", ");
			}
			Console.WriteLine();
		}

		protected IEnumerable<XElement> GetAllElements(XElement ast) {
			return ast.DescendantsAndSelf().Where(e => _elementNames.Contains(e.Name()));
		}

		public abstract bool IsAccepted(XElement e);
	}
}