using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction  {

	public string Name { get; set; }
	public float BaseTimeRequired { get; set; }
	//TODO: replace this with a crew member
	public bool Assigned { get; set; }
	public bool Completed { get; set; }

	[Newtonsoft.Json.JsonConstructor]
	public Interaction(string name, float time, bool assigned)
	{
		Name = name;
		BaseTimeRequired = time;
		Assigned = assigned;
		Completed = false;
	}



}
