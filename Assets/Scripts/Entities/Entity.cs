using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    /// <summary>
    /// Number of grid spaces that an enemy can move
    /// </summary>
    public int Speed { get; protected set; }
    public int MaxHealth { get; protected set; }
    public int CurrentHealth { get; protected set; }
    public int Infamy { get; protected set; }
    public int Gold { get; protected set; }

    public List<Attack> Attacks { get; protected set; }
    public List<Consumable> Consumables { get; protected set; }

    public void ModifyHealth(int value)
    {
        CurrentHealth += value;
        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;
    }
}
