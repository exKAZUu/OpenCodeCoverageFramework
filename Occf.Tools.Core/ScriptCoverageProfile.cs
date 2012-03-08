#region License

// Copyright (C) 2011-2012 Kazunori Sakamoto
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Code2Xml.Core.CodeToXmls;
using Code2Xml.Core.XmlToCodes;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Occf.Core.CoverageProfiles;
using Occf.Core.Operators.Inserters;
using Occf.Core.Operators.Selectors;
using Occf.Core.Operators.Taggers;
using Paraiba.Collections.Generic;

namespace Occf.Tools.Core {
    public class ScriptCoverageProfile : CoverageProfile {
        private static readonly IDictionary<string, ScriptCoverageProfile>
                Caches;

        private static readonly ScriptEngine PythonEngine;

        private readonly string _name;
        private readonly ScriptScope _scope;

        private Selector _functionSelector;
        private Selector _functionNameSelector;
        private Selector _statementSelector;
        private Selector _branchSelector;
        private Selector _caseLabelTailSelector;
        private CodeToXml _codeToXml;
        private Selector _conditionSelector;
        private string _filePattern;
        private Selector _foreachHeadSelector;
        private Selector _foreachSelector;
        private Selector _foreachTailSelector;
        private Selector _initializerSelector;

        private IEnumerable<string> _libraryNames;
        private NodeInserter _nodeInserter;

        private Selector _switchSelector;
        private Tagger _tagger;
        private Selector _testCaseLabelTailSelector;
        private XmlToCode _xmlToCode;

        static ScriptCoverageProfile() {
            Caches = new Dictionary<string, ScriptCoverageProfile>();
            PythonEngine = Python.CreateEngine();
        }

        private ScriptCoverageProfile(ScriptScope scope, string name) {
            _scope = scope;
            _name = name;
        }

        public override string Name {
            get { return _name; }
        }

        public override string FilePattern {
            get {
                return _filePattern ??
                       (_filePattern = Get<string>("FilePattern"));
            }
        }

        public override CodeToXml CodeToXml {
            get {
                return _codeToXml ??
                       (_codeToXml = Get<CodeToXml>("CodeToXml"));
            }
        }

        public override XmlToCode XmlToCode {
            get {
                return _xmlToCode ??
                       (_xmlToCode = Get<XmlToCode>("XmlToCode"));
            }
        }

        public override NodeInserter NodeInserter {
            get {
                return _nodeInserter ??
                       (_nodeInserter = Get<NodeInserter>("NodeInserter"));
            }
        }

        public override Selector StatementSelector {
            get {
                return _statementSelector ??
                       (_statementSelector = Get<Selector>("StatementSelector")
                                             ?? NoSelector.Instance);
            }
        }

        public override Selector InitializerSelector {
            get {
                return _initializerSelector ??
                       (_initializerSelector =
                        Get<Selector>("InitializerSelector")
                        ?? NoSelector.Instance);
            }
        }

        public override Selector BranchSelector {
            get {
                return _branchSelector ??
                       (_branchSelector = Get<Selector>("BranchSelector")
                                          ?? NoSelector.Instance);
            }
        }

        public override Selector ConditionSelector {
            get {
                return _conditionSelector ??
                       (_conditionSelector = Get<Selector>("ConditionSelector")
                                             ?? NoSelector.Instance);
            }
        }

        public override Selector SwitchSelector {
            get {
                return _switchSelector ??
                       (_switchSelector = Get<Selector>("SwitchSelector")
                                          ?? NoSelector.Instance);
            }
        }

        public override Selector CaseLabelTailSelector {
            get {
                return _caseLabelTailSelector ??
                       (_caseLabelTailSelector =
                        Get<Selector>("CaseLabelTailSelector")
                        ?? NoSelector.Instance);
            }
        }

        public override Selector TestCaseLabelTailSelector {
            get {
                return _testCaseLabelTailSelector ??
                       (_testCaseLabelTailSelector =
                        Get<Selector>("TestCaseLabelTailSelector")
                        ?? NoSelector.Instance);
            }
        }

        public override Selector ForeachSelector {
            get {
                return _foreachSelector ??
                       (_foreachSelector = Get<Selector>("ForeachSelector")
                                           ?? NoSelector.Instance);
            }
        }

        public override Selector ForeachHeadSelector {
            get {
                return _foreachHeadSelector ??
                       (_foreachHeadSelector =
                        Get<Selector>("ForeachHeadSelector")
                        ?? NoSelector.Instance);
            }
        }

        public override Selector ForeachTailSelector {
            get {
                return _foreachTailSelector ??
                       (_foreachTailSelector =
                        Get<Selector>("ForeachTailSelector")
                        ?? NoSelector.Instance);
            }
        }

        public override Selector FunctionSelector {
            get {
                return _functionSelector ??
                       (_functionSelector =
                        Get<Selector>("FunctionSelector")
                        ?? NoSelector.Instance);
            }
        }

        public override Selector FunctionNameSelector {
            get {
                return _functionNameSelector ??
                       (_functionNameSelector =
                        Get<Selector>("FunctionNameSelector")
                        ?? NoSelector.Instance);
            }
        }

        public override IEnumerable<string> LibraryNames {
            get {
                return _libraryNames ??
                       (_libraryNames = Get<IEnumerable>("LibraryNames")
                                                .Cast<object>()
                                                .Select(o => o.ToString()));
            }
        }

        public override Tagger Tagger {
            get { return _tagger ?? (_tagger = Get<Tagger>("Tagger")); }
        }

        private T Get<T>(string name) where T : class {
            dynamic func = _scope.GetVariable(name);
            if (func == null) {
                return default(T);
            }
            return func() as T;
        }

        public static void AllLoad() {
            var langs = new[] {
                    Tuple.Create("*.py", PythonEngine),
            };
            foreach (var lang in langs) {
                foreach (var name in Directory.EnumerateFiles(lang.Item1)) {
                    var path = Path.Combine("profiles", name);
                    var scope = lang.Item2.CreateScope();
                    lang.Item2.ExecuteFile(path, scope);
                    var nameWithoutExt = Path.GetFileNameWithoutExtension(name);
                    Caches[nameWithoutExt] = new ScriptCoverageProfile(
                            scope, nameWithoutExt);
                }
            }
        }

        public static ScriptCoverageProfile Load(string name) {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name));
            var profile = Caches.GetValueOrDefault(name);
            if (profile != null) {
                return profile;
            }

            var path = Path.Combine("profiles", name);
            ScriptEngine engine;
            if (File.Exists(path + ".py")) {
                path = path + ".py";
                engine = PythonEngine;
            } else {
                throw new ArgumentException();
            }
            var scope = engine.CreateScope();
            engine.ExecuteFile(path, scope);
            profile = new ScriptCoverageProfile(scope, name);
            Caches[name] = profile;
            return profile;
        }
    }
}