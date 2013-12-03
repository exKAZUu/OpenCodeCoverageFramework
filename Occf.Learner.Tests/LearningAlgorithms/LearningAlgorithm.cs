using System;

namespace Occf.Learner.Core.Tests.LearningAlgorithms {
	public abstract class LearningAlgorithm {
		public abstract Func<double[], Tuple<bool, double>> Learn(LearningData learningData, out double error);
	}
}