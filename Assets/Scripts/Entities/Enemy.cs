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
        Attack,
        Consumable,
        Sleep
    }

    State state;

    //Actual score values for each state, general formula is:
    //assignmentScore = (totalStatesNum - enum value + modifier)
    int attackingScore, consumableScore;

    //TODO put these under entity as player will also need them most likely
    List<Attack> attacks;
    List<Consumable> consumables;

    bool move = true;

    private void Awake()
    {
       

       
    }

    protected override void Start()
    {
        attacks = new List<Attack>
        {
            new Attack("Basic Attack", 2, 4)
        };

        consumables = new List<Consumable>
        {
            new HealingConsumable("Potion", 3)
        };

        base.Start();
        BattleController battleController = FindObjectOfType<BattleController>();
        if (battleController != null)
        {
            battleController.OnEnemyTurnEvent += OnEnemyTurn;
        }
    }

    private void OnEnemyTurn(List<Entity> targets)
    {
        Attack attack = DetermineAttackScore(targets);
        state = GetState();
        if (move)
        {
            Node node = Pathfinding.GetClosestNode(this.nodeParent, targets[0].nodeParent, this.Speed);
            if (node != null)
            {
                node.Child.Select();
                print(node.location);
            }
           
        }

       /* switch (state)
        {
            case State.Attack:
                break;

            case State.Consumable:
                break;
        }*/
    }

    /// <summary>
    /// Return the state for the turn.
    /// </summary>
    /// <returns></returns>
    public State GetState()
    {
        
        return State.Attack;
    }

    /// <summary>
    /// Sets all of the score values, formula:
    /// assignmentScore = (totalStatesNum - enum value + modifier)
    /// </summary>
    /// <param name="targets">The list of possible targets</param>
    public void DetermineScores(List<Entity> targets)
    {
        //Reset scores from last turn
        attackingScore = 0;
        consumableScore = 0;
    }

    private Attack DetermineAttackScore(List<Entity> targets)
    {
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
        foreach (Attack attack in attacks)
        {
            int value = attack.Range - distanceToTarget;

            if (canMove) { value += Speed; move = true; }

            if (value >= 0)
            {
                attacksInRange.Add(attack);
            }
        }

        //If there is an attack in range, increase score by X
        if (attacksInRange.Count > 0)
        {
            attackingScore += 5;
        }

        currentAttack = attacksInRange[0];

        //Get the attack with heighest damage
        //TODO factor in other values such as armour piercing
        for (int i = 1; i < attacksInRange.Count; i++)
        {
            if (attacksInRange[i].Damage > currentAttack.Damage)
            {
                currentAttack = attacksInRange[i];
            }
        }

        //target can be killed in one hit
        if (target.CurrentHealth <= currentAttack.Damage)
        {
            attackingScore += 999;
        }

        //TODO create method called ChooseAttack which takes in attacksInRange List
        return currentAttack;
    }


    private void AttackTarget(Attack attack, Entity target)
    {
        target.ModifyHealth((-1) * attack.Damage);
    }

}
