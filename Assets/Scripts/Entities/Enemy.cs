﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity {

    /// <summary>
    /// The current AI state chosen, int values are priority with lower meaning 
    /// higher priority
    /// </summary>
    public enum State {
        Attacking = 2,
        Healing = 1,
        Sleep
    }

    //Actual score values for each state, general formula is:
    //assignmentScore = (totalStatesNum - enum value + modifier)
    int attackingScore, healingScore;

    /// <summary>
    /// Return the state for the turn.
    /// </summary>
    /// <returns></returns>
    public State GetState()
    {
        return State.Sleep;
    }

    /// <summary>
    /// Sets all of the score values, formula:
    /// assignmentScore = (totalStatesNum - enum value + modifier)
    /// </summary>
    /// <param name="targets">The list of possible targets</param>
    private void DetermineScores(List<Entity> targets)
    {
        Entity target = null;
        if (targets.Count == 1)
        {
            target = targets[0];
        }
    }

}
