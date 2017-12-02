using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction  {

	public string Name { get; set; }
	public float BaseTimeRequired { get; set; }
	public CrewMember assignee { get; set; }
	public bool Completed { get; set; }

	[Newtonsoft.Json.JsonConstructor]
	public Interaction(string name, float time)
	{
		Name = name;
		BaseTimeRequired = time;
		assignee = null;
		Completed = false;
	}



}
