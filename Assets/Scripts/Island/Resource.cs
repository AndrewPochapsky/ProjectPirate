using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : ISellable{
	public string Name { get; protected set; }
	public int MinQuantity { get; protected set; }
	public int MaxQuantity { get; protected set; }
	public int Amount { get; set; }
	public int SellPrice { get; protected set; }

	[Newtonsoft.Json.JsonConstructor]
    public Resource(string name, int minQuantity, int maxQuantity, int sellPrice)
	{
		Name = name;
		MinQuantity = minQuantity;
		MaxQuantity = maxQuantity;
		SellPrice = sellPrice;
	}

	/// <summary>
	/// Create a new resource based an an existing resource's name
	/// </summary>
	/// <param name="resource">The existing resource</param>
	public Resource(Resource resource)
	{
		Name = resource.Name;
		SellPrice = resource.SellPrice;
	}
}
