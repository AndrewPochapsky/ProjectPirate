using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityData {
	public int Speed { get; protected set; }
    public int MaxHealth { get; protected set; }
    public int CurrentHealth { get; protected set; }
    public int Infamy { get; protected set; }
    public int Gold { get; protected set; }

    public List<Attack> Attacks { get; protected set; }
    public List<Consumable> Consumables { get; protected set; }
}
