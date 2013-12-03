using System;
using System.Linq;
using Accord.MachineLearning.Bayes;

namespace Occf.Learner.Core.Tests.LearningAlgorithms {
	public class NaiveBayesLearner : LearningAlgorithm {
		public override Func<double[], Tuple<bool, double>> Learn(LearningData learningData, out double error) {
			var naiveBayes = new NaiveBayes(2, learningData.Variables.Select(v => 2).ToArray());
			error = naiveBayes.Estimate(learningData.Inputs.Select(ds => ds.Select(d => (int)d).ToArray()).ToArray(),
					learningData.Outputs);
			return record => {
				double logLikelihood;
				double[] responses;
				var ret = naiveBayes.Compute(record.Select(d => (int)d).ToArray(), out logLikelihood,
						out responses);
				return Tuple.Create(ret <= 0, logLikelihood);
			};
		}

		public override string ToString() {
			return "NaiveBayes";
		}
	}
}