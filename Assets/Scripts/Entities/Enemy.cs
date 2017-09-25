using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Entity {

    /// <summary>
    /// The current AI state chosen, int values are priority with lower meaning 
    /// higher priority
    /// </summary>
    public enum State {
        Attacking,
        ConsumeConsumable,
        Sleep
    }

    //Actual score values for each state, general formula is:
    //assignmentScore = (totalStatesNum - enum value + modifier)
    int attackingScore, consumableScore;

    //TODO put these under entity as player will also need them most likely
    List<Attack> attacks;
    List<Consumable> consumables;

    private void Awake()
    {
        attacks = new List<Attack>
        {
            new Attack("Basic Attack", 2, 4)
        };

        consumables = new List<Consumable>
        {
            new HealingConsumable("Potion", 3)
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

        int distanceToTarget = Pathfinding.GetDistance(nodeParent, target.nodeParent);

        target.RefreshParent();

        Attack currentAttack = null;

        List<Attack> attacksInRange = new List<Attack>();
        foreach(Attack attack in attacks)
        {
            int value = attack.Range - distanceToTarget;

            if (canMove) { value += Speed; }

            if(value >= 0)
            {
                attacksInRange.Add(attack);
            }
        }
        
        //If there is an attack in range, increase score by X
        if(attacksInRange.Count > 0)
        {
            attackingScore += 5;
        }

        

      

        attackingScore = (attacks[0].Range -  + Speed);

        

        if (!canMove)
        {
            attackingScore -= Speed;
        }

        print("attacking score: " + attackingScore);
    }

}
