using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class InteractionManager : MonoBehaviour {

	public static InteractionManager Instance;


	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
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
