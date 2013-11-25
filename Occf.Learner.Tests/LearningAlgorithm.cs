using System;

namespace Occf.Learner.Core.Tests {
	public abstract class LearningAlgorithm {
		public abstract string Description { get; }

		public abstract Func<double[], double> Learn(LearningData learningData);
	}
}