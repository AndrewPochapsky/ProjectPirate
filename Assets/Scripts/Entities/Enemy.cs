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

    BattleController battleController;

    private void Awake()
    {
       

       
    }

    protected override void Start()
    {
        attacks = new List<Attack>
        {
            new Attack("Basic Attack", 2, 3)
        };

        consumables = new List<Consumable>
        {
            new HealingConsumable("Potion", 3)
        };

        base.Start();
        battleController = FindObjectOfType<BattleController>();
        if (battleController != null)
        {
            battleController.OnEnemyTurnEvent += OnEnemyTurn;
        }
    }
    bool done = false;
    private void OnEnemyTurn(List<Entity> targets)
    {
        Attack attack = DetermineAttackScore(targets);
        state = GetState();
        //TODO only execute on attack state
        if (move)
        {
            Node nodeToMoveTo = null;
            List<Node> nodes = Pathfinding.GetRange(battleController.Nodes, nodeParent, (Speed - attack.Range));

            /*foreach(Node n in nodes)
            {
                n.Child.Select();
            }*/

            int minDistance = Pathfinding.GetDistance(targets[0].nodeParent, nodes[0]);
            for(int i = 1; i < nodes.Count; i++)
            {
                if(nodeParent != nodes[i])
                {
                    int currentDistance = Pathfinding.GetDistance(targets[0].nodeParent, nodes[i]);
                    if (currentDistance < minDistance)
                    {
                        minDistance = currentDistance;
                        nodeToMoveTo = nodes[i];
                    }
                }
              
            }
            //nodeToMoveTo.Child.Select();
            Pathfinding.FindPath(nodeParent, nodeToMoveTo);
            done = true;
            print(nodeToMoveTo.location);
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
            if (canMove)
                value += Speed;

            //TODO find a better way to incorporate speed
            //if (canMove) { value += Speed; move = true; }

            if (value >= 0)
            {
                attacksInRange.Add(attack);
            }
        }

        //If there is an attack in range, increase score by X
        if (attacksInRange.Count > 0)
        {
            attackingScore += Speed;
            move = true;
        }

        if(attacksInRange.Count > 0)
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
        if (currentAttack != null && target.CurrentHealth <= currentAttack.Damage)
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
