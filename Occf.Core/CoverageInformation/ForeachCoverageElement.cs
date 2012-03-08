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
using System.Xml.Linq;
using Occf.Core.Operators.Taggers;

namespace Occf.Core.CoverageInformation {
	[Serializable]
	public class ForeachCoverageElement : CoverageElement {
		private CoverageState _lastState;

		public ForeachCoverageElement(
				string relativePath, XElement node, Tagger tagger)
				: base(relativePath, node, tagger) {}

		/// <summary>
		///   Updates <c>State</c> property if
		/// </summary>
		/// <param name="state"> </param>
		public override void UpdateState(CoverageState state) {
			// DummyRecord(TrueOnly)
			// foreach() {
			//   Record(FalseOnly) ループ判定成立
			//   statement;
			//   DummyRecord(TrueOnly)
			// }
			// Record(TrueOnly)    ループ判定不成立（TrueOnlyが連続すれば）
			if (state == CoverageState.FalseOnly || _lastState == state) {
				State |= state;
			}
			_lastState = state;
		}
	}
}