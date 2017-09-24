using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack {

	public string Name { get; private set; }
    public int Damage { get; private set; }
    public int Range { get; private set; }

    public Attack(string name, int damage, int range)
    {
        Name = name;
        Damage = damage;
        Range = range;
    }
}
