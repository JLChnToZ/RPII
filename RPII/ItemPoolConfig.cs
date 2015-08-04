using System;
using System.Collections.Generic;

namespace RPII {
	public class ItemPoolConfig:ICloneable {
		readonly Dictionary<string, HashSet<ProbabilityModifier>> modifiers;
		
		public ItemPoolConfig() {
			modifiers = new Dictionary<string, HashSet<ProbabilityModifier>>();
		}
		
		HashSet<ProbabilityModifier> InternalGetModifiers(string tag) {
			HashSet<ProbabilityModifier> modifierInTag;
			if(!modifiers.TryGetValue(tag, out modifierInTag)) {
				modifierInTag = new HashSet<ProbabilityModifier>();
				modifiers.Add(tag, modifierInTag);
			}
			return modifierInTag;
		}
		
		void RecycleModifier(string tag) {
			HashSet<ProbabilityModifier> modifierInTag;
			if(!modifiers.TryGetValue(tag, out modifierInTag))
				return;
			if(modifierInTag.Count <= 0)
				modifiers.Remove(tag);
		}
		
		public void AddModifier(ProbabilityModifier modifier, params string[] tags) {
			foreach(var tag in tags)
				AddModifier(tag, modifier);
		}
		
		public void AddModifier(string tag, ProbabilityModifier modifier) {
			InternalGetModifiers(tag).Add(modifier);
		}
		
		public void RemoveModifier(ProbabilityModifier modifier, params string[] tags) {
			foreach(var tag in tags)
				RemoveModifier(tag, modifier);
		}
		
		public void RemoveModifier(string tag, ProbabilityModifier modifier) {
			InternalGetModifiers(tag).Remove(modifier);
			RecycleModifier(tag);
		}
		
		public void RemoveAllModifiers(string tag) {
			InternalGetModifiers(tag).Clear();
			RecycleModifier(tag);
		}
		
		public void RemoveAllModifiers() {
			foreach(var modifier in modifiers.Values)
				modifier.Clear();
			modifiers.Clear();
		}
		
		public IEnumerable<ProbabilityModifier> GetModifiers(string tag) {
			foreach(var item in InternalGetModifiers(tag))
				yield return item;
		}
		
		public object Clone() {
			var newInstance = new ItemPoolConfig();
			foreach(var kv in modifiers)
				newInstance.modifiers.Add(kv.Key, new HashSet<ProbabilityModifier>(kv.Value));
			return newInstance;
		}
	}
}
