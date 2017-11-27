using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class DataGenerator : MonoBehaviour{

	public static DataGenerator Instance { get; set; }
	private List<string> islandNames;

	// Use this for initialization
	void Start () {
		if(Instance != null && Instance != this){
			Destroy(gameObject);
		}else{
			Instance = this;
		}

		islandNames = JsonConvert.DeserializeObject<List<string>>(Resources.Load<TextAsset>("JSON/IslandNames").ToString());
	}
	
	public string GenerateIslandName(int numSyllables)
	{
		string name = string.Empty;
		for(int i = 0; i < numSyllables; i++)
		{
			name+=islandNames[Random.Range(0, islandNames.Count)];
		}
		name+=" Island";
		name = FirstCharToUpper(name);
		return name;
	}

	private string FirstCharToUpper(string s)
	{
		return char.ToUpper(s[0]) + s.Substring(1);
	}
}
