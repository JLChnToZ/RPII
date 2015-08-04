using System;
using System.Collections;
using System.Collections.Generic;

namespace RPII {
	public class ProbabilityModifier:ICloneable {
		readonly Dictionary<int, float> modifiers;
		
		public ProbabilityModifier() {
			modifiers = new Dictionary<int, float>();
		}
		
		public static ProbabilityModifier Default {
			get {
				var instance = new ProbabilityModifier();
				instance.modifiers.Add(0, 1);
				return instance;
			}
		}
		
		internal float GetValue(float original) {
			float value = 0;
			foreach(var kv in modifiers)
				value += (float)Math.Pow(original * kv.Value, kv.Key);
			return value;
		}
		
		public float this[int baseValue] {
			get {
				return modifiers.ContainsKey(baseValue) ? modifiers[baseValue] : 0;
			}
			set {
				modifiers[baseValue] = value;
			}
		}

		public object Clone() {
			var newInstance = new ProbabilityModifier();
			foreach(var kv in modifiers)
				newInstance.modifiers.Add(kv.Key, kv.Value);
			return newInstance;
		}
	}
	
	public class ProbabilityItem<T>:ICollection<string> {
		internal readonly HashSet<string> tags;
		public readonly float baseProbability;
		public readonly T content;
		
		public ProbabilityItem(float probability, T content) {
			this.tags = new HashSet<string>();
			this.content = content;
			this.baseProbability = probability;
		}
		
		public void AddTag(params string[] tags) {
			foreach(var tag in tags)
				this.tags.Add(tag);
		}
		
		public bool HasTag(string tag) {
			return tags.Contains(tag);
		}
		
		public bool RemoveTag(params string[] tags) {
			foreach(var tag in tags)
				this.tags.Remove(tag);
		}
		
		internal float CalculateProb(ItemPoolConfig config) {
			var value = baseProbability;
			foreach(var tag in tags)
				foreach(var modifier in config.GetModifiers(tag))
					value = modifier.GetValue(value);
			return value;
		}
		
		void ICollection<string>.Add(string item) {
			tags.Add(item);
		}

		void ICollection<string>.Clear() {
			tags.Clear();
		}

		bool ICollection<string>.Contains(string item) {
			return tags.Contains(item);
		}

		void ICollection<string>.CopyTo(string[] array, int arrayIndex) {
			tags.CopyTo(array, arrayIndex);
		}

		bool ICollection<string>.Remove(string item) {
			return tags.Remove(item);
		}

		int ICollection<string>.Count {
			get { return tags.Count; }
		}

		bool ICollection<string>.IsReadOnly {
			get { return false; }
		}

		IEnumerator<string> IEnumerable<string>.GetEnumerator() {
			return tags.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return tags.GetEnumerator();
		}
		
		internal class Comparer: IEqualityComparer<ProbabilityItem<T>> {
			public bool Equals(ProbabilityItem<T> x, ProbabilityItem<T> y) {
				if(x == null || y == null)
					return false;
				return x.content.Equals(y.content);
			}
			
			public int GetHashCode(ProbabilityItem<T> obj) {
				return obj.content.GetHashCode();
			}
		}
	}
}
