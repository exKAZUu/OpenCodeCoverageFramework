using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Occf.Core.Manipulators;
using Occf.Core.Tests;

namespace Occf.Learner.Tests {
    [TestFixture]
    public class FindLearnerTest {
        private static IEnumerable<TestCaseData> TestCases {
            get {
                var names = new[] {
                        "mul_mv.c",
                        "mul_mv2.c",
                        "mersenne.c",
                        "multi.h",
                        "bubblesort.c",
                        "quicksort.c",
                        "block1.c",
                        "block2.c",
                        "block3.c",
                        "get_sign.c",
                        "uint4.c",
                };
                return names.Select(name => new TestCaseData(name));
            }
        }


        private static IEnumerable<TestCaseData> JavaTestCases {
            get {
                var names = new[] {
                        "Block1.java",
                        "Block2.java",
                        "Block3.java",
                        "Condition.java",
                        "Simple.java",
                };
                return names.Select(name => new TestCaseData(name));
            }
        }

        [Test, TestCaseSource("TestCases")]
        public void LearnC(string fileName) {
            var profile = LanguageSupports.GetCoverageModeByClassName("C");
            var inPath = Path.Combine(Fixture.GetCoverageInputPath(), fileName);
            var codeFile = new FileInfo(inPath);
            var ast = profile.CodeToXml.GenerateFromFile(codeFile.FullName);
            var statements = profile.AstAnalyzer.FindStatements(ast).ToList();
            FindLearner.Learn(ast, statements);
            //var statements2 = rule.Find(ast).ToList();
            //Assert.That(statements2.Count, Is.EqualTo(statements.Count));
            //Assert.That(statements2, Is.SubsetOf(statements));
        }

        [Test, TestCaseSource("JavaTestCases")]
        public void LearnJava(string fileName) {
            var profile = LanguageSupports.GetCoverageModeByClassName("Java");
            var inPath = Path.Combine(Fixture.GetCoverageInputPath(), fileName);
            var codeFile = new FileInfo(inPath);
            var ast = profile.CodeToXml.GenerateFromFile(codeFile.FullName);
            var statements = profile.AstAnalyzer.FindStatements(ast).ToList();
            FindLearner.Learn(ast, statements);
            //var statements2 = rule.Find(ast).ToList();
            //Assert.That(statements2.Count, Is.EqualTo(statements.Count));
            //Assert.That(statements2, Is.SubsetOf(statements));
        }
    }
}