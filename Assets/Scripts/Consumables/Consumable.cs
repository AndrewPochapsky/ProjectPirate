using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consumable : ISellable{
    
    public string Name { get; protected set; }
    public int Amount { get; set; }
    public int SellPrice { get; protected set; }

    public abstract void Consume(BattleEntity target);

}
