using System;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;

namespace Occf.Learner.Core.Tests {
	public class SvmLearner : LearningAlgorithm {
		private readonly IKernel _kernel;

		public override string Description {
			get { return "SVM with " + _kernel; }
		}

		public SvmLearner() {
			_kernel = new Linear();
		}

		public SvmLearner(IKernel kernel) {
			_kernel = kernel;
		}

		public override Func<double[], double> Learn(LearningData learningData) {
			var svm = new KernelSupportVectorMachine(_kernel, learningData.Variables.Count);
			var smo = new SequentialMinimalOptimization(svm, learningData.Inputs, learningData.Outputs);
			smo.Run();
			return svm.Compute;
		}
	}
}