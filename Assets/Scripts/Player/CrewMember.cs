using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewMember {

	public string Name { get; private set; }
	public Interaction Task { get; set; }

	public CrewMember(string name)
	{
		Name = name;
		Task = null;
	}
}
