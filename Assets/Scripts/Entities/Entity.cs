using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public enum InfamyTier
    {
        Level1 = 0,
        Level2 = 50,
        Level3 = 200,
        Level4 = 500,
        Level5 = 1000,
        Null
    }

    public EntityData entityData { get; set; }


    public static InfamyTier GetNextTier(InfamyTier currentTier)
    {
        var values = Enum.GetValues(typeof(InfamyTier)).Cast<InfamyTier>();

        foreach(var value in values)
        {
            if((int)value > (int)currentTier)
            {
                return value;
            }
        }
        return InfamyTier.Null;
    }
    


}
