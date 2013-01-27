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
using System.IO;

namespace Occf.Core.Utils {
	public enum SortingOption {
		None,
		Name,
	}

	public static class DirectoryUtil {
		/// <summary>
		///   Returns the names of files (including their paths) that match the specified search pattern in the specified directory, using a value to determine whether to search subdirectories and a value to determine how to sort.
		/// </summary>
		/// <param name="path"> The directory to search. </param>
		/// <param name="searchPattern"> The search string to match against the names of files in path. The parameter cannot end in two periods ("..") or contain two periods ("..") followed by DirectorySeparatorChar or AltDirectorySeparatorChar, nor can it contain any of the characters in InvalidPathChars. </param>
		/// <param name="searchOption"> One of the SearchOption values that specifies whether the search operation should include all subdirectories or only the current directory. </param>
		/// <param name="sortingOption"> One of the SortingOption values that specifies how to sort string arrays to return. </param>
		/// <returns> A String array containing the names of files which is sorted with the specified sorting option in the specified directory that match the specified search pattern. File names include the full path. </returns>
		public static string[] GetFiles(
				string path, string searchPattern,
				SearchOption searchOption,
				SortingOption sortingOption) {
			var paths = Directory.GetFiles(path, searchPattern, searchOption);
			switch (sortingOption) {
			case SortingOption.None:
				break;
			case SortingOption.Name:
				Array.Sort(paths);
				break;
			default:
				throw new ArgumentOutOfRangeException("sortingOption");
			}
			return paths;
		}
	}
}