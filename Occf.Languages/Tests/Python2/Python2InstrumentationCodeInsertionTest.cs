﻿#region License

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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Occf.Core.Manipulators;

namespace Occf.Languages.Tests.Python2 {
    public class Python2InstrumentationCodeInsertionTest {
        private static IEnumerable<TestCaseData> TestCases {
            get {
                var names = new[] {
                        "Block1.py",
                        "Block2.py",
                        "Block3.py",
                };
                return names.Select(name => new TestCaseData(name));
            }
        }

        [Test, TestCaseSource("TestCases")]
        public void VerifyInstrumentationCode(string fileName) {
            var profile = LanguageSupports.GetCoverageModeByClassName("Python2");
            CodeInsertTest.VerifyCodeInsertion(profile, fileName);
        }

        [Test, TestCaseSource("TestCases")]
        public void InsertInstrumentationCode(string fileName) {
            var profile = LanguageSupports.GetCoverageModeByClassName("Python2");
            CodeInsertTest.InsertInstrumentationCode(profile, fileName);
        }
    }
}