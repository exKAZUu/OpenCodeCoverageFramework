using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.Location;
using Occf.Learner.Core;
using Paraiba.Linq;
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

		public bool RangesEquals(Dictionary<CodeRange, XElement> range2Elements) {
			return Range2Elements.Values.ToHashSet().SetEquals(range2Elements.Values.ToHashSet());
		}

		public bool RangesEquals(string elementName, Dictionary<CodeRange, XElement> range2Elements) {
			return Range2Elements.Values.Where(e => e.Name.LocalName == elementName).ToHashSet()
				.SetEquals(range2Elements.Values.Where(e => e.Name.LocalName == elementName).ToHashSet());
		}
	}
}