using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using NDesk.Options;
using Occf.Core.CoverageInfos;
using Occf.Tools.Core;
using Paraiba.Linq;

namespace Occf.Tools.Cui {
	public class CoverageDisplay {
		private const string S = "  ";
		private const int W = 12;

		private static readonly string Usage =
				"Occf 1.0.0" + "\n" +
				"Copyright (C) 2011 Kazunori SAKAMOTO" + "\n" +
				"" + "\n" +
				"Usage: Occf cov[erage] <root> [<coverage>]" + "\n" +
				"" + "\n" +
				S + "<root>".PadRight(W)
				+ "path of root directory (including source and test code)" + "\n" +
				S + "<coverage>".PadRight(W) + "path of coverage data whose name is "
				+ Names.CoverageData + "\n" +
				S + "-d, -detail <name>".PadRight(W)
				+
				"show all not executed elements."
				+ "\n" +
				"";

		public static bool Run(IList<string> args) {
			var detail = false;
			// parse options
			var p = new OptionSet {
					{ "d|detail", v => detail = v != null },
			};
			try {
				args = p.Parse(args);
			} catch {
				return Program.Print(Usage);
			}

			if (args.Count < 1)
				return Program.Print(Usage);

			var iArgs = 0;
			var rootPath = args[iArgs++];
			if (!Directory.Exists(rootPath)) {
				return Program.Print("root directory doesn't exist.\nroot:" + rootPath);
			}
			rootPath = Path.GetFullPath(rootPath);

			var covPath = args.Count >= iArgs + 1 ? args[iArgs++] : null;
			if (!File.Exists(covPath)) {
				covPath = PathFinder.FindCoverageDataPath(covPath);
			}
			if (!File.Exists(covPath)) {
				covPath = PathFinder.FindCoverageDataPath(rootPath);
			}
			if (!File.Exists(covPath)) {
				return
						Program.Print("coverage data file doesn't exist.\ncoverage:" + covPath);
			}
			covPath = Path.GetFullPath(covPath);

			return Analyze(rootPath, covPath, detail);
		}

		private static bool Analyze(string rootPath, string covPath, bool detail) {
			// カバレッジ情報（母数）の取得
			var formatter = new BinaryFormatter();
			var covInfoPath = PathFinder.FindCoverageInfoPath(rootPath);
			var covInfo = InfoReader.ReadCoverageInfo(covInfoPath, formatter);
			CoverageDataReader.ReadFile(covInfo, covPath);

			var tags = ReconstructTags(covInfo);
			foreach (var tag in tags/*.Where(t => !t.Contains("class "))*/) {
				Console.WriteLine(tag);
				{
					var checkedLine = new HashSet<Tuple<string, int>>();
					var executedAndNot = covInfo.StatementTargets
							.Where(e => e.Tag.StartsWith(tag))
							.Where(
									e =>
									checkedLine.Add(Tuple.Create(e.RelativePath, e.Position.StartLine)))
							.Halve(e => e.State == CoverageState.Done);
					var nExe = executedAndNot.Item1.Count;
					var nAll = nExe + executedAndNot.Item2.Count;
					Console.WriteLine(
							"Line Coverage: " + nExe * (100.0 / nAll) + "% : " + nExe + " / "
							+ nAll);
					if (detail) {
						foreach (var element in executedAndNot.Item2) {
							Console.Write(element.Position.SmartPosition);
						}
					}
				}
				{
					var executedAndNot = covInfo.StatementTargets
							.Where(e => e.Tag.StartsWith(tag))
							.Halve(e => e.State == CoverageState.Done);
					var nExe = executedAndNot.Item1.Count;
					var nAll = nExe + executedAndNot.Item2.Count;
					Console.WriteLine(
							"Statement Coverage: " + nExe * (100.0 / nAll) + "% : " + nExe + " / "
							+ nAll);
					if (detail) {
						foreach (var element in executedAndNot.Item2) {
							Console.Write(element.Position.SmartPosition);
						}
					}
				}
				{
					var executedAndNot = covInfo.BranchTargets
							.Where(e => e.Tag.StartsWith(tag))
							.Halve(e => e.State == CoverageState.Done);
					var nExe = executedAndNot.Item1.Count;
					var nAll = nExe + executedAndNot.Item2.Count;
					Console.WriteLine(
							"Branch Coverage: " + nExe * (100.0 / nAll) + "% : " + nExe + " / "
							+ nAll);
					if (detail) {
						foreach (var element in executedAndNot.Item2) {
							Console.Write(element.Position.SmartPosition);
						}
					}
				}
				{
					var nExe = 0;
					var nAll = 0;
					var notExecuted = new List<ICoverageElement>();
					var targets = covInfo.BranchConditionTargets.Where(
							e => e.Tag.StartsWith(tag))
							.SelectMany(
									e =>
									e.Targets.Count > 0
											? e.Targets : Enumerable.Repeat((ICoverageElement)e, 1));
					foreach (var t in targets) {
						nAll += 2;
						if (t.State == CoverageState.Done) {
							nExe += 2;
						} else {
							notExecuted.Add(t);
							if (t.State != CoverageState.None)
								nExe++;
						}
					}
					Console.WriteLine(
							"Branch Coverage in detail: " + nExe * (100.0 / nAll) + "% : " + nExe
							+ " / " + nAll);
					if (detail) {
						foreach (var element in notExecuted) {
							Console.Write(element.Position.SmartPosition);
						}
					}
				}
				{
					var executedAndNot = covInfo.BranchConditionTargets
							.Where(e => e.Tag.StartsWith(tag))
							.Halve(e => e.StateChildrenOrParent == CoverageState.Done);
					var nExe = executedAndNot.Item1.Count;
					var nAll = nExe + executedAndNot.Item2.Count;
					Console.WriteLine(
							"Condition Coverage: " + nExe * (100.0 / nAll) + "% : " + nExe + " / "
							+ nAll);
					if (detail) {
						foreach (var element in executedAndNot.Item2) {
							Console.Write(element.Position.SmartPosition);
						}
					}
				}
				{
					var nExe = 0;
					var nAll = 0;
					var notExecuted = new List<ICoverageElement>();
					foreach (
							var t in covInfo.BranchConditionTargets.Where(e => e.Tag.StartsWith(tag))
							) {
						if (t.Targets.Count == 0) {
							nAll += 2;
							if (t.State == CoverageState.Done) {
								nExe += 2;
							} else {
								notExecuted.Add(t);
								if (t.State != CoverageState.None)
									nExe++;
							}
						} else {
							foreach (var t2 in t.Targets) {
								nAll += 2;
								if (t2.State == CoverageState.Done) {
									nExe += 2;
								} else {
									notExecuted.Add(t2);
									if (t2.State != CoverageState.None)
										nExe++;
								}
							}
						}
					}
					Console.WriteLine(
							"Condition Coverage in detail: " + nExe * (100.0 / nAll) + "% : " + nExe
							+ " / "
							+ nAll);
					if (detail) {
						foreach (var element in notExecuted) {
							Console.Write(element.Position.SmartPosition);
						}
					}
				}
				{
					var executedAndNot = covInfo.BranchConditionTargets
							.Where(e => e.Tag.StartsWith(tag))
							.Halve(e => e.State == CoverageState.Done);
					var nExe = executedAndNot.Item1.Count;
					var nAll = nExe + executedAndNot.Item2.Count;
					Console.WriteLine(
							"Branch/Condition Coverage: " + nExe * (100.0 / nAll) + "% : " + nExe
							+ " / " + nAll);
					if (detail) {
						foreach (var element in executedAndNot.Item2) {
							Console.Write(element.Position.SmartPosition);
						}
					}
				}
				Console.WriteLine();
			}
			return true;
		}

		private static SortedSet<string> ReconstructTags(CoverageInfo info) {
			// タグを構成要素に分解して再構成する
			var tagSet = info.TargetList.Select(t => t.Tag).ToHashSet();
			var newTagSet = new SortedSet<string>();

			foreach (var tag in tagSet) {
				var tagElements = tag.Split(
						new[] { '>' },
						StringSplitOptions.RemoveEmptyEntries);
				var newTag = string.Empty;
				foreach (var tagEelment in tagElements) {
					newTag += tagEelment + '>';
					newTagSet.Add(newTag);
				}
			}
			return newTagSet;
		}
	}
}