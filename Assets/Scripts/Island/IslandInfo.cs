﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandInfo  {
	public string Name { get; protected set; }
    public bool Visited { get; set; }
	public List<Interaction> Interactions { get; private set; }
	public List<Resource> Resources { get; private set; }

	public IslandInfo()
	{
		Name = DataGenerator.Instance.GenerateIslandName(Random.Range(2, 4));
		Interactions = InteractionManager.Instance.GetInteractionList();
		Resources = ResourceManager.Instance.GetResourcesList(amount: 3);
	}

	public string FormattedResourceList()
	{
		string s = string.Empty;

		for(int i = 0; i < Resources.Count; i++)
		{
			Resource r = Resources[i];
				 
			if (r.Amount > 0)
				s += r.Name + "(" + r.Amount + ")";

			if(i + 1 != Resources.Count && r.Amount > 0)
			{
				s += ", ";
			}
		}

		if(s == ", ")
			return "All resources depleted";

		return s;
	}
}
