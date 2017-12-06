using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : ISellable{
	public string Name { get; protected set; }
	public int MinQuantity { get; protected set; }
	public int MaxQuantity { get; protected set; }
	public int Amount { get; set; }
	public int SellPrice { get; set; }

	[Newtonsoft.Json.JsonConstructor]
    public Resource(string name, int minQuantity, int maxQuantity, int sellPrice)
	{
		Name = name;
		MinQuantity = minQuantity;
		MaxQuantity = maxQuantity;
		SellPrice = sellPrice;

		Amount = Random.Range(minQuantity, MaxQuantity + 1);
	}
}
