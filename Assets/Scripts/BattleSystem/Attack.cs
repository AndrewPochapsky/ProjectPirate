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

    /// <summary>
    /// Deal attack's damage to target entity
    /// </summary>
    /// <param name="attack">The specified attack</param>
    /// <param name="target">The target</param>
    public static void AttackTarget(Attack attack, BattleEntity target)
    {
        target.ModifyHealth((-1) * attack.Damage);
    }
}
