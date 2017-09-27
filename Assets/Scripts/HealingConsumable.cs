﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingConsumable : Consumable
{
    public int HealingValue { get; protected set; }

    public HealingConsumable(string name, int healingValue)
    {
        Name = name;
        HealingValue = healingValue;
    }

    public override void Consume(Entity target)
    {
        target.ModifyHealth(HealingValue);
    }
}
