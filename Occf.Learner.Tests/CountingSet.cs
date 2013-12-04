using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Occf.Learner.Core.Tests {
	/// <summary>
	/// A class for counting the specified values.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CountingSet<T> : ICollection<T> {
		private readonly Dictionary<T, int> _dict;

		public IEnumerable<KeyValuePair<T, int>> ItemsWithCount {
			get { return _dict; }
		}

		public CountingSet(IEnumerable<T> values) {
			_dict = new Dictionary<T, int>();
			foreach (var key in values) {
				int value;
				if (_dict.TryGetValue(key, out value)) {
					_dict[key] = value + 1;
				} else {
					_dict.Add(key, 1);
				}
			}
		}

		public void Add(T item) {
			Add(item, 1);
		}

		public void Clear() {
			_dict.Clear();
		}

		public bool Contains(T item) {
			return _dict.ContainsKey(item);
		}

		public void CopyTo(T[] array, int arrayIndex) {
			for (int i = arrayIndex; i < array.Length; i++) {
				Add(array[i]);
			}
		}

		public bool Remove(T item) {
			return Remove(item, 1);
		}

		public int Count {
			get { return _dict.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public IEnumerator<T> GetEnumerator() {
			return _dict.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return _dict.Keys.GetEnumerator();
		}

		public void Add(T item, int value) {
			Contract.Requires(value >= 0);
			int count;
			if (_dict.TryGetValue(item, out count)) {
				_dict[item] = count + value;
			} else {
				_dict.Add(item, value);
			}
		}

		public bool Remove(T item, int value) {
			Contract.Requires(value >= 0);
			int count;
			if (_dict.TryGetValue(item, out count)) {
				count -= value;
				if (count > 0) {
					_dict[item] = count;
				} else {
					_dict.Remove(item);
				}
				return true;
			}
			return false;
		}

		public bool ClearItem(T item) {
			return _dict.Remove(item);
		}

		public int ClearItemsIf(Func<T, bool> predicate) {
			var count = 0;
			foreach (var key in _dict.Keys) {
				if (predicate(key)) {
					_dict.Remove(key);
					count++;
				}
			}
			return count;
		}

		public int ClearItemsIf(Func<T, int, bool> predicate) {
			var count = 0;
			foreach (var kv in _dict.ToList()) {
				if (predicate(kv.Key, kv.Value)) {
					_dict.Remove(kv.Key);
					count++;
				}
			}
			return count;
		}

		public int CountItem(T item) {
			int count;
			if (_dict.TryGetValue(item, out count)) {
				return count;
			}
			return 0;
		}

		public int SumCounts() {
			return _dict.Values.Sum();
		}

		public void UnionWith(CountingSet<T> set) {
			foreach (var kv in set.ItemsWithCount) {
				Add(kv.Key, kv.Value);
			}
		}

		public void ExceptFor(CountingSet<T> set) {
			foreach (var kv in set.ItemsWithCount) {
				Remove(kv.Key, kv.Value);
			}
		}
	}

	public static class CountingSet {
		public static CountingSet<T> ToCountingSet<T>(this IEnumerable<T> items) {
			return new CountingSet<T>(items);
		}
	}
}