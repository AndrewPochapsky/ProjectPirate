using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consumable {
    
    public string Name { get; protected set; }

    public abstract void Consume(Entity target);

}
