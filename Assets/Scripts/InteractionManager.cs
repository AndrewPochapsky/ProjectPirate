using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class InteractionManager : MonoBehaviour {

	public static InteractionManager Instance;


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
	}

	public List<Interaction> GetInteractionList()
	{
		return JsonConvert.DeserializeObject<List<Interaction>>(Resources.Load("JSON/Interactions").ToString());		
	}

	public Interaction GetInteraction(List<Interaction> interactions, string name)
	{
		foreach(Interaction i in interactions)
		{
			if(i.Name == name)
				return i;
		}
		return null;
	}

}
