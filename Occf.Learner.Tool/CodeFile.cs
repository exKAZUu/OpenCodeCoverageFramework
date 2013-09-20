using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.Location;
using Paraiba.Text;

namespace Occf.Learner.Tool {
	public class CodeFile {
		public bool ReadOnly { get; set; }

		public FileInfo Info { get; private set; }

		public string Code { get; private set; }

		public XElement Ast { get; private set; }

		public IDictionary<CodeRange, XElement> Range2Elements { get; set; }

		public CodeFile(CodeToXml codeToXml, FileInfo info) {
			Info = info;
			Code = GuessEncoding.ReadAllText(info.FullName);
			Ast = codeToXml.Generate(Code);
			Range2Elements = new Dictionary<CodeRange, XElement>();
		}
	}
}