using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleEnemy : Enemy {

    private void Awake()
    {
        Speed = 1;
        MaxHealth = 10;
        CurrentHealth = MaxHealth;

        Attacks = new List<Attack>
        {
            new Attack("Basic Attack", 2, 1),
            //new Attack("Super Attack", 3, 4)
        };

        Consumables = new List<Consumable>
        {
            new HealingConsumable("Basic Potion", 3),
            new HealingConsumable("Super Potion", 6)
        };
    }
}
