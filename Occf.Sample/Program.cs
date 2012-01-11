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
using Paraiba.IO;

namespace Occf.Sample {
    internal class Program {
        private static void Main(string[] args) {
            var outDir = @"C:\coverage";
            var inputDir = @"..\..\..\fixture\project\input\GetMid";
            var excludeInDir = @"..\..\..\fixture\project\input\GetMid\test";

            outDir = Path.GetFullPath(outDir);
            inputDir = Path.GetFullPath(inputDir);
            excludeInDir = Path.GetFullPath(excludeInDir);

            var instrumenter = new SampleInstrumenter(outDir, inputDir);
            var profile = ScriptCoverageProfile.Load("Java");
            var regex =
                    new Regex(
                            profile.FilePattern.Replace("*", ".*").Replace(
                                    "?", "."));

            Directory.CreateDirectory(outDir);
            var filePaths = Directory.EnumerateFiles(
                    inputDir, "*", SearchOption.AllDirectories);
            foreach (var filePath in filePaths) {
                var relativePath = XPath.GetRelativePath(filePath, inputDir);
                if (regex.IsMatch(filePath)) {
                    if (!filePath.StartsWith(excludeInDir)) {
                        instrumenter.WriteInstrumentedProductionCode(
                                profile, relativePath);
                    } else {
                        instrumenter.WriteInstrumentedTestCode(
                                profile, relativePath);
                    }
                } else {
                    instrumenter.CopyFile(relativePath);
                }
            }
            instrumenter.CopyLibraries(profile);
        }
    }
}