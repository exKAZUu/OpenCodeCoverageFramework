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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Occf.Core.Utils;
using Paraiba.Linq;

namespace Occf.Core.Profiles {
	public class CoverageProfiles {
		private static CoverageProfiles _instance;

#pragma warning disable 649
		[ImportMany] private IEnumerable<CoverageProfile> _coverageProfiles;

#pragma warning restore 649

		public static IEnumerable<CoverageProfile> Profile {
			get { return Instance._coverageProfiles; }
		}

		private CoverageProfiles() {
			var catalog = new AggregateCatalog();
			catalog.Catalogs.Add(
					new AssemblyCatalog(Assembly.GetExecutingAssembly()));
			catalog.Catalogs.Add(
					new DirectoryCatalog(OccfGlobal.CurrentDirectory, "Occf*.dll"));
			//catalog.Catalogs.Add(new DirectoryCatalog("Extensions"));
			var container = new CompositionContainer(catalog);
			container.ComposeParts(this);
		}

		private static CoverageProfiles Instance {
			get { return _instance ?? (_instance = new CoverageProfiles()); }
		}

		public static CoverageProfile GetCoverageProfileByName(string name) {
			var lowerName = name.ToLower();
			return Profile
					.Where(p => p.Name.ToLower().Contains(lowerName))
					.MinElementOrDefault(p => Math.Abs(p.Name.Length - name.Length));
		}

		public static CoverageProfile GetCoverageProfileByClassName(string className) {
			var lowerName = className.ToLower();
			return Profile
					.Where(p => p.GetType().Name.ToLower().Contains(lowerName))
					.MinElementOrDefault(
							p => Math.Abs(p.GetType().Name.Length - className.Length));
		}
	}
}