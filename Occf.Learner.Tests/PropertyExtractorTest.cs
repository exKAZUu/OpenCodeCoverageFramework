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
			Assert.That(new ElementSetExtractor(0).ExtractProperty(stmt),
					Is.EqualTo(new[] { "IF", "parExpression", "statement" }));
			Assert.That(new ElementSetExtractor(-1).ExtractProperty(stmt),
					Is.EqualTo(new[] { "statement" }));
			Assert.That(new ElementSetExtractor(-2).ExtractProperty(stmt),
					Is.EqualTo(new[] { "LBRACE", "blockStatement", "blockStatement", "blockStatement", "RBRACE" }));
			Assert.That(new ElementSetExtractor(1).ExtractProperty(stmt),
					Is.EqualTo(new[] { "LPAREN", "expression", "RPAREN", "block" }));
			Assert.That(new ElementSetExtractor(2).ExtractProperty(stmt),
					Is.EqualTo(new[] { "conditionalExpression", "LBRACE", "blockStatement", "RBRACE" }));
			Assert.That(new ElementSetExtractor(-99).ExtractProperty(stmt), Is.Empty);
			Assert.That(new ElementSetExtractor(99).ExtractProperty(stmt), Is.Empty);
		}
	}
}