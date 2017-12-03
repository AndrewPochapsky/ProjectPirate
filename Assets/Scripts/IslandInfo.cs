using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandInfo  {
	public string Name { get; protected set; }
    public bool Visited { get; set; }
	public List<Interaction> Interactions { get; private set; }

	public IslandInfo()
	{
		 Name = DataGenerator.Instance.GenerateIslandName(Random.Range(2, 4));
		 Interactions = InteractionManager.Instance.GetInteractionList();

	}
}
