using System.Collections.Generic;
using Accord.MachineLearning.DecisionTrees;

namespace Occf.Learner.Core.Tests {
	public class LearningData {
		public IList<DecisionVariable> Variables { get; set; }
		public Dictionary<int, HashSet<Predicate>> Depth2Predicates { get; set; }
		public double[][] Inputs { get; set; }
		public int[] Outputs { get; set; }
	}
}