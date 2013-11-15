using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Code2Xml.Core;
using Occf.Core.Manipulators;
using Occf.Core.Manipulators.Analyzers;
using Occf.Core.Manipulators.Taggers;
using Occf.Core.Manipulators.Transformers;
using Occf.Languages.Java.Manipulators.Analyzers;
using Occf.Languages.Java.Manipulators.Taggers;
using Occf.Languages.Java.Manipulators.Transformers;
using Occf.Languages.Java.Properties;
using Paraiba.IO;

namespace Occf.Languages.Java.Manipulators {
	[Export(typeof(LanguageSupport))]
	public class JavaSupport : LanguageSupport {
		private IEnumerable<string> _filePatterns;
		private AstTransformer _inserter;
		private Tagger _tagger;

		public override string Name {
			get { return "Java"; }
		}

		public override Processor Processor {
			get { return ProcessorLoader.JavaUsingAntlr3; }
		}

		public override IEnumerable<string> FilePatterns {
			get { return _filePatterns ?? (_filePatterns = new[] { "*.java" }); }
		}

		public override AstTransformer AstTransformer {
			get { return _inserter ?? (_inserter = new JavaAstTransformer()); }
		}

		public override AstAnalyzer AstAnalyzer {
			get { return JavaAstAnalyzer.Instance; }
		}

		public override Tagger Tagger {
			get { return _tagger ?? (_tagger = new JavaTagger()); }
		}

		public override void CopyLibraries(DirectoryInfo outDirInfo, RecordingMode recordingMode) {
			switch (recordingMode) {
			case RecordingMode.BinaryFile:
				ParaibaFile.WriteIfDifferentSize(
						Path.Combine(outDirInfo.FullName, "CoverageWriter.jar"),
						Resources.CoverageWriter_File);
				ParaibaFile.WriteIfDifferentSize(
						Path.Combine(
								outDirInfo.FullName, "Occf.Writer.File.Java.dll"),
						Environment.Is64BitOperatingSystem
								? Resources.Occf_Writer_File_Java_x64
								: Resources.Occf_Writer_File_Java_x86);
				break;
			case RecordingMode.TextFile:
				ParaibaFile.WriteIfDifferentSize(
						Path.Combine(outDirInfo.FullName, "CoverageWriter.jar"),
						Resources.CoverageWriter_Text);
				break;
			case RecordingMode.Gaio:
				break;
			case RecordingMode.SharedMemory:
				break;
			case RecordingMode.TcpIp:
				break;
			default:
				throw new ArgumentOutOfRangeException("recordingMode");
			}

			ParaibaFile.WriteIfDifferentSize(
					Path.Combine(outDirInfo.FullName, "junit-4.8.2.jar"),
					Resources.junit_4_8_2);
		}

		public override void RemoveLibraries(DirectoryInfo outDirInfo) {
			outDirInfo.GetFile("CoverageWriter.jar");
			outDirInfo.GetFile("junit-4.8.2.jar");
			outDirInfo.GetFile("Occf.Writer.File.Java.dll");
		}
	}
}