using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleEnemy : Enemy {

    private void Awake()
    {
        Speed = 5;
        MaxHealth = 10;
        CurrentHealth = MaxHealth / 2;
    }
}
