using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;
using Code2Xml.Core;
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
		private List<bool> _previousActuals;
		private const int LearningCount = 5;
		private bool _isFirstTime;

		protected BitLearningExperimentWithGrouping(params string[] elementNames) {
			_elementNames = elementNames.ToHashSet();
		}

		public void CheckLearnable(ICollection<string> allPaths, ICollection<string> seedPaths) {
			var acceptedElements = new List<Tuple<XElement, int>>();
			var rejectedElements = new HashSet<XElement>();

			foreach (var path in seedPaths.Concat(allPaths)) {
				var codeFile = new FileInfo(path);
				try {
					var ast = Processor.GenerateXml(codeFile, null, true);
					rejectedElements.UnionWith(GetAllElements(ast));
					acceptedElements.AddRange(GetAcceptedElements(ast));
				} catch (Exception e) {
					Console.WriteLine(e.Message);
					Console.WriteLine(path);
				}
			}
			rejectedElements.ExceptWith(acceptedElements.Select(t => t.Item1));
			var groupedAcceptedElements = acceptedElements
					.GroupBy(t => t.Item2, t => t.Item1)
					.ToList();

			var masterPredicates = groupedAcceptedElements
					.Select(g => g.GetCommonKeys(_predicateDepth))
					.Aggregate((ks1, ks2) => {
						ks1.UnionWith(ks2);
						return ks1;
					}).ToList();

			var acceptedFeatures = groupedAcceptedElements
					.Select(g => g.Select(e => GetBits(e, masterPredicates))
							.Aggregate((f1, f2) => f1 & f2))
					.ToHashSet();
			var rejectedFeatures = rejectedElements
					.Select(e => GetBits(e, masterPredicates))
					.ToHashSet();

			var idealPredicates = acceptedFeatures
					.Select(s => new Stack<BigInteger>(Enumerable.Repeat(s, 1)))
					.Select(GetPredicate)
					.ToList();
			if (!CanReject(idealPredicates, rejectedFeatures)) {
				throw new Exception("Ideal predicates can't reject elements.");
			}
		}

		public void LearnUntilBeStable(ICollection<string> allPaths, ICollection<string> seedPaths) {
			_previousActuals = new List<bool>();
			_isFirstTime = true;

			var seedAcceptedElements = new List<Tuple<XElement, int>>();
			var seedRejectedElements = new HashSet<XElement>();
			foreach (var path in seedPaths) {
				Console.WriteLine(path);
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile, null, true);
				seedRejectedElements.UnionWith(GetAllElements(ast));
				seedAcceptedElements.AddRange(GetAcceptedElements(ast));
			}
			seedRejectedElements.ExceptWith(seedAcceptedElements.Select(t => t.Item1));
			var groupedSeedAcceptedElements = seedAcceptedElements
					.GroupBy(t => t.Item2, t => t.Item1)
					.ToList();

			_masterPredicates = groupedSeedAcceptedElements
					.Select(g => g.GetCommonKeys(_predicateDepth))
					.Aggregate((ks1, ks2) => {
						ks1.UnionWith(ks2);
						return ks1;
					}).ToList();
			_featureCount = _masterPredicates.Count;

			_seedAccepted = groupedSeedAcceptedElements
					.Select(g => g.Select(e => GetBits(e, _masterPredicates))
							.Aggregate((f1, f2) => f1 & f2))
					.ToHashSet();
			_rejected = seedRejectedElements
					.Select(e => GetBits(e, _masterPredicates))
					.ToHashSet();
			_accepted = new List<BigInteger>();

			var acceptedFeaturesCounter = new CountingSet<BigInteger>();
			var allFeaturesCounter = new CountingSet<BigInteger>();
			foreach (var path in allPaths) {
				Console.WriteLine(path);
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				allFeaturesCounter.UnionWith(GetAllElements(ast)
						.Select(e => GetBits(e, _masterPredicates)));
				acceptedFeaturesCounter.UnionWith(GetAcceptedElements(ast)
						.Select(t => GetBits(t.Item1, _masterPredicates)));
			}

			var acceptedFeaturesCount = acceptedFeaturesCounter.Count;
			var allFeaturesCount = allFeaturesCounter.Count;
			// 初期の（＝最も厳しい）Predicateで除外できるものは無視
			acceptedFeaturesCounter.ClearItemsIf(f => !IsRejected(_seedAccepted, f));
			allFeaturesCounter.ClearItemsIf(f => !IsRejected(_seedAccepted, f));
			var alreadyAcceptedCount = acceptedFeaturesCount - acceptedFeaturesCounter.Count;
			if (alreadyAcceptedCount != allFeaturesCount - allFeaturesCounter.Count) {
				throw new Exception("Some excluded elements wrongly are accepted.");
			}
			Console.WriteLine("Already accepted: " + alreadyAcceptedCount);

			allFeaturesCounter.ExceptWith(acceptedFeaturesCounter);
			_acceptedFeatures = acceptedFeaturesCounter.ToHashSet();
			_rejectedFeatures = allFeaturesCounter.ToHashSet();
			if (acceptedFeaturesCounter.Count > 0) {
				Console.WriteLine("Accepted Max A: "
				                  + acceptedFeaturesCounter.ItemsWithCount.Max(kv => kv.Value) +
				                  ", Min A: " + acceptedFeaturesCounter.ItemsWithCount.Min(kv => kv.Value));
			}
			if (allFeaturesCounter.Count > 0) {
				Console.WriteLine("Rejected Max R: " + allFeaturesCounter.ItemsWithCount.Max(kv => kv.Value) +
				                  ", Min R: " + allFeaturesCounter.ItemsWithCount.Min(kv => kv.Value));
			}

			const int initialFeatureCount = 5;
			var initialMaxFeatures = GetInitialFeatures(allFeaturesCounter, initialFeatureCount, 1);
			var initialMinFeatures = GetInitialFeatures(allFeaturesCounter, initialFeatureCount, -1);
			foreach (var feature in initialMaxFeatures.Concat(initialMinFeatures)) {
				if (_rejectedFeatures.Contains(feature)) {
					//_rejected.Add(feature);
				} else {
					//_accepted.Add(feature);
				}
			}

			_rejectedFeatures.UnionWith(_rejected);
			var initialPredicates = _seedAccepted
					.Select(s => new Stack<BigInteger>(Enumerable.Repeat(s, 1)))
					.Select(GetPredicate)
					.ToList();
			if (!CanReject(initialPredicates, _rejectedFeatures)) {
				throw new Exception("Initial predicates can't reject elements.");
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
				_isFirstTime = false;
			}
			Console.WriteLine("Required elements: " + (_accepted.Count + _rejected.Count));
		}

		private List<BigInteger> GetInitialFeatures(
				CountingSet<BigInteger> allFeaturesCounter, int initialFeatureCount, int sign) {
			var initialMaxFeatures = allFeaturesCounter
					.ItemsWithCount.OrderByDescending(kv => kv.Value * sign)
					.Select(kv => kv.Key)
					.Where(f => !_seedAccepted.Contains(f) && !_rejected.Contains(f))
					.Take(initialFeatureCount)
					.ToList();
			return initialMaxFeatures;
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

		/// <summary>
		/// Reject最大化（小さい方から）
		/// </summary>
		/// <returns></returns>
		private List<BigInteger> LearnPredicatesFromMinMaxAccepted() {
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

		/// <summary>
		/// Reject最大化
		/// </summary>
		/// <returns></returns>
		private List<BigInteger> LearnPredicatesFromMaxMaxAccepted() {
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
				var preds1 = LearnPredicatesFromMaxMaxAccepted();
				var preds2 = LearnPredicatesFromMinMaxAccepted();
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
			return LearnPredicatesFromMaxMaxAccepted();
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
			var predicates = LearnPredicatesFromMinMaxAccepted();
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
			var difAccepted = new List<NextTarget>();
			var difRejected = new List<NextTarget>();

			var index = 0;

			foreach (var feature in _acceptedFeatures) {
				var differential = GetClassifierInput(feature, predicates, out minIndex, out diffPattern);
				var actual = differential == 0;
				if (_isFirstTime) {
					_previousActuals.Add(actual);
				} else if (_previousActuals[index] != actual) {
					_previousActuals[index] = actual;
					difAccepted.Add(new NextTarget {
						DiffPattern = diffPattern,
						Differential = differential,
						Feature = feature
					});
				}
				index++;
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
				if (_isFirstTime) {
					_previousActuals.Add(actual);
				} else if (_previousActuals[index] != actual) {
					_previousActuals[index] = actual;
					difRejected.Add(new NextTarget {
						DiffPattern = diffPattern,
						Differential = differential,
						Feature = feature
					});
				}
				index++;
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
			Console.WriteLine("DA: " + difAccepted.Count + ", DR: " + difRejected.Count);
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
			return nextAcceptedCount == 0;
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

		protected abstract IEnumerable<Tuple<XElement, int>> GetAcceptedElements(XElement ast);
	}
}