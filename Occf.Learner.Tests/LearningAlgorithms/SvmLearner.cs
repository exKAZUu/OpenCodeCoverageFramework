using System;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;

namespace Occf.Learner.Core.Tests.LearningAlgorithms {
	public class SvmLearner : LearningAlgorithm {
		private readonly IKernel _kernel;

		public SvmLearner() {
			_kernel = new Linear();
		}

		public SvmLearner(IKernel kernel) {
			_kernel = kernel;
		}

		public override Func<double[], Tuple<bool, double>> Learn(LearningData learningData, out double error) {
			var svm = new KernelSupportVectorMachine(_kernel, learningData.Variables.Count);
			var smo = new SequentialMinimalOptimization(svm, learningData.Inputs, learningData.Outputs);
			error = smo.Run();
			return record => {
				var ret = svm.Compute(record);
				return Tuple.Create(ret <= 0, ret);
			};
		}

		public override string ToString() {
			return "SVM with " + _kernel;
		}
	}
}