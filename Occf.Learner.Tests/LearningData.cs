using System.Collections.Generic;
using Accord.MachineLearning.DecisionTrees;

namespace Occf.Learner.Core.Tests {
	public class LearningData {
		public IList<DecisionVariable> Variables { get; set; }
		public IList<ElementSequenceExtractor> Extractors { get; set; }
		public double[][] Inputs { get; set; }
		public int[] Outputs { get; set; }
		public Dictionary<string, int> Prop2Index { get; set; }
	}
}