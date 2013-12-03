using System;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;

namespace Occf.Learner.Core.Tests.LearningAlgorithms {
	public class C45Learner : LearningAlgorithm {
		private readonly LearningAlgorithm _other;

		public C45Learner(LearningAlgorithm other) {
			_other = other;
		}

		public override Func<double[], Tuple<bool, double>> Learn(LearningData learningData, out double error) {
			var tree = new DecisionTree(learningData.Variables, 2);
			var learning = new C45Learning(tree);
			error = learning.Run(learningData.Inputs, learningData.Outputs);
			var judge = _other.Learn(learningData, out error);
			return record => {
				var ret = judge(record);
				return Tuple.Create(tree.Compute(record) <= 0, ret.Item2);
			};
		}

		public override string ToString() {
			return "C45 with " + _other;
		}
	}
}