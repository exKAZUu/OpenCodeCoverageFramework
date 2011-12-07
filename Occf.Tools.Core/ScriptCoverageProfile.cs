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
using Occf.Core.Extensions;
using Occf.Core.Operators.Inserters;
using Occf.Core.Operators.Selectors;
using Occf.Core.Operators.Taggers;
using Paraiba.Collections.Generic;

namespace Occf.Tools.Core {
	public class ScriptCoverageProfile : CoverageProfile {
		private static readonly IDictionary<string, ScriptCoverageProfile> Caches;
		private static readonly ScriptEngine PythonEngine;

		public string Name { get; private set; }

		private readonly ScriptScope _scope;

		private string _filePattern;

		private CodeToXml _codeToXml;
		private XmlToCode _xmlToCode;

		private NodeInserter _nodeInserter;

		private Selector _statementSelector;
		private Selector _initializerSelector;
		private Selector _branchSelector;
		private Selector _conditionSelector;
		private Selector _switchSelector;
		private Selector _caseLabelTailSelector;
		private Selector _testCaseLabelTailSelector;
		private Selector _foreachSelector;
		private Selector _foreachHeadSelector;
		private Selector _foreachTailSelector;

		private IEnumerable<string> _libraryNames;
		private Tagger _tagger;

		static ScriptCoverageProfile() {
			Caches = new Dictionary<string, ScriptCoverageProfile>();
			PythonEngine = Python.CreateEngine();
		}

		private ScriptCoverageProfile(ScriptScope scope, string name) {
			_scope = scope;
			Name = name;
		}

		private T GetValue<T>(string name) where T : class {
			dynamic func = _scope.GetVariable(name);
			if (func == null)
				return default(T);
			return func() as T;
		}

		public override string FilePattern {
			get {
				return _filePattern ??
				       (_filePattern = GetValue<string>("FilePattern"));
			}
		}

		public override CodeToXml CodeToXml {
			get {
				return _codeToXml ??
				       (_codeToXml = GetValue<CodeToXml>("CodeToXml"));
			}
		}

		public override XmlToCode XmlToCode {
			get {
				return _xmlToCode ??
				       (_xmlToCode = GetValue<XmlToCode>("XmlToCode"));
			}
		}

		public override NodeInserter NodeInserter {
			get {
				return _nodeInserter ??
				       (_nodeInserter = GetValue<NodeInserter>("NodeInserter"));
			}
		}

		public override Selector StatementSelector {
			get {
				return _statementSelector ??
				       (_statementSelector = GetValue<Selector>("StatementSelector")
				                             ?? NoSelector.Instance);
			}
		}

		public override Selector InitializerSelector {
			get {
				return _initializerSelector ??
				       (_initializerSelector = GetValue<Selector>("InitializerSelector")
				                               ?? NoSelector.Instance);
			}
		}

		public override Selector BranchSelector {
			get {
				return _branchSelector ??
				       (_branchSelector = GetValue<Selector>("BranchSelector")
				                          ?? NoSelector.Instance);
			}
		}

		public override Selector ConditionSelector {
			get {
				return _conditionSelector ??
				       (_conditionSelector = GetValue<Selector>("ConditionSelector")
				                             ?? NoSelector.Instance);
			}
		}

		public override Selector SwitchSelector {
			get {
				return _switchSelector ??
				       (_switchSelector = GetValue<Selector>("SwitchSelector")
				                          ?? NoSelector.Instance);
			}
		}

		public override Selector CaseLabelTailSelector {
			get {
				return _caseLabelTailSelector ??
				       (_caseLabelTailSelector = GetValue<Selector>("CaseLabelTailSelector")
				                             ?? NoSelector.Instance);
			}
		}

		public override Selector TestCaseLabelTailSelector {
			get {
				return _testCaseLabelTailSelector ??
				       (_testCaseLabelTailSelector = GetValue<Selector>("TestCaseLabelTailSelector")
				                            ?? NoSelector.Instance);
			}
		}

		public override Selector ForeachSelector {
			get {
				return _foreachSelector ??
				       (_foreachSelector = GetValue<Selector>("ForeachSelector")
				                             ?? NoSelector.Instance);
			}
		}

		public override Selector ForeachHeadSelector {
			get {
				return _foreachHeadSelector ??
				       (_foreachHeadSelector = GetValue<Selector>("ForeachHeadSelector")
				                             ?? NoSelector.Instance);
			}
		}

		public override Selector ForeachTailSelector {
			get {
				return _foreachTailSelector ??
				       (_foreachTailSelector = GetValue<Selector>("ForeachTailSelector")
				                             ?? NoSelector.Instance);
			}
		}

		public override IEnumerable<string> LibraryNames {
			get {
				return _libraryNames ??
				       (_libraryNames = GetValue<IEnumerable>("LibraryNames")
				                        		.Cast<object>()
				                        		.Select(o => o.ToString()));
			}
		}

		public override Tagger Tagger {
			get { return _tagger ?? (_tagger = GetValue<Tagger>("Tagger")); }
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
					Caches[nameWithoutExt] = new ScriptCoverageProfile(scope, nameWithoutExt);
				}
			}
		}

		public static ScriptCoverageProfile Load(string name) {
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name));
			var profile = Caches.GetValueOrDefault(name);
			if (profile != null)
				return profile;

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