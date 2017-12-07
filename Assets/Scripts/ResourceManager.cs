using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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

	public List<Resource> GetResourcesList(int amount)
	{
		List<Resource> list = new List<Resource>();
		for(int i = 0; i < amount; i++)
		{
			int randIndex = Random.Range(0, allResources.Count);
			Resource chosenResource = allResources[randIndex];

			chosenResource.Amount = Random.Range(chosenResource.MinQuantity, chosenResource.MaxQuantity + 1);
			
			int containsIndex = list.FindIndex(r => r.Name == chosenResource.Name);

        	if(containsIndex != -1)
			{
				list[containsIndex].Amount += chosenResource.Amount;
			}
			else
			{
				list.Add(chosenResource);
			}
		}
		return list;
	}


	
}
