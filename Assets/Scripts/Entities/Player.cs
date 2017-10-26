using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity {

    private void Awake()
    {
        Attacks = new List<Attack>
        {
            new Attack("Basic Attack", 2, 3),
            new Attack("Super Attack", 4, 4)
        };
        Speed = 4;
        MaxHealth = 15;
        CurrentHealth = MaxHealth;
    }

    
	
	
}
