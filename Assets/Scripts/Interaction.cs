using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class Interaction  {

	[JsonConverter(typeof(StringEnumConverter))]
	public enum Type { 
		survey, 
		gatherResources,
		gather_apple, 
		gather_orange, 
		gather_bannana, 
		gather_metal, 
		gather_wood,
		none }

	public Type InteractionType { get; private set; } 
	public string DisplayName { get; private set; }
	public int Duration { get; private set; }
	public CrewMember assignee { get; set; }
	public bool Completed { get; set; }
	public bool OneTime { get; private set; }
	public bool InProgress { get; set; }
	public Type Prerequisite { get; private set; }
	public UnityEngine.UI.Button AssignCrewButton { get; set; }

	[Newtonsoft.Json.JsonConstructor]
	public Interaction(Type type, string displayName, int duration, bool oneTime, Type prerequisite)
	{
		InteractionType = type;
		DisplayName = displayName;
		Duration = duration;
		OneTime = oneTime;
		Prerequisite = prerequisite;
		assignee = null;
		Completed = false;
		InProgress = false;
	}

	public static void UnAssignDuplicateTasks(CrewMember crewMember, Player player)
	{
		foreach(CrewMember member in player.crew)
		{
			if(crewMember.Name != member.Name && crewMember.Task == member.Task)
			{
				member.Task = null;
			}
		}
	}
}
