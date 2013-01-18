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
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Occf.Core.CoverageInformation.Elements;
using Occf.Core.Utils;

namespace Occf.Core.CoverageInformation {
	[Serializable]
	public class CoverageInfo {
		public string BasePath;
		public string LanguageName;

		public List<Tuple<int, int>> BranchConditionRanges;
		public List<Tuple<int, int>> BranchRanges;
		public List<Tuple<int, int>> StatementRanges;
		public List<Tuple<int, int>> SwitchRanges;
		public List<CoverageElementGroup> TargetGroups;
		public List<CoverageElement> Targets;

		public CoverageInfo(
				string basePath, string languageName, SharingMethod sharingMethod) {
			SharingMethod = sharingMethod;
			BasePath = basePath;
			LanguageName = languageName;
			Targets = new List<CoverageElement>();
			TargetGroups = new List<CoverageElementGroup>();

			StatementRanges = new List<Tuple<int, int>>();
			BranchRanges = new List<Tuple<int, int>>();
			BranchConditionRanges = new List<Tuple<int, int>>();
			SwitchRanges = new List<Tuple<int, int>>();
		}

		public SharingMethod SharingMethod { get; private set; }

		public IEnumerable<Tuple<int, CoverageElement>> StatementIndexAndTargets {
			get { return StatementRanges.SelectMany(GetCoverageIndexAndElements); }
		}

		public IEnumerable<CoverageElement> StatementTargets {
			get { return StatementRanges.SelectMany(GetCoverageElements); }
		}

		public IEnumerable<CoverageElement> BranchTargets {
			get { return BranchRanges.SelectMany(GetCoverageElements); }
		}

		public IEnumerable<CoverageElementGroup> BranchConditionTargets {
			get { return BranchConditionRanges.SelectMany(GetCoverageElementGroups); }
		}

		public IEnumerable<CoverageElementGroup> SwitchTargets {
			get { return SwitchRanges.SelectMany(GetCoverageElementGroups); }
		}

		private IEnumerable<CoverageElement> GetCoverageElements(Tuple<int, int> r) {
			return Targets.Skip(r.Item1).Take(r.Item2 - r.Item1);
		}

		private IEnumerable<Tuple<int, CoverageElement>> GetCoverageIndexAndElements(
				Tuple<int, int> r) {
			for (var i = r.Item1; i < r.Item2; i++) {
				yield return Tuple.Create(i, Targets[i]);
			}
		}

		private IEnumerable<CoverageElementGroup> GetCoverageElementGroups(
				Tuple<int, int> r) {
			return TargetGroups.Skip(r.Item1).Take(r.Item2 - r.Item1);
		}

		public static CoverageInfo ReadCoverageInfo(FileInfo infoFile) {
			return ReadCoverageInfo(infoFile, new BinaryFormatter());
		}

		public static CoverageInfo ReadCoverageInfo(FileInfo infoFile, BinaryFormatter formatter) {
			using (var fs = new FileStream(infoFile.FullName, FileMode.Open)) {
				return (CoverageInfo)formatter.Deserialize(fs);
			}
		}

		public static void WriteCoverageInfo(DirectoryInfo rootDir, CoverageInfo covInfo) {
			WriteCoverageInfo(rootDir, covInfo, new BinaryFormatter());
		}

		public static void WriteCoverageInfo(
				DirectoryInfo rootDir, CoverageInfo covInfo, BinaryFormatter formatter) {
			var covPath = Path.Combine(rootDir.FullName, OccfNames.CoverageInfo);
			using (var fs = new FileStream(covPath, FileMode.Create)) {
				formatter.Serialize(fs, covInfo);
			}
		}
	}
}