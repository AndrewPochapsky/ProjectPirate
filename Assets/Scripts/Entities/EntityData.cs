using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityData {

	public int Speed { get; set; }
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int Infamy { get; set; }
    public int Gold { get; set; }
    public Entity.InfamyTier Tier { get; set; }

    public List<Attack> Attacks { get; set; }
    public List<Consumable> Consumables { get; set; }
    
    //For the player:
    public List<ISellable> Inventory { get; set; }
    public List<CrewMember> Crew { get; set; }

    public EntityData()
    {
        Attacks = new List<Attack>();
        Consumables = new List<Consumable>();
        Crew = new List<CrewMember>();
        Inventory = new List<ISellable>();
    }

    public EntityData(int speed, int maxHealth, int infamy, int gold)
    {
        Speed = speed;
        MaxHealth = maxHealth;
        CurrentHealth = CurrentHealth;
        Infamy = infamy;
        Gold = gold;

        Attacks = new List<Attack>();
        Consumables = new List<Consumable>();
        Crew = new List<CrewMember>();
        Inventory = new List<ISellable>();
    }
}
