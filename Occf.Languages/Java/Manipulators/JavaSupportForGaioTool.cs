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

using System.ComponentModel.Composition;
using System.IO;
using Occf.Core.Manipulators;
using Occf.Languages.Java.Properties;
using Paraiba.IO;

namespace Occf.Languages.Java.Manipulators {
	[Export(typeof(LanguageSupport))]
	public class JavaSupportForGaioTool : JavaSupport {
		public override string Name {
			get { return "JavaForUM"; }
		}

		public override void CopyLibraries(DirectoryInfo outDirInfo, RecordingMode recordingMode) {
			ParaibaFile.WriteIfDifferentSize(
					Path.Combine(outDirInfo.FullName, "CoverageWriter.UM.jar"),
					Resources.CoverageWriter_UM);
			ParaibaFile.WriteIfDifferentSize(
					Path.Combine(outDirInfo.FullName, "junit-4.8.2.jar"),
					Resources.junit_4_8_2);
			ParaibaFile.WriteIfDifferentSize(
					Path.Combine(outDirInfo.FullName, "Occf.Writer.UM.Java.dll"),
					Resources.Occf_Writer_UM_Java);
		}

		public override void RemoveLibraries(DirectoryInfo outDirInfo) {
			outDirInfo.GetFile("CoverageWriter.UM.jar");
			outDirInfo.GetFile("junit-4.8.2.jar");
			outDirInfo.GetFile("Occf.Writer.UM.Java.dll");
		}
	}
}