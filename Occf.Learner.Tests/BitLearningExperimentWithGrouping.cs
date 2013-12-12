using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;
using Code2Xml.Core;
using Occf.Learner.Core.Tests.LearningAlgorithms;
using Paraiba.Linq;

namespace Occf.Learner.Core.Tests {
	internal class NextTarget {
		public int Differential { get; set; }
		public BigInteger Feature { get; set; }
		public BigInteger DiffPattern { get; set; }
	}

	public abstract class BitLearningExperimentWithGrouping {
		public int WrongCount { get; set; }
		protected abstract Processor Processor { get; }
		private readonly ISet<string> _elementNames;
		private HashSet<BigInteger> _acceptedFeatures;
		private HashSet<BigInteger> _rejectedFeatures;
		private List<BigInteger> _accepted;
		private HashSet<BigInteger> _rejected;
		private HashSet<BigInteger> _seedAccepted;
		private HashSet<BigInteger> _learnedDiffs;
		private int _predicateDepth = 10;
		private List<string> _masterPredicates;
		private int _featureCount;
		private const int LearningCount = 5;

		protected BitLearningExperimentWithGrouping(params string[] elementNames) {
			_elementNames = elementNames.ToHashSet();
		}

		public void LearnUntilBeStable(
				ICollection<string> allPaths, ICollection<string> seedPaths, LearningAlgorithm learner,
				double threshold) {
			_learnedDiffs = new HashSet<BigInteger>();

			var seedAccepted = new HashSet<XElement>();
			var seedRejected = new HashSet<XElement>();
			foreach (var path in seedPaths) {
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				seedRejected.UnionWith(GetAllElements(ast).ToHashSet());
				seedAccepted.UnionWith(GetAcceptedElements(ast));
			}
			seedRejected.ExceptWith(seedAccepted);
			_masterPredicates = seedAccepted.GetUnionKeys(_predicateDepth).ToList();
			_featureCount = _masterPredicates.Count;
			_seedAccepted = seedAccepted.Select(e => GetBits(e, _masterPredicates))
					.ToHashSet();
			_rejected = seedRejected.Select(e => GetBits(e, _masterPredicates))
					.ToHashSet();
			_accepted = new List<BigInteger>();

			_acceptedFeatures = new HashSet<BigInteger>();
			_rejectedFeatures = new HashSet<BigInteger>();
			foreach (var path in allPaths) {
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				var rejectedElements = GetAllElements(ast).ToHashSet();
				var acceptedElements = GetAcceptedElements(ast).ToHashSet();
				rejectedElements.ExceptWith(acceptedElements);
				_acceptedFeatures.UnionWith(acceptedElements
						.Select(e => GetBits(e, _masterPredicates)));
				_rejectedFeatures.UnionWith(rejectedElements
						.Select(e => GetBits(e, _masterPredicates)));
			}
			_rejectedFeatures.UnionWith(_rejected);

			var count = 2;
			while (true) {
				var time = Environment.TickCount;
				if (LearnAndApply() && --count == 0) {
					break;
				}
				Console.WriteLine("Time: " + (Environment.TickCount - time));
			}
			Console.WriteLine("Required elements: " + (_accepted.Count + _rejected.Count));
		}

		private bool CanReject(IEnumerable<BigInteger> predicates, ICollection<BigInteger> rejected) {
			return predicates.All(p => rejected.All(r => (r & p) != p));
		}

		private BigInteger GetPredicate(IEnumerable<BigInteger> features) {
			return features.Aggregate((v1, v2) => v1 & v2);
		}

		private IEnumerable<List<BigInteger>> EnumeratePredicates() {
			var learningSet = _seedAccepted
					.Select(s => new Stack<BigInteger>(Enumerable.Repeat(s, 1)))
					.ToList();
			var stack = new Stack<int>();
			stack.Push(-1);
			var i = 0;
			var count = 0;
			var skipped = 0;
			while (true) {
				if (count++ % 100000 == 0) {
					Console.WriteLine(count + ", " + skipped);
				}
				while (stack.Count - 1 < _accepted.Count) {
					if (!CanReject(learningSet.Select(GetPredicate), _rejected)) {
						skipped++;
						break;
					}
					learningSet[i].Push(_accepted[stack.Count - 1]);
					stack.Push(i);
					i = 0;
				}
				yield return learningSet.Select(GetPredicate).ToList();
				do {
					i = stack.Pop();
					if (i == -1) {
						yield break;
					}
					learningSet[i].Pop();
					i++;
				} while (i == learningSet.Count);
			}
		}

		private List<BigInteger> LearnCommonKeys() {
			var candidatePredicates = EnumeratePredicates()
					.Where(predicates => CanReject(predicates, _rejected));
			return candidatePredicates
					.MaxElementOrDefault(CountRejected);
		}

		private int CountRejected(IEnumerable<BigInteger> predicates) {
			return _acceptedFeatures.Concat(_rejectedFeatures)
					.Count(feature => predicates.All(p => (feature & p) != p));
		}

		private bool LearnAndApply() {
			var predicates = LearnCommonKeys();
			BigInteger diffPattern;
			int minIndex;

			var correctlyAccepted = 0;
			var correctlyRejected = 0;
			var wronglyAccepted = 0;
			var wronglyRejected = 0;
			var nextAcceptedList = new List<List<NextTarget>>();
			for (int i = 0; i < _seedAccepted.Count; i++) {
				nextAcceptedList.Add(new List<NextTarget>());
			}
			var nextRejectedList = new List<List<NextTarget>>();
			for (int i = 0; i < _seedAccepted.Count; i++) {
				nextRejectedList.Add(new List<NextTarget>());
			}

			foreach (var feature in _acceptedFeatures) {
				var differential = GetClassifierInput(feature, predicates, out minIndex, out diffPattern);
				var actual = differential == 0;
				if (actual) {
					correctlyAccepted++;
				} else {
					if (wronglyRejected == 0) {
						//Console.WriteLine("WR (" + prob + "): " + e.SafeParent().SafeParent().SafeParent());
					}
					wronglyRejected++;
				}
				if (!actual && !_seedAccepted.Contains(feature)) {
					UpdateNextElements(nextAcceptedList[minIndex], feature, differential, diffPattern);
				}
			}

			foreach (var feature in _rejectedFeatures) {
				var differential = GetClassifierInput(feature, predicates, out minIndex, out diffPattern);
				var actual = differential == 0;
				if (actual) {
					if (wronglyAccepted == 0) {
						//Console.WriteLine("WA (" + prob + "): " + e.SafeParent().SafeParent().SafeParent());
					}
					wronglyAccepted++;
				} else {
					correctlyRejected++;
				}
				if (!actual && !_rejected.Contains(feature)) {
					UpdateNextElements(nextRejectedList[minIndex], feature, differential, diffPattern);
				}
			}
			var nextAcceptedCount = nextAcceptedList.Select(l => l.Count).Sum();
			var nextRejectedCount = nextRejectedList.Select(l => l.Count).Sum();

			Console.WriteLine("done");
			Console.WriteLine("WA: " + wronglyAccepted + ", WR: " + wronglyRejected + ", CA: "
			                  + correctlyAccepted + ", CR: " + correctlyRejected + ", L: "
			                  + (_accepted.Count + _rejected.Count) + ", P: "
			                  + string.Join(", ", predicates.Select(CountBits)));
			Console.WriteLine("Accepted: " + _accepted.Count + " + " + nextAcceptedCount
			                  + " / " + _acceptedFeatures.Count);
			Console.WriteLine("Rejected: " + _rejected.Count + " + " + nextRejectedCount
			                  + " / " + _rejectedFeatures.Count);
			WrongCount = wronglyAccepted + wronglyRejected;
			foreach (var nextTargets in nextAcceptedList) {
				foreach (var nextTarget in nextTargets) {
					_accepted.Add(nextTarget.Feature);
				}
			}
			foreach (var nextTargets in nextRejectedList) {
				foreach (var nextTarget in nextTargets) {
					_rejected.Add(nextTarget.Feature);
				}
			}
			return nextAcceptedCount + nextRejectedCount == 0;
		}

		private void UpdateNextElements(
				List<NextTarget> nextTargets, BigInteger feature, int differential, BigInteger diffPattern) {
				if (!_learnedDiffs.Contains(diffPattern)) {
					_learnedDiffs.Add(diffPattern);
					nextTargets.Add(new NextTarget {
						Differential = differential,
						DiffPattern = diffPattern,
						Feature = feature,
					});
					nextTargets.Sort((t1, t2) => t1.Differential.CompareTo(t2.Differential));
					if (nextTargets.Count > LearningCount) {
						nextTargets.RemoveAt(LearningCount);
					}
				}
		}

		private int GetClassifierInput(
				BigInteger target, IEnumerable<BigInteger> predicates, out int minIndex, out BigInteger diffs) {
			var minCount = int.MaxValue;
			var minDiffs = BigInteger.Zero;
			var index = 0;
			minIndex = 0;
			foreach (var predicate in predicates) {
				var diff = (target & predicate) ^ predicate;
				var count = CountBits(diff);
				if (minCount > count) {
					minCount = count;
					minDiffs = diff;
					minIndex = index;
				}
				index++;
			}
			diffs = minDiffs;
			return minCount;
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

		protected abstract IEnumerable<XElement> GetAcceptedElements(XElement ast);
	}
}