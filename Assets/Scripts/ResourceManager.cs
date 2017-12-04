using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ResourceManager : MonoBehaviour {

	public static ResourceManager Instance;

	private List<Resource> allResources;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		if(Instance != null && Instance != this){
			Destroy(gameObject);
		}else{
			Instance = this;
		}

		allResources = JsonConvert.DeserializeObject<List<Resource>>(Resources.Load("JSON/Resources").ToString());
	}

	public List<Resource> GetResourcesList()
	{
		return null;
	}
}
