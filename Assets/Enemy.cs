using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity {

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		data = new EntityData();
		data.Speed = 3;
        data.MaxHealth = 1;
        data.CurrentHealth = data.MaxHealth;

        data.Attacks = new List<Attack>
        {
            new Attack("Basic Attack", 2, 4),
            new Attack("Super Attack", 3, 4)
        };

        data.Consumables = new List<Consumable>
        {
            new HealingConsumable("Basic Potion", 3),
            new HealingConsumable("Super Potion", 6)
        };
	}
}
