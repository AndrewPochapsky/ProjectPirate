using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public EntityData data { get; set; }

    public List<Attack> Attacks { get; protected set; }
    public List<Consumable> Consumables { get; protected set; }

}
