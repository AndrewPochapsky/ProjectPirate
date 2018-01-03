using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity {

    public bool dead = false;

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		entityData = new EntityData();
		entityData.Speed = 3;
        entityData.MaxHealth = 1;
        entityData.CurrentHealth = entityData.MaxHealth;

        entityData.Attacks = new List<Attack>
        {
            new Attack("Basic Attack", 2, 4),
            new Attack("Super Attack", 3, 4)
        };

        entityData.Consumables = new List<Consumable>
        {
            new HealingConsumable("Basic Potion", 3),
            new HealingConsumable("Super Potion", 6)
        };
	}
}
