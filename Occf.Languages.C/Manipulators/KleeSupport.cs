﻿#region License

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

using System.ComponentModel.Composition;
using System.IO;
using Occf.Core.Modes;
using Occf.Languages.C.Properties;
using Paraiba.IO;

namespace Occf.Languages.C.Modes {
	[Export(typeof(LanguageSupport))]
	public class KleeSupport : CSupport {
		public override void CopyLibraries(DirectoryInfo outDirInfo) {
			ParaibaFile.WriteIfDifferentSize(
					Path.Combine(outDirInfo.FullName, "covman.c"),
					Resources.covman_klee_c);
			ParaibaFile.WriteIfDifferentSize(
					Path.Combine(outDirInfo.FullName, "covman.h"),
					Resources.covman_h);
		}
	}
}