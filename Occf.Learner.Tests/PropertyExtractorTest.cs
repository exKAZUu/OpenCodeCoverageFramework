using System.Linq;
using Code2Xml.Core;
using NUnit.Framework;

namespace Occf.Learner.Core.Tests {
	[TestFixture]
	internal class PropertyExtractorTest {
		[Test]
		public void TestElementSetExtractor() {
			var xml = ProcessorLoader.JavaUsingAntlr3.GenerateXml(@"
public abstract class Command {
  public File createFileFromFilePath(String filePath) throws FileNotFoundException,
      FileProcessException {
    File file = new File(filePath);
    if (!file.isFile()) {
      throw new FileProcessException(null, file);
    }
    return file;
  }
}");
			var stmt = xml.Descendants("statement").First();
			Assert.That(string.Join("/", new ElementSetExtractor(0).ExtractProperty(stmt)),
					Is.EqualTo(string.Join("/", new[] { "if", "parExpression", "statement" })));
			Assert.That(string.Join("/", new ElementSetExtractor(-1).ExtractProperty(stmt)),
					Is.EqualTo(string.Join("/", new[] { "statement" })));
			Assert.That(string.Join("/", new ElementSetExtractor(-2).ExtractProperty(stmt)),
					Is.EqualTo(string.Join("/",
							new[] { "{", "blockStatement", "blockStatement", "blockStatement", "}" })));
			Assert.That(string.Join("/", new ElementSetExtractor(1).ExtractProperty(stmt)),
					Is.EqualTo(string.Join("/", new[] { "(", "expression", ")", "block" })));
			Assert.That(string.Join("/", new ElementSetExtractor(2).ExtractProperty(stmt)),
					Is.EqualTo(string.Join("/", new[] { "conditionalExpression", "{", "blockStatement", "}" })));
			Assert.That(new ElementSetExtractor(-99).ExtractProperty(stmt), Is.Empty);
			Assert.That(new ElementSetExtractor(99).ExtractProperty(stmt), Is.Empty);
		}

		[Test]
		public void TestElementSetExtractor2() {
			var xml = ProcessorLoader.JavaScriptUsingAntlr3.GenerateXml(@"
if (true == true) { }
");
			var exp = xml.Descendants("expression").First(e => e.TokenText() == "true==true");
			var t0 = new ElementSequenceExtractorRemovingLoneElements(0).ExtractProperty(exp);
			var t1 = new ElementSequenceExtractorRemovingLoneElements(-1).ExtractProperty(exp);
			var t2 = new ElementSequenceExtractorRemovingLoneElements(-2).ExtractProperty(exp);
			var t3 = new ElementSequenceExtractorRemovingLoneElements(-3).ExtractProperty(exp);
			var t4 = new ElementSequenceExtractorRemovingLoneElements(-4).ExtractProperty(exp);

			Assert.That(string.Join("/", new ElementSequenceExtractorRemovingLoneElements(0).ExtractProperty(exp)),
					Is.EqualTo(string.Join("/", new[] { "if", "parExpression", "statement" })));
			Assert.That(string.Join("/", new ElementSetExtractor(-1).ExtractProperty(exp)),
					Is.EqualTo(string.Join("/", new[] { "statement" })));
			Assert.That(string.Join("/", new ElementSetExtractor(-2).ExtractProperty(exp)),
					Is.EqualTo(string.Join("/",
							new[] { "{", "blockStatement", "blockStatement", "blockStatement", "}" })));
			Assert.That(string.Join("/", new ElementSetExtractor(1).ExtractProperty(exp)),
					Is.EqualTo(string.Join("/", new[] { "(", "expression", ")", "block" })));
			Assert.That(string.Join("/", new ElementSetExtractor(2).ExtractProperty(exp)),
					Is.EqualTo(string.Join("/", new[] { "conditionalExpression", "{", "blockStatement", "}" })));
			Assert.That(new ElementSetExtractor(-99).ExtractProperty(exp), Is.Empty);
			Assert.That(new ElementSetExtractor(99).ExtractProperty(exp), Is.Empty);
		}
	}
}