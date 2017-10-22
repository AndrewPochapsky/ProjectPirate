using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleEnemy : Enemy {

    private void Awake()
    {
        Speed = 5;
        MaxHealth = 10;
        CurrentHealth = MaxHealth / 2;

        Attacks = new List<Attack>
        {
            new Attack("Basic Attack", 2, 4)
        };

        Consumables = new List<Consumable>
        {
            new HealingConsumable("Basic Potion", 3),
            new HealingConsumable("Super Potion", 6)
        };
    }
}
