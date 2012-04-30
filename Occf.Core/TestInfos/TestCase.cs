#region License

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

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Code2Xml.Core.Position;

namespace Occf.Core.TestInfos {
	[Serializable]
	public class TestCase {
		public TestCase(string relativePath, string name, XElement node)
				: this(relativePath, name, CodePositions.New(node)) {}

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
			// These properties weren't initialized in inserting due to performance
			Statements = new HashSet<int>();
			Decisions = new HashSet<int>();
			Conditions = new HashSet<int>();
			ConditionDecisions = new HashSet<int>();
			StatementConditionDecisions = new HashSet<int>();
			Paths = new List<int>();
			// Assume all testcases are passed without testresult.txt
			Passed = true;
		}

		public override string ToString() {
			return Name;
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