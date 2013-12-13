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
			var seedAcceptedElements = new HashSet<XElement>();
			var seedRejectedElements = new HashSet<XElement>();
			foreach (var path in seedPaths) {
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				seedRejectedElements.UnionWith(GetAllElements(ast));
				seedAcceptedElements.UnionWith(GetAcceptedElements(ast));
			}
			seedRejectedElements.ExceptWith(seedAcceptedElements);
			_masterPredicates = seedAcceptedElements.GetUnionKeys(_predicateDepth).ToList();
			_featureCount = _masterPredicates.Count;

			_seedAccepted = seedAcceptedElements
					.Select(e => GetBits(e, _masterPredicates))
					.ToHashSet();
			_rejected = seedRejectedElements
					.Select(e => GetBits(e, _masterPredicates))
					.ToHashSet();
			_accepted = new List<BigInteger>();

			var acceptedFeaturesCounter = new CountingSet<BigInteger>();
			var allFeaturesCounter = new CountingSet<BigInteger>();
			foreach (var path in allPaths) {
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				allFeaturesCounter.UnionWith(GetAllElements(ast)
						.Select(e => GetBits(e, _masterPredicates)));
				acceptedFeaturesCounter.UnionWith(GetAcceptedElements(ast)
						.Select(e => GetBits(e, _masterPredicates)));
			}
			const int initialFeatureCount = 5;
			var initialMaxFeatures = allFeaturesCounter
					.ItemsWithCount.OrderByDescending(kv => kv.Value)
					.Select(kv => kv.Key)
					.Where(f => !_seedAccepted.Contains(f) && !_rejected.Contains(f))
					.Take(initialFeatureCount)
					.ToList();
			var initialMinFeatures = allFeaturesCounter
					.ItemsWithCount.OrderBy(kv => kv.Value)
					.Select(kv => kv.Key)
					.Where(f => !_seedAccepted.Contains(f) && !_rejected.Contains(f))
					.Take(initialFeatureCount)
					.ToList();

			allFeaturesCounter.ExceptWith(acceptedFeaturesCounter);
			_acceptedFeatures = acceptedFeaturesCounter.ToHashSet();
			_rejectedFeatures = allFeaturesCounter.ToHashSet();
			Console.WriteLine("Max A: " + acceptedFeaturesCounter.ItemsWithCount.Max(kv => kv.Value) +
			                  ", Min A: " + acceptedFeaturesCounter.ItemsWithCount.Min(kv => kv.Value) +
			                  ", Max R: " + allFeaturesCounter.ItemsWithCount.Max(kv => kv.Value) +
			                  ", Min R: " + allFeaturesCounter.ItemsWithCount.Min(kv => kv.Value));

			foreach (var feature in initialMaxFeatures.Concat(initialMinFeatures)) {
				if (_rejectedFeatures.Contains(feature)) {
					_rejected.Add(feature);
				} else {
					_accepted.Add(feature);
				}
			}

			_rejectedFeatures.UnionWith(_rejected);
			var count = 2;
			while (true) {
				var time = Environment.TickCount;
				if (LearnAndApply()) {
					if (--count == 0) {
						break;
					}
					LearnPredicates();
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

		private static double Dispersion(IEnumerable<int> values) {
			var list = values as IList<int> ?? values.ToList();
			var average = list.Average();
			return list.Select(v => (average - v) * (average - v)).Average();
		}

		private List<BigInteger> LearnPredicatesFromMinDispersionAccepted() {
			var learningSet = _seedAccepted
					.Select(s => new Stack<BigInteger>(Enumerable.Repeat(s, 1)))
					.ToList();
			var acceptedSet = _accepted.ToHashSet();
			while (acceptedSet.Count > 0) {
				var selectedTuple = acceptedSet
						.Select(a => {
							var minSum = double.MaxValue;
							var index = -1;
							for (int i = 0; i < learningSet.Count; i++) {
								learningSet[i].Push(a);
								var predicates = learningSet.Select(GetPredicate).ToList();
								if (CanReject(predicates, _rejected)) {
									var count = predicates.Select(CountBits).Sum();
									if (minSum > count) {
										minSum = count;
										index = i;
									}
								}
								learningSet[i].Pop();
							}
							return Tuple.Create(a, minSum, index);
						})
						.Where(t => t.Item3 >= 0)
						.MinElementOrDefault(t => t.Item2);
				if (selectedTuple == null) {
					throw new Exception("Fail to learn rules");
				}
				learningSet[selectedTuple.Item3].Push(selectedTuple.Item1);
				acceptedSet.Remove(selectedTuple.Item1);
			}
			return learningSet.Select(GetPredicate).ToList();
		}

		private List<BigInteger> LearnPredicatesFromMinMinAccepted() {
			var learningSet = _seedAccepted
					.Select(s => new Stack<BigInteger>(Enumerable.Repeat(s, 1)))
					.ToList();
			var acceptedSet = _accepted.ToHashSet();
			while (acceptedSet.Count > 0) {
				var selectedTuple = acceptedSet
						.Select(a => {
							var minCount = int.MaxValue;
							var minIndex = -1;
							for (int i = 0; i < learningSet.Count; i++) {
								learningSet[i].Push(a);
								var predicates = learningSet.Select(GetPredicate).ToList();
								if (CanReject(predicates, _rejected)) {
									var count = CountRejected(predicates);
									if (minCount > count) {
										minCount = count;
										minIndex = i;
									}
								}
								learningSet[i].Pop();
							}
							return Tuple.Create(a, minCount, minIndex);
						})
						.Where(t => t.Item3 >= 0)
						.MinElementOrDefault(t => t.Item2);
				if (selectedTuple == null) {
					throw new Exception("Fail to learn rules");
				}
				learningSet[selectedTuple.Item3].Push(selectedTuple.Item1);
				acceptedSet.Remove(selectedTuple.Item1);
			}
			return learningSet.Select(GetPredicate).ToList();
		}

		private List<BigInteger> LearnPredicatesFromMinAccepted() {
			var learningSet = _seedAccepted
					.Select(s => new Stack<BigInteger>(Enumerable.Repeat(s, 1)))
					.ToList();
			var acceptedSet = _accepted.ToHashSet();
			while (acceptedSet.Count > 0) {
				var selectedTuple = acceptedSet
						.Select(a => {
							var maxCount = -1;
							var maxIndex = -1;
							for (int i = 0; i < learningSet.Count; i++) {
								learningSet[i].Push(a);
								var predicates = learningSet.Select(GetPredicate).ToList();
								if (CanReject(predicates, _rejected)) {
									var count = CountRejected(predicates);
									if (maxCount < count) {
										maxCount = count;
										maxIndex = i;
									}
								}
								learningSet[i].Pop();
							}
							return Tuple.Create(a, maxCount, maxIndex);
						})
						.Where(t => t.Item3 >= 0)
						.MinElementOrDefault(t => t.Item2);
				if (selectedTuple == null) {
					throw new Exception("Fail to learn rules");
				}
				learningSet[selectedTuple.Item3].Push(selectedTuple.Item1);
				acceptedSet.Remove(selectedTuple.Item1);
			}
			return learningSet.Select(GetPredicate).ToList();
		}

		private List<BigInteger> LearnPredicatesFromMaxAccepted() {
			var learningSet = _seedAccepted
					.Select(s => new Stack<BigInteger>(Enumerable.Repeat(s, 1)))
					.ToList();
			var acceptedSet = _accepted.ToHashSet();
			while (acceptedSet.Count > 0) {
				var selectedTuple = acceptedSet
						.Select(a => {
							var maxCount = -1;
							var maxIndex = -1;
							for (int i = 0; i < learningSet.Count; i++) {
								learningSet[i].Push(a);
								var predicates = learningSet.Select(GetPredicate).ToList();
								if (CanReject(predicates, _rejected)) {
									var count = CountRejected(predicates);
									if (maxCount < count) {
										maxCount = count;
										maxIndex = i;
									}
								}
								learningSet[i].Pop();
							}
							return Tuple.Create(a, maxCount, maxIndex);
						})
						.Where(t => t.Item3 >= 0)
						.MaxElementOrDefault(t => t.Item2);
				if (selectedTuple == null) {
					throw new Exception("Fail to learn rules");
				}
				learningSet[selectedTuple.Item3].Push(selectedTuple.Item1);
				acceptedSet.Remove(selectedTuple.Item1);
			}
			return learningSet.Select(GetPredicate).ToList();
		}

		private List<BigInteger> LearnPredicates() {
			while (true) {
				var preds1 = LearnPredicatesFromMaxAccepted();
				var preds2 = LearnPredicatesFromMinAccepted();
				var newAccepted = _acceptedFeatures
						.Where(f => IsRejected(preds1, f) != IsRejected(preds2, f))
						.ToList();
				var newRejected = _rejectedFeatures
						.Where(f => IsRejected(preds1, f) != IsRejected(preds2, f))
						.ToList();
				_accepted.AddRange(newAccepted);
				_rejected.UnionWith(newRejected);
				Console.WriteLine("A: " + newAccepted.Count + ", R: " + newRejected.Count);
				if (newAccepted.Count + newRejected.Count == 0) {
					break;
				}
			}
			return LearnPredicatesFromMaxAccepted();
		}

		private List<BigInteger> OriginalLearnPredicates() {
			var candidatePredicates = EnumeratePredicates()
					.Where(predicates => CanReject(predicates, _rejected));
			return candidatePredicates
					.MaxElementOrDefault(CountRejected);
		}

		private int CountRejected(IEnumerable<BigInteger> predicates) {
			return _acceptedFeatures.Concat(_rejectedFeatures)
					.Count(feature => IsRejected(predicates, feature));
		}

		private static bool IsRejected(IEnumerable<BigInteger> predicates, BigInteger feature) {
			return predicates.All(p => (feature & p) != p);
		}

		private bool LearnAndApply() {
			var predicates = LearnPredicatesFromMinMinAccepted();
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