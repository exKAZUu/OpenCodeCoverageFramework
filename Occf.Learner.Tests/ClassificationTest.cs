using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using Paraiba.Linq;

namespace Occf.Learner.Core.Tests {
	[TestFixture]
	public class ClassificationTest {
		[Test]
		public void Classify() {
			var accepted = new List<BigInteger> {
				(1 << 4) + (1 << 3) + (1 << 2) + (0 << 1) + (1 << 0),
				(1 << 4) + (0 << 3) + (0 << 2) + (1 << 1) + (1 << 0),
				(1 << 4) + (1 << 3) + (0 << 2) + (0 << 1) + (1 << 0),
				(0 << 4) + (1 << 3) + (1 << 2) + (1 << 1) + (0 << 0),
				(1 << 4) + (1 << 3) + (1 << 2) + (1 << 1) + (0 << 0)
			};
			var seeds = new List<BigInteger> {
				(1 << 4) + (0 << 3) + (0 << 2) + (1 << 1) + (1 << 0),
				(1 << 4) + (0 << 3) + (1 << 2) + (1 << 1) + (1 << 0),
			};
			var rejected = new List<BigInteger> {
				(0 << 4) + (1 << 3) + (1 << 2) + (0 << 1) + (1 << 0),
				(1 << 4) + (0 << 3) + (0 << 2) + (1 << 1) + (0 << 0),
				(1 << 4) + (1 << 3) + (1 << 2) + (0 << 1) + (0 << 0),
				(0 << 4) + (1 << 3) + (0 << 2) + (1 << 1) + (1 << 0)
			};
			var learningSet = seeds
					.Select(s => new Stack<BigInteger>(Enumerable.Repeat(s, 1)))
					.ToList();

			var histories = new Stack<int>();
			for (int i = 0; i < accepted.Count; i++) {
				for (int j = 0;; j++) {
					learningSet[j].Push(accepted[i]);
					var predicates = learningSet.Select(GetPredicate).ToList();
					if (CanReject(predicates, rejected)) {
						histories.Push(j);
						break;
					}
					learningSet[j].Pop();
					while (j == learningSet.Count - 1) {
						if (histories.IsEmpty()) {
							throw new Exception("Failed to learn");
						}
						i--;
						j = histories.Pop();
						learningSet[j].Pop();
					}
				}
			}

			{
				var predicates = learningSet.Select(GetPredicate).ToList();
				foreach (var predicate in predicates) {
					Console.WriteLine(PredicateGenerator.BigIntegerToString(predicate, 100));
				}
			}
		}

		private BigInteger GetPredicate(IEnumerable<BigInteger> properties) {
			return properties.Aggregate((p1, p2) => p1 & p2);
		}

		private bool CanReject(IEnumerable<BigInteger> predicates, IEnumerable<BigInteger> rejected) {
			return predicates.All(p => rejected.All(r => (p & r) != p));
		}
	}
}