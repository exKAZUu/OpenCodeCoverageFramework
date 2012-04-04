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
using NDesk.Options;
using Occf.Core.CoverageInformation;
using Occf.Tools.Core;
using Paraiba.IO;
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

			if (args.Count < 1) {
				return Program.Print(Usage);
			}

			var iArgs = 0;
			var rootDir = new DirectoryInfo(args[iArgs++]);
			if (!rootDir.Exists) {
				return Program.Print(
						"Root directory doesn't exist.\nroot:" + rootDir.FullName);
			}

			var covDataFile = args.Count >= iArgs + 1
			                  		? new FileInfo(args[iArgs++]) : null;
			covDataFile = PathFinder.FindCoverageDataPath(covDataFile, rootDir);
			if (!covDataFile.SafeExists()) {
				return
						Program.Print(
								"Coverage data file doesn't exist.\ncoverage:" + covDataFile.FullName);
			}

			return Analyze(rootDir, covDataFile, detail);
		}

		private static bool Analyze(
				DirectoryInfo rootDir, FileInfo covDataFile, bool detail) {
			// カバレッジ情報（母数）の取得
			var formatter = new BinaryFormatter();
			var covInfoPath = PathFinder.FindCoverageInfoPath(rootDir);
			var covInfo = InfoReader.ReadCoverageInfo(covInfoPath, formatter);
			CoverageDataReader.ReadFile(covInfo, covDataFile);

			var tags = ReconstructTags(covInfo);
			foreach (var tag in tags /*.Where(t => !t.Contains("class "))*/) {
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
							if (t.State != CoverageState.None) {
								nExe++;
							}
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
								if (t.State != CoverageState.None) {
									nExe++;
								}
							}
						} else {
							foreach (var t2 in t.Targets) {
								nAll += 2;
								if (t2.State == CoverageState.Done) {
									nExe += 2;
								} else {
									notExecuted.Add(t2);
									if (t2.State != CoverageState.None) {
										nExe++;
									}
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
			var tagSet = info.Targets.Select(t => t.Tag).ToHashSet();
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