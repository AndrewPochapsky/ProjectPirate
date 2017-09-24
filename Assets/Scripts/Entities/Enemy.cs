using System.Collections;
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

    List<Attack> attacks;

    private void Awake()
    {
        attacks = new List<Attack>
        {
            new Attack("Basic Attack", 2, 4)
        };
    }

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
    public void DetermineScores(List<Entity> targets)
    {
        bool move = false;

        Entity target = null;
        this.RefreshParent();
        if (targets.Count == 1)
        {
            target = targets[0];
        }
        target.RefreshParent();

        int attackingModifier = (attacks[0].Range - Pathfinding.GetDistance(nodeParent, target.nodeParent) + Speed);
        if (!canMove)
        {
            attackingModifier -= Speed;
        }

        print("attacking modifier: " + attackingModifier);
    }

}
