using System;
using System.Linq;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;

namespace Occf.Learner.Core.Tests {
	public class Id3Learner : LearningAlgorithm {
		public override string Description {
			get { return "ID3"; }
		}

		public override Func<double[], double> Learn(LearningData learningData) {
			var tree = new DecisionTree(learningData.Variables, 2);
			var learning = new ID3Learning(tree);
			learning.Run(
					learningData.Inputs.Select(ds => ds.Select(d => (int)d).ToArray()).ToArray(),
					learningData.Outputs);
			return record => tree.Compute(record);
		}
	}
}