using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;
using Code2Xml.Core;
using Paraiba.Linq;

namespace Occf.Learner.Core.Tests {
	public abstract class BitLearningExperiment {
		public int WrongCount { get; set; }
		protected abstract Processor Processor { get; }
		private readonly ISet<string> _elementNames;
		private Dictionary<string, HashSet<BigInteger>> _acceptedFeatureDict;
		private Dictionary<string, HashSet<BigInteger>> _rejectedFeatureDict;
		private HashSet<BigInteger> _seedAccepted;
		private HashSet<BigInteger> _seedRejected;
		private HashSet<BigInteger> _learnedDiffs;
		private int _predicateDepth = 10;
		private List<string> _masterPredicates;
		private int _featureCount;
		private const int LearningCount = 20;

		protected BitLearningExperiment(params string[] elementNames) {
			_elementNames = elementNames.ToHashSet();
		}

		public void LearnUntilBeStable(
				ICollection<string> allPaths, ICollection<string> seedPaths) {
			_learnedDiffs = new HashSet<BigInteger>();

			var seedAccepted = new HashSet<XElement>();
			var seedRejected = new HashSet<XElement>();
			foreach (var path in seedPaths) {
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				seedRejected.UnionWith(GetAllElements(ast).ToHashSet());
				seedAccepted.UnionWith(GetAcceptedElements(ast).Select(t => t.Item1));
			}
			seedRejected.ExceptWith(seedAccepted);
			_masterPredicates = seedAccepted.GetUnionKeys(_predicateDepth).ToList();
			_featureCount = _masterPredicates.Count;
			_seedAccepted = seedAccepted.Select(e => GetBits(e, _masterPredicates)).ToHashSet();
			_seedRejected = seedRejected.Select(e => GetBits(e, _masterPredicates)).ToHashSet();

			_acceptedFeatureDict = new Dictionary<string, HashSet<BigInteger>>();
			_rejectedFeatureDict = new Dictionary<string, HashSet<BigInteger>>();
			foreach (var path in allPaths) {
				var codeFile = new FileInfo(path);
				var ast = Processor.GenerateXml(codeFile);
				var rejectedElements = GetAllElements(ast).ToHashSet();
				var acceptedElements = GetAcceptedElements(ast).Select(t => t.Item1).ToHashSet();
				rejectedElements.ExceptWith(acceptedElements);
				_acceptedFeatureDict.Add(path, acceptedElements
						.Select(e => GetBits(e, _masterPredicates))
						.ToHashSet());
				_rejectedFeatureDict.Add(path, rejectedElements
						.Select(e => GetBits(e, _masterPredicates))
						.ToHashSet());
			}

			while (true) {
				var time = Environment.TickCount;
				if (LearnAndApply()) {
					break;
				}
				Console.WriteLine("Time: " + (Environment.TickCount - time));
			}
			Console.WriteLine("Required elements: " + (_seedAccepted.Count + _seedRejected.Count));
		}

		private bool CanReject(IEnumerable<BigInteger> predicates, IEnumerable<BigInteger> rejected) {
			return rejected.All(r => predicates.All(p => (r & p) != p));
		}

		private BigInteger GetCommonPredicate(IEnumerable<BigInteger> features) {
			return features.Aggregate((v1, v2) => v1 & v2);
		}

		private BigInteger LearnCommonKeys() {
			var predicate = GetCommonPredicate(_seedAccepted);
			if (!CanReject(new[] { predicate }, _seedRejected)) {
				throw new Exception("Failed to learn rules.");
			}
			return predicate;
		}

		private bool LearnAndApply() {
			var predicate = LearnCommonKeys();
			BigInteger diffs;

			var correctlyAccepted = 0;
			var correctlyRejected = 0;
			var wronglyAccepted = 0;
			var wronglyRejected = 0;
			var nextElements = new List<Tuple<int, BigInteger, string, bool, BigInteger>>();

			foreach (var kv in _acceptedFeatureDict) {
				var path = kv.Key;
				var features = kv.Value;
				foreach (var feature in features) {
					var diffCount = GetClassifierInput(feature, predicate, out diffs);
					var actual = diffCount == 0;
					if (actual) {
						correctlyAccepted++;
					} else {
						if (wronglyRejected == 0) {
							//Console.WriteLine("WR (" + prob + "): " + e.SafeParent().SafeParent().SafeParent());
						}
						wronglyRejected++;
					}
					UpdateNextElements(diffCount, diffs, nextElements, feature, path, true);
				}
				Console.Write(".");
			}

			foreach (var kv in _rejectedFeatureDict) {
				var path = kv.Key;
				var features = kv.Value;
				foreach (var feature in features) {
					var diffCount = GetClassifierInput(feature, predicate, out diffs);
					var actual = diffCount == 0;
					if (actual) {
						if (wronglyAccepted == 0) {
							//Console.WriteLine("WA (" + prob + "): " + e.SafeParent().SafeParent().SafeParent());
						}
						wronglyAccepted++;
					} else {
						correctlyRejected++;
					}
					UpdateNextElements(diffCount, diffs, nextElements, feature, path, false);
				}
				Console.Write(".");
			}
			Console.WriteLine("done");
			Console.WriteLine("WA: " + wronglyAccepted + ", WR: " + wronglyRejected + ", CA: "
			                  + correctlyAccepted + ", CR: " + correctlyRejected + ", L: "
			                  + (_seedAccepted.Count + _seedRejected.Count) + ", P: " + CountBits(predicate));
			Console.WriteLine("Accepted: " + _seedAccepted.Count
			                  + " / " + _acceptedFeatureDict.Sum(kv => kv.Value.Count));
			Console.WriteLine("Rejected: " + _seedRejected.Count
			                  + " / " + _rejectedFeatureDict.Sum(kv => kv.Value.Count));
			WrongCount = wronglyAccepted + wronglyRejected;
			if (nextElements.Count > 0) {
				Console.WriteLine("Diff: " + nextElements[0].Item1);
				foreach (var t in nextElements) {
					if (t.Item4) {
						_seedAccepted.Add(t.Item2);
					} else {
						_seedRejected.Add(t.Item2);
					}
				}
			}
			return nextElements.Count(t => t.Item4) == 0;
		}

		private void UpdateNextElements(
				int count, BigInteger diffs, List<Tuple<int, BigInteger, string, bool, BigInteger>> nextElements,
				BigInteger feature, string path, bool accepted) {
			if (count > 0) {
				if (!_learnedDiffs.Contains(diffs)) {
					_learnedDiffs.Add(diffs);
					nextElements.Add(Tuple.Create(count, feature, path, accepted, diffs));
					nextElements.Sort((t1, t2) => t1.Item1.CompareTo(t2.Item1));
					if (nextElements.Count > LearningCount) {
						nextElements.RemoveAt(LearningCount);
					}
				}
			}
		}

		private int GetClassifierInput(BigInteger target, BigInteger predicate, out BigInteger diffs) {
			diffs = (target & predicate) ^ predicate;
			return CountBits(diffs);
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