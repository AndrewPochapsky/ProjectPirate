using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class Interaction  {

	[JsonConverter(typeof(StringEnumConverter))]
	public enum Type { survey, gatherResources }

	public Type InteractionType { get; set; }
	public int Duration { get; set; }
	public CrewMember assignee { get; set; }
	public bool Completed { get; set; }
	public bool InProgress { get; set; }

	[Newtonsoft.Json.JsonConstructor]
	public Interaction(Type type, int duration)
	{
		InteractionType = type;
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
