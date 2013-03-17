#region License

// Copyright (C) 2012-2013 Kazunori Sakamoto
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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Occf.Writer.Builder {
    internal class Program {
        private static void Main(string[] args) {
            ChangeCurrentDirectory();
            ApplySwigToJava();
            ApplySwigToPython();
        }

        private static void ApplySwigToPython() {
            var dirPaths = new[] { "Binary.Python2", "Binary.Python3" };
            foreach (var dirPath in dirPaths) {
                var swigArgs = new[] {
                        "-c++", "-python", "*.i"
                };
                var info = new ProcessStartInfo {
                        FileName = "swig",
                        Arguments = string.Join(" ", swigArgs),
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WorkingDirectory = dirPath,
                };
                try {
                    using (var p = Process.Start(info)) p.WaitForExit();
                } catch (Win32Exception e) {
                    throw new InvalidOperationException("Failed to launch " + info.FileName + ".", e);
                }
                Console.WriteLine("Processed: " + Path.GetFullPath(dirPath));
            }
        }

        private static void ApplySwigToJava() {
            var dirPaths = new[] { "Binary.Java", "SharedMemory.Java", "TcpIp.Java" };
            foreach (var dirPath in dirPaths) {
                var swigArgs = new[] {
                        "-c++", "-java", "-package", "jp.ac.waseda.cs.washi", "*.i"
                };
                var info = new ProcessStartInfo {
                        FileName = "swig",
                        Arguments = string.Join(" ", swigArgs),
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WorkingDirectory = dirPath,
                };
                try {
                    using (var p = Process.Start(info)) p.WaitForExit();
                } catch (Win32Exception e) {
                    throw new InvalidOperationException("Failed to launch " + info.FileName + ".", e);
                }

                var srcPath = Path.Combine(dirPath, "CoverageWriter.java");
                var lines = File.ReadAllLines(srcPath);
                var fragment = "static { System.loadLibrary(\"Occf.Writer." + dirPath + "\"); }";
                var newLines = lines.Select(line =>
                        line.StartsWith("}") ? fragment + line : line);
                File.WriteAllLines(srcPath, newLines);

                var newDirPath = Path.Combine(dirPath, "CoverageWriter", "src", "main", "java", "jp",
                        "ac", "waseda", "cs", "washi");
                File.Copy(Path.Combine(dirPath, "CoverageWriter.java"),
                        Path.Combine(newDirPath, "CoverageWriter.java"), true);
                File.Copy(Path.Combine(dirPath, "CoverageWriterJNI.java"),
                        Path.Combine(newDirPath, "CoverageWriterJNI.java"), true);
                Console.WriteLine("Processed: " + Path.GetFullPath(dirPath));
            }
        }

        private static void ChangeCurrentDirectory() {
            var dirInfo = new DirectoryInfo(Environment.CurrentDirectory);
            Environment.CurrentDirectory = dirInfo.Parent.Parent.FullName;
        }
    }
}