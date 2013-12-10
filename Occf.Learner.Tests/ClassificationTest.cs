using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Occf.Learner.Core.Tests {
	[TestFixture]
	public class ClassificationTest {
		[Test]
		public void Classify() {
			var accepted = new List<int> {
				(1 << 4) + (1 << 3) + (1 << 2) + (0 << 2) + (1 << 0),
				(1 << 4) + (0 << 3) + (0 << 2) + (1 << 2) + (1 << 0),
				(1 << 4) + (1 << 3) + (0 << 2) + (0 << 2) + (1 << 0),
				(0 << 4) + (1 << 3) + (1 << 2) + (1 << 2) + (0 << 0),
				(1 << 4) + (1 << 3) + (1 << 2) + (1 << 2) + (0 << 0)
			};
			var seeds = new List<int> {
				(1 << 4) + (0 << 3) + (0 << 2) + (1 << 2) + (1 << 0),
				(1 << 4) + (0 << 3) + (1 << 2) + (1 << 2) + (1 << 0),
			};
			var rejected = new List<int> {
				(0 << 4) + (1 << 3) + (1 << 2) + (0 << 2) + (1 << 0),
				(1 << 4) + (0 << 3) + (0 << 2) + (1 << 2) + (0 << 0),
				(1 << 4) + (1 << 3) + (1 << 2) + (0 << 2) + (0 << 0),
				(0 << 4) + (1 << 3) + (0 << 2) + (1 << 2) + (1 << 0)
			};
			var learningSet = seeds.Select(s => new List<int> { s }).ToList();

			for (int i = 0; i < accepted.Count; i++) {
				var predicates = learningSet.Select(GetPredicate).ToList();
				for (int j = 0; j < learningSet.Count; j++) {
					learningSet[j].Add(accepted[i]);
					if (CanClassify(predicates, rejected)) {
						break;
					}
					learningSet[j].RemoveAt(learningSet[j].Count - 1);
				}
			}

			{
				var predicates = learningSet.Select(GetPredicate).ToList();
				foreach (var predicate in predicates) {
					Console.WriteLine(Convert.ToString(predicate, 2));
				}
			}
		}

		private int GetPredicate(IEnumerable<int> properties) {
			return properties.Aggregate((p1, p2) => p1 & p2);
		}

		private bool CanClassify(IEnumerable<int> predicates, IEnumerable<int> rejected) {
			return predicates.All(p => rejected.All(r => (p & r) == p));
		}
	}
}