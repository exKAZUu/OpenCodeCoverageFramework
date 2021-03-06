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

namespace Occf.Core.Utils {
	public static class OccfSettings {
		public static string Python2() {
			return @"C:\Python27\python.exe";
		}

		public static string Python3() {
			return @"C:\Python31\python.exe";
		}

		public static string Ruby19() {
			return @"C:\Ruby192\bin\ruby.exe";
		}
	}
}