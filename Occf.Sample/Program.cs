#region License

// Copyright (C) 2011-2012 Kazunori Sakamoto
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
using System.Text.RegularExpressions;
using Occf.Tools.Core;

namespace Occf.Sample {
	internal class Program {
		private static void Main(string[] args) {
			var outDirInfo = new DirectoryInfo(@"C:\coverage");
			var inputDirInfo = new DirectoryInfo(
					@"..\..\..\fixture\project\input\GetMid");
			var excludeInDirInfo =
					new DirectoryInfo(@"..\..\..\fixture\project\input\GetMid\test");

			var instrumenter = new SampleInstrumenter(outDirInfo, inputDirInfo);
			var profile = ScriptCoverageProfile.Load("Java");
			var regex =
					new Regex(profile.FilePattern.Replace("*", ".*").Replace("?", "."));

			outDirInfo.Create();
			var fileInfos = inputDirInfo.EnumerateFiles("*", SearchOption.AllDirectories);
			foreach (var fileInfo in fileInfos) {
				if (regex.IsMatch(fileInfo.FullName)) {
					if (!fileInfo.FullName.StartsWith(excludeInDirInfo.FullName)) {
						instrumenter.WriteInstrumentedProductionCode(profile, fileInfo);
					} else {
						instrumenter.WriteInstrumentedTestCode(profile, fileInfo);
					}
				} else {
					instrumenter.CopyFile(fileInfo);
				}
			}
			instrumenter.CopyLibraries(profile);
		}
	}
}