using System;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;

namespace Occf.Learner.Core.Tests {
	public class C45Learner : LearningAlgorithm {
		public override string Description {
			get { return "C45"; }
		}

		public override Func<double[], double> Learn(LearningData learningData) {
			var tree = new DecisionTree(learningData.Variables, 2);
			var learning = new C45Learning(tree);
			learning.Run(learningData.Inputs, learningData.Outputs);
			return record => tree.Compute(record);
		}
	}
}