#region License

// Copyright (C) 2009-2012 Kazunori Sakamoto
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
using NUnit.Framework;
using Occf.Core.CoverageCode;
using Occf.Core.CoverageInformation;
using Occf.Core.Profiles;
using Occf.Core.Tests;

namespace Occf.Languages.Tests {
    public static class CodeInsertTest {
        public static void VerifyCodeInsertion(
                CoverageProfile profile, string fileName) {
            var info = new CoverageInfo(
                    Fixture.GetCoverageInputPath(),
                    profile.Name, SharingMethod.SharedMemory);
            string inPath = Path.Combine(
                    Fixture.GetCoverageInputPath(), fileName);
            string code = CoverageCodeGenerator.GetCoveragedCode(
                    new FileInfo(inPath), info,
                    profile);

            string expPath = Path.Combine(
                    Fixture.GetCoverageExpectationPath(), fileName);
            try {
                using (var reader = new StreamReader(expPath)) {
                    string expected = reader.ReadToEnd();
                    Assert.That(code, Is.EqualTo(expected));
                }
            } catch {
                string path = Fixture.GetOutputPath(fileName);
                File.WriteAllText(path, code);
                throw;
            }
        }
    }
}