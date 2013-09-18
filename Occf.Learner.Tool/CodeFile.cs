using System.Collections.Generic;
using System.Xml.Linq;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.Location;
using Paraiba.Text;

namespace Occf.Learner.Tool {
	public class CodeFile {
		public string Code { get; private set; }

		public XElement Ast { get; private set; }

		public IDictionary<CodeRange, XElement> Range2Elements { get; set; }

		public CodeFile(CodeToXml codeToXml, string path) {
			Code = GuessEncoding.ReadAllText(path);
			Ast = codeToXml.Generate(Code);
			Range2Elements = new Dictionary<CodeRange, XElement>();
		}
	}
}