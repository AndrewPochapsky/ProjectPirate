using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Entity {

    /// <summary>
    /// AI states
    /// </summary>
    public enum State {
        Attack,
        Consumable,
        Move
    }

    State state;

    //Actual score values for each state, general formula is:
    //assignmentScore = (totalStatesNum - enum value + modifier)
    int attackingScore, consumableScore, moveScore;

    bool move = true;

    BattleController battleController;

    protected override void Start()
    {
        base.Start();
        battleController = FindObjectOfType<BattleController>();
        if (battleController != null)
        {
            battleController.OnEnemyTurnEvent += OnEnemyTurn;
        }
    }

    /// <summary>
    /// Logic to be executed on enemy turn
    /// </summary>
    /// <param name="targets">List of possible targets</param>
    private void OnEnemyTurn(List<Entity> targets)
    {
        Entity target = GetTarget(targets);
        Attack attack = DetermineAttackScore(target);
        Consumable consumable = DetermineConsumableScore();

        DetermineMoveScore(canAttack: attack != null, canConsume: consumable != null);

        state = GetState();
        
        switch (state)
        {
            case State.Attack:
                if (attack!= null && move)//TODO remove the first condition
                {
                    Node nodeToMoveTo = null;
                    List<Node> movementRangeNodes = Pathfinding.GetRange(battleController.Nodes, nodeParent, (Speed));

                    int minDistance = int.MaxValue;
                    for (int i = 0; i < movementRangeNodes.Count; i++)
                    {
                        if (nodeParent != movementRangeNodes[i] && target.nodeParent != movementRangeNodes[i])
                        {
                            List<Node> attackRangeNodes = Pathfinding.GetRange(battleController.Nodes, movementRangeNodes[i], attack.Range);

                            int currentDistance = Pathfinding.GetDistance(target.nodeParent, movementRangeNodes[i]);
                            if (currentDistance < minDistance)
                            {
                                minDistance = currentDistance;
                                nodeToMoveTo = movementRangeNodes[i];
                            }
                        }
                    }

                    List<Node> path = Pathfinding.FindPath(nodeParent, nodeToMoveTo, reverse: true);
                    for (int i = 0; i < path.Count; i++)
                    {
                        int distance = Pathfinding.GetDistance(path[i], target.nodeParent);
                        if (distance <= attack.Range)
                        {
                            path = Pathfinding.FindPath(nodeParent, path[i], reverse: true);
                            break;
                        }
                    }

                    if (nodeParent == path[path.Count-1])
                    {
                        //Do action and end turn
                        RaiseEndTurnEvent();
                    }
                    if(pathNodes.Count == 0)
                    {
                        SetPathNodes(path);
                    }
                    
                }
                else if(attack != null)
                {
                    print("Attacking");
                    RaiseEndTurnEvent();
                }
                break;

            case State.Consumable:
                print("consuming");
                //TODO allow selecting other targets(party members)
                RaiseEndTurnEvent();//TOD remove this, it is here temp. so game is playable
                //consumable.Consume(target: this);
                break;

            case State.Move:
                print("Moving...");
                //TODO write logic for moving
                //Find path to target node(selected by DetermineMoveScore()
                //Check if it has reached the target node
                //If so, send EndTurn event
                //Refer to Case State.Attack: ...
                RaiseEndTurnEvent();
                break;
        }
    }

    /// <summary>
    /// Return the state for the turn.
    /// </summary>
    /// <returns></returns>
    public State GetState()
    {
        if(attackingScore > consumableScore && attackingScore > moveScore)
        {
            return State.Attack;
        }
        else if (attackingScore < consumableScore && consumableScore < moveScore)
        {
            return State.Consumable;
        }
        return State.Move;
    }

    /// <summary>
    /// Gets target entity from list of entities
    /// </summary>
    /// <param name="targets">List of entities</param>
    /// <returns>The chosen target</returns>
    public Entity GetTarget(List<Entity> targets)
    {
        if(targets.Count == 1)
        {
            return targets[0];
        }
        return null;
    }

    /// <summary>
    /// Determines the attack score
    /// </summary>
    /// <param name="target">The target</param>
    /// <returns>The chosen attack</returns>
    private Attack DetermineAttackScore(Entity target)
    {
        this.RefreshParent();
        int distanceToTarget = Pathfinding.GetDistance(nodeParent, target.nodeParent);

        target.RefreshParent();

        Attack currentAttack = null;

        List<Attack> allAttacks = new List<Attack>();
        List<Attack> attacksInReadyRange = new List<Attack>();
        List<Attack> attacksInMovementRange = new List<Attack>();

        foreach (Attack attack in attacks)
        {
            int valueOne, valueTwo; 
            valueOne = valueTwo = (attack.Range - distanceToTarget);

            if (canMove)
                valueTwo += Speed;

            //entity is able to attack
            if (valueOne >= 0)
            {
                allAttacks.Add(attack);
                attacksInReadyRange.Add(attack);
                attackingScore += 10;
                move = false;
            }
            //Entity can attack if moves
            else if(valueTwo >= 0)
            {
                allAttacks.Add(attack);
                attacksInMovementRange.Add(attack);
                attackingScore += Speed;
                move = true;
            }
        }

        if (attacksInReadyRange.Count > 0)
            currentAttack = attacksInReadyRange[0];
        else if (attacksInMovementRange.Count > 0)
            currentAttack = attacksInMovementRange[0];

       
        //Get the attack with heighest damage
        //TODO factor in other values such as armour piercing
        for (int i = 1; i < allAttacks.Count; i++)
        {
            if (allAttacks[i].Damage > currentAttack.Damage)
            {
                currentAttack = allAttacks[i];
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

    private Consumable DetermineConsumableScore()
    {
        //Get all types of consumables 
        //Check if a specific type exists, if so then increase points
        //For example, if need health and contains HealingConsumable, increase points and set that to selectedConsumable
        //Store all types in list, List<HealingConsumable>...

        Consumable selectedAbsoluteConsumable = null;
        HealingConsumable selectedHealingConsumable = null;
        //Seperate all current consumables into list
        List<HealingConsumable> healingConsumables = consumables.Where(c => c is HealingConsumable)
                                                                .Select(c => c as HealingConsumable)
                                                                .ToList();
        int healingScore = -1;
        if (MaxHealth != CurrentHealth && healingConsumables.Count > 0)
        {
            selectedHealingConsumable = healingConsumables[0];
            healingScore = (MaxHealth - CurrentHealth) + selectedHealingConsumable.HealingValue;
            for (int i = 1; i < healingConsumables.Count; i++)
            {
                int _value = (MaxHealth - CurrentHealth) + healingConsumables[i].HealingValue;
                if (_value < healingScore)
                {
                    healingScore = _value;
                    selectedHealingConsumable = healingConsumables[i];
                }
            }
        }

        if(selectedHealingConsumable != null)
        {
            int healthDifference = MaxHealth - CurrentHealth;
            //Potion would replenish all health
            if(healingScore == healthDifference)
            {
                consumableScore = 900;
            }
            //else if(healingScore < healthDifference && healthDiffernce > )
            //TODO change this
            selectedAbsoluteConsumable = selectedHealingConsumable;
        }

        return selectedAbsoluteConsumable;
    }

    private void DetermineMoveScore(bool canAttack, bool canConsume)
    {
        //IF cant attack or consume, then moving is the only option
        if(!canAttack && !canConsume)
        {
            moveScore = 1000;
        }
    }

    /// <summary>
    /// Deal attack's damage to target entity
    /// </summary>
    /// <param name="attack">The specified attack</param>
    /// <param name="target">The target</param>
    private void AttackTarget(Attack attack, Entity target)
    {
        target.ModifyHealth((-1) * attack.Damage);
    }

}
