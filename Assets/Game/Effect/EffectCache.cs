using System.Collections.Generic;
using UnityEngine;

public class EffectCache : MonoBehaviour
{
	private Dictionary<string, Effect> cache;

	public Effect GetPrefab(string name) => cache[name];
	public Effect Create(string name) => Instantiate(GetPrefab(name), transform);
	public T Create<T>(string name) where T:Effect => (T)Create(name);

	private void Awake()
	{
		// load all effects into cache
		var prefabs = Resources.LoadAll<Effect>("Effects");
		cache = new Dictionary<string, Effect>();
		for (int i = 0; i < prefabs.Length; ++i)
			cache.Add(prefabs[i].name, prefabs[i]);
	}
}
