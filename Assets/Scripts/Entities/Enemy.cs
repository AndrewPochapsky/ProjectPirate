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
            new Attack("Basic Attack", 2, 4)
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

    private void OnEnemyTurn(List<Entity> targets)
    {
        Attack attack = DetermineAttackScore(targets);
        state = GetState();
        //TODO only execute on attack state
        if (attack != null && move)
        {
            Node nodeToMoveTo = null;
            print("Speed: " + Speed);
            print("Range: " + attack.Range);
            List<Node> _nodes = Pathfinding.GetRange(battleController.Nodes, nodeParent, (Speed));
            //

            /*foreach(Node n in nodes)
            {
                n.Child.Select();
            }*/

            int minDistance = int.MaxValue;
            for(int i = 0; i < _nodes.Count; i++)
            {
                if (nodeParent != _nodes[i])
                {
                    List<Node> range = Pathfinding.GetRange(battleController.Nodes, _nodes[i], attack.Range);

                    int currentDistance = (Pathfinding.GetDistance(targets[0].nodeParent, _nodes[i]));
                    if (currentDistance < minDistance)
                    {
                        minDistance = currentDistance;
                        nodeToMoveTo = _nodes[i];
                    }
                }
              
            }
            //nodeToMoveTo.Child.Select();
            
            List<Node> path = Pathfinding.FindPath(nodeParent, nodeToMoveTo);
            for(int i = 0; i < path.Count; i++)
            {
                List<Node> range = Pathfinding.GetRange(battleController.Nodes, path[i], attack.Range);
                if (range.Contains(targets[0].nodeParent))
                {
                    path = Pathfinding.FindPath(nodeParent, path[i]);
                    break;
                }
            }
            //print(nodeToMoveTo.location);
           
            //Displays attack range from the new location of enemy
            /*foreach (Node n in Pathfinding.GetRange(battleController.Nodes, path[path.Count - 1], attack.Range))
            {
                n.Child.Select(Color.black);
            }*/
            foreach (Node node in path)
            {
                node.Child.Select(Color.gray);
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

        List<Attack> attacksInReadyRange = new List<Attack>();
        List<Attack> attacksInMovementRange = new List<Attack>();

        foreach (Attack attack in attacks)
        {
            int valueOne, valueTwo; 
            valueOne = valueTwo = attack.Range - distanceToTarget;

            if (canMove)
                valueTwo += Speed;

            //TODO find a better way to incorporate speed
            //if (canMove) { value += Speed; move = true; }

            //Attack is able to attack
            if (valueOne >= 0)
            {
                attacksInReadyRange.Add(attack);
                print("attack in ready range");
                attackingScore += 10;
                move = false;
            }
            else if(valueTwo >= 0)
            {
                attacksInMovementRange.Add(attack);
                print("attack in movement range");
                attackingScore += Speed;
                move = true;
            }
        }

        if (attacksInReadyRange.Count > 0)
            currentAttack = attacksInReadyRange[0];
        else if (attacksInMovementRange.Count > 0)
            currentAttack = attacksInMovementRange[0];

        //Get the attack with heighest damage
        //TODO re-enable this
        //TODO factor in other values such as armour piercing
        /*for (int i = 1; i < attacksInReadyRange.Count; i++)
        {
            if (attacksInReadyRange[i].Damage > currentAttack.Damage)
            {
                currentAttack = attacksInReadyRange[i];
            }
        }*/

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
