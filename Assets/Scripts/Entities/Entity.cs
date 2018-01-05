using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    /// <summary>
    /// The infamy tiers. Each coorresponding number is now much infamy is required to reach
    /// the next tier. For example to get from the first to second it takes X Infamy, from second
    /// to third its Y infamy, and from third to fourth its Z Infamy 
    /// </summary>
    public enum InfamyTier
    {
        Level1 = 0, 
        Level2 = 50, //X
        Level3 = 200, //Y
        Level4 = 500, //Z
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

    public static InfamyTier LevelUp(EntityData data)
    {
        InfamyTier nextTier = GetNextTier(data.Tier);
        if(nextTier == InfamyTier.Null)
        {
            return InfamyTier.Null;
        }
        else
        {
            data.Tier = nextTier;
            return nextTier; 
        }
    }
    


}
