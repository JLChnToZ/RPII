using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace RPII {
	public class ItemPool<T>:ICollection<T> {
		readonly HashSet<ProbabilityItem<T>> items;
		
		public ItemPool() {
			items = new HashSet<ProbabilityItem<T>>(new ProbabilityItem<T>.Comparer());
		}
		
		public ProbabilityItem<T> AddItem(T content) {
			var item = new ProbabilityItem<T>(1, content);
			items.Add(item);
			return item;
		}
		
		public ProbabilityItem<T> AddItem(T content, float baseProb) {
			var item = new ProbabilityItem<T>(baseProb, content);
			items.Add(item);
			return item;
		}
		
		public void RemoveItem(ProbabilityItem<T> item) {
			items.Remove(item);
		}
		
		public IEnumerable GetItems() {
			foreach(var item in items)
				yield return item.content;
		}
		
		static float GetRandomValue(Random random) {
			return (float)random.NextDouble();
		}
		
		public object GetRandomValue(ItemPoolConfig config, Random random) {
			return GetRandomObject(config, GetRandomValue(random)).content;
		}
		
		public void GetRandomValue(ItemPoolConfig config, Random random, out T result, out string[] tags) {
			GetRandomValue(config, GetRandomValue(random), out result, out tags);
		}
		
		static float GetRandomValue(RandomNumberGenerator rng) {
			var rndBytes = new byte[4];
			rng.GetBytes(rndBytes);
			return (float)BitConverter.ToUInt32(rndBytes) / uint.MaxValue;
		}
		
		public object GetRandomValue(ItemPoolConfig config, RandomNumberGenerator rng) {
			return GetRandomObject(config, GetRandomValue(rng)).content;
		}
		
		public void GetRandomValue(ItemPoolConfig config, RandomNumberGenerator rng, out T result, out string[] tags) {
			GetRandomValue(config, GetRandomValue(rng), out result, out tags);
		}
		
		public void GetRandomValue(ItemPoolConfig config, float randomValue, out T result, out string[] tags) {
			var value = GetRandomObject(config, randomValue);
			result = value.content;
			tags = new List<string>(value.tags).ToArray();
		}
		
		public ProbabilityItem<T> GetRandomObject(ItemPoolConfig config, float randomValue) {
			float sum = 0;
			var probDict = new Dictionary<ProbabilityItem<T>, float>();
			foreach(var item in items) {
				float prob = item.CalculateProb(config);
				if(prob < 0)
					throw new InvalidOperationException("Prob is out of range.");
				probDict[item] = prob;
				sum += prob;
			}
			randomValue *= sum;
			float count = 0;
			foreach(var kv in probDict) {
				count += kv.Value;
				if(randomValue < count)
					return kv.Key;
			}
		}

		public void Add(T item) {
			items.Add(new ProbabilityItem<T>(1, item);
		}

		public void Clear() {
			items.Clear();
		}

		public bool Contains(T item) {
			return items.Contains(new ProbabilityItem<T>(1, item));
		}

		public void CopyTo(T[] array, int arrayIndex) {
			int i = arrayIndex;
			foreach(var item in items)
				array[i++] = item.content;
		}

		public bool Remove(T item) {
			return items.Remove(new ProbabilityItem<T>(1, item));
		}

		public int Count {
			get { return items.Count; }
		}

		public bool IsReadOnly {
			get { return false; }
		}

		public IEnumerator<T> GetEnumerator() {
			foreach(var item in items)
				yield return item.content;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
