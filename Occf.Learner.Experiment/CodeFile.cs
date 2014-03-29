using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Code2Xml.Core;
using Code2Xml.Core.Location;
using Code2Xml.Core.Processors;
using Paraiba.Linq;
using Paraiba.Text;

namespace Occf.Learner.Experiment {
	public class CodeFile {
		public bool ReadOnly { get; set; }

		public FileInfo Info { get; private set; }

		public string Code { get; private set; }

		public XElement Ast { get; private set; }

		public IDictionary<CodeRange, XElement> Range2Elements { get; set; }

		public CodeFile(CstGenerator cstGenerator, FileInfo info) {
			Info = info;
			Code = GuessEncoding.ReadAllText(info.FullName);
			Ast = cstGenerator.GenerateTree(Code);
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