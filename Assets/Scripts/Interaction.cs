using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction  {

	public string Name { get; set; }
	public int Duration { get; set; }
	public CrewMember assignee { get; set; }
	public bool Completed { get; set; }
	public bool InProgress { get; set; }

	[Newtonsoft.Json.JsonConstructor]
	public Interaction(string name, int duration)
	{
		Name = name;
		Duration = duration;
		assignee = null;
		Completed = false;
		InProgress = false;
	}

	public static void UnAssignDuplicateTasks(CrewMember crewMember, Player player)
	{
		foreach(CrewMember member in player.crew)
		{
			if(crewMember.Name != member.Name &&  crewMember.Task == member.Task)
			{
				member.Task = null;
			}
		}
	}


}
