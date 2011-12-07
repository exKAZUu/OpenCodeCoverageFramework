using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Occf.Core.CodeInformations;

namespace Occf.Core.TestInfos {
	[Serializable]
	public class TestCase {
		public TestCase(string relativePath, string name, XElement node)
			: this(relativePath, name, CodePositionFactory.Create(node)) {}

		private TestCase(string relativePath, string name, CodePosition pos) {
			RelativePath = relativePath;
			Name = name;
			Position = pos;
		}

		public bool Passed { get; set; }
		public string RelativePath { get; private set; }
		public string Name { get; private set; }
		public CodePosition Position { get; private set; }

		public HashSet<int> Statements { get; private set; }
		public HashSet<int> Decisions { get; private set; }
		public HashSet<int> Conditions { get; private set; }
		public HashSet<int> ConditionDecisions { get; private set; }
		public HashSet<int> StatementConditionDecisions { get; private set; }
		public List<int> Paths { get; private set; }

		public void InitializeForStoringData() {
			Statements = new HashSet<int>();
			Decisions = new HashSet<int>();
			Conditions = new HashSet<int>();
			ConditionDecisions = new HashSet<int>();
			StatementConditionDecisions = new HashSet<int>();
			Paths = new List<int>();
		}

		//		public static TestCase Reader(BinaryReader reader) {
		//			var filePath = reader.ReadString();
		//			var name = reader.ReadString();
		//			var pos = CodePosition.Read(reader);
		//			return new TestCase(filePath, name, pos);
		//		}
		//
		//		public void Write(BinaryWriter writer) {
		//			writer.Write(FilePath);
		//			writer.Write(Name);
		//			Position.Write(writer);
		//		}
	}
}