using System;
using System.Collections.Generic;
using System.Linq;

namespace Occf.Core.CoverageInfos {
	[Serializable]
	public class CoverageInfo {
		public string BasePath;
		public string LanguageName;

		public List<Tuple<int, int>> BranchConditionRanges;
		public List<Tuple<int, int>> BranchRanges;
		public List<Tuple<int, int>> StatementRanges;
		public List<Tuple<int, int>> SwitchRanges;
		public List<CoverageElementGroup> TargetGroupList;
		public List<CoverageElement> TargetList;

		public CoverageInfo(string basePath, string languageName, SharingMethod sharingMethod) {
			SharingMethod = sharingMethod;
			BasePath = basePath;
			LanguageName = languageName;
			TargetList = new List<CoverageElement>();
			TargetGroupList = new List<CoverageElementGroup>();

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
			return TargetList.Skip(r.Item1).Take(r.Item2 - r.Item1);
		}

		private IEnumerable<Tuple<int, CoverageElement>> GetCoverageIndexAndElements(
			Tuple<int, int> r) {
			for (var i = r.Item1; i < r.Item2; i++) {
				yield return Tuple.Create(i, TargetList[i]);
			}
		}

		private IEnumerable<CoverageElementGroup> GetCoverageElementGroups(
			Tuple<int, int> r) {
			return TargetGroupList.Skip(r.Item1).Take(r.Item2 - r.Item1);
		}
	}
}