using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consumable : ISellable{
    
    public string Name { get; protected set; }
    public int SellPrice { get; set; }

    public abstract void Consume(Entity target);

}
