using System.Linq;
using Code2Xml.Languages.Java.CodeToXmls;
using NUnit.Framework;

namespace Occf.Learner.Core.Tests {
	[TestFixture]
	internal class PropertyExtractorTest {
		[Test]
		public void TestElementSetExtractor() {
			var xml = JavaCodeToXml.Instance.Generate(@"
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
					Is.EqualTo(string.Join("/", new[] { "{", "blockStatement", "blockStatement", "blockStatement", "}" })));
			Assert.That(string.Join("/", new ElementSetExtractor(1).ExtractProperty(stmt)),
					Is.EqualTo(string.Join("/", new[] { "(", "expression", ")", "block" })));
			Assert.That(string.Join("/", new ElementSetExtractor(2).ExtractProperty(stmt)),
					Is.EqualTo(string.Join("/", new[] { "conditionalExpression", "{", "blockStatement", "}" })));
			Assert.That(new ElementSetExtractor(-99).ExtractProperty(stmt), Is.Empty);
			Assert.That(new ElementSetExtractor(99).ExtractProperty(stmt), Is.Empty);
		}
	}
}