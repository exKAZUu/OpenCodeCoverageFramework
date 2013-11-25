#region License

// Copyright (C) 2011-2013 Kazunori Sakamoto
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.IO;
using System.Linq;
using Accord.Statistics.Kernels;
using NUnit.Framework;

namespace Occf.Learner.Core.Tests {
	[TestFixture]
	public class ExtractingJavaBranchTest {
		[Test]
		public void TestPageObjectGenerator() {
			var allPaths = Directory.GetFiles(
					@"C:\Users\exKAZUu\Projects\PageObjectGenerator", "*.java",
					SearchOption.AllDirectories)
					.ToList();
			var seedPaths = new[] {
				@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\main\java\com\google\testing\pogen\GenerateCommand.java",
				@"C:\Users\exKAZUu\Projects\PageObjectGenerator\src\main\java\com\google\testing\pogen\generator\test\java\TestCodeGenerator.java",
			};

			var exp = new JavaBranchExperiment();
			var learner = new SvmLearner(new Linear());
			exp.LearnUntilBeStable(allPaths, seedPaths, learner, 0.5);
		}
	}
}