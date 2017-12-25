using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : BattleEntity {

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
    int attackingScore, consumableScore = -1, moveScore;

    bool moveForAttack = true;
    bool checkForLocationReached = false;
    BattleController battleController;

    Node node = null;
    Attack currentAttack = null;
    BattleEntity target = null;

    protected override void Start()
    {
        base.Start();
        battleController = FindObjectOfType<BattleController>();
        if (battleController != null)
        {
            battleController.OnEnemyTurnEvent += OnEnemyTurn;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (checkForLocationReached)
        {
            if (nodeParent == node)
            {
                node = null;
                checkForLocationReached = false;
                canMove = false;

                //If move was initiated as part of attacking
                if (state == State.Attack)
                {
                    Attack.AttackTarget(currentAttack, target);
                    currentAttack = null;
                    target = null;
                    RaiseEndTurnEvent();
                }
                else
                {
                    ResetScores();
                    //Do another action
                    print("Recursion");
                    OnEnemyTurn(new List<BattleEntity>() { target });
                }
               
            }
        }
    }

    /// <summary>
    /// Logic to be executed on enemy turn
    /// </summary>
    /// <param name="targets">List of possible targets</param>
    private void OnEnemyTurn(List<BattleEntity> targets)
    {
        target = GetTarget(targets);

        currentAttack = DetermineAttackScore(target);
        Consumable consumable = DetermineConsumableScore();
        node = DetermineMoveScore(target, canAttack: currentAttack != null, canConsume: consumable != null);
        
        print("A-score: " + attackingScore + " C-score: "+ consumableScore + " M-Score: "+moveScore);

        //If enemy cant do anything just end turn
        if(currentAttack == null && consumable == null && !canMove)
        {
            RaiseEndTurnEvent();
        }

        state = GetState();

        switch (state)
        {
            case State.Attack:
                //print("Attacking...");
                if (currentAttack!= null && moveForAttack)//TODO remove the first condition
                {
                    Node nodeToMoveTo = null;
                    List<Node> movementRangeNodes = Pathfinding.GetRange(battleController.Nodes, nodeParent, (Speed));

                    int minDistance = int.MaxValue;
                    for (int i = 0; i < movementRangeNodes.Count; i++)
                    {
                        if (nodeParent != movementRangeNodes[i] && target.nodeParent != movementRangeNodes[i])
                        {
                            List<Node> attackRangeNodes = Pathfinding.GetRange(battleController.Nodes, movementRangeNodes[i], currentAttack.Range);

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
                        if (distance <= currentAttack.Range)
                        {
                            path = Pathfinding.FindPath(nodeParent, path[i], reverse: true);
                            break;
                        }
                    }
                    SetPathNodes(path);
                    node = path[path.Count - 1];
                    checkForLocationReached = true;

                }
                else if(currentAttack != null)
                {
                    Attack.AttackTarget(currentAttack, target);
                    RaiseEndTurnEvent();
                }
                break;

            case State.Consumable:
                //print("consuming");
                consumable.Consume(this);
                print("Consuming "+consumable.Name);
                //TODO allow selecting other targets(party members)
                RaiseEndTurnEvent();
                break;

            case State.Move:
                //print("Moving...");
                List<Node> _path = Pathfinding.FindPath(nodeParent, node, reverse: true);

                SetPathNodes(_path);
                node = _path[_path.Count - 1];
                checkForLocationReached = true;
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
        else if (consumableScore > attackingScore && consumableScore > moveScore)
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
    public BattleEntity GetTarget(List<BattleEntity> targets)
    {
        if(targets.Count == 1)//TODO choose targets when there are more than one
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
    private Attack DetermineAttackScore(BattleEntity target)
    {
        this.RefreshParent();
        int distanceToTarget = Pathfinding.GetDistance(nodeParent, target.nodeParent);

        target.RefreshParent();

        Attack currentAttack = null;

        List<Attack> allAttacks = new List<Attack>();
        List<Attack> attacksInReadyRange = new List<Attack>();
        List<Attack> attacksInMovementRange = new List<Attack>();

        foreach (Attack attack in Attacks)
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
                moveForAttack = false;
            }
            //Entity can attack if moves
            else if(valueTwo >= 0)
            {
                allAttacks.Add(attack);
                attacksInMovementRange.Add(attack);
                attackingScore += Speed;
                moveForAttack = true;
            }
        }

        if (attacksInReadyRange.Count > 0)
            currentAttack = attacksInReadyRange[0];
        else if (attacksInMovementRange.Count > 0)
            currentAttack = attacksInMovementRange[0];

        //Get the attack with highest damage
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
            print("Setting attack score to max");
            attackingScore = 999;
        }
        else if(currentAttack != null)
        {
            attackingScore = 50;
        }

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
        List<HealingConsumable> healingConsumables = Consumables.Where(c => c is HealingConsumable)
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
            print("Healing score: "+ healingScore + " health diff: "+healthDifference);
            //Potion would replenish all health
            if(selectedHealingConsumable.HealingValue == healthDifference)
            {
                consumableScore += 75;
            }
            else if(selectedHealingConsumable.HealingValue < healthDifference )
            {
                consumableScore += 150;
            }
           
            //else if(healingScore < healthDifference && healthDiffernce > )
            //TODO change this
            selectedAbsoluteConsumable = selectedHealingConsumable;
        }

        return selectedAbsoluteConsumable;
    }

    private Node DetermineMoveScore(BattleEntity target, bool canAttack, bool canConsume)
    {
        List<Node> movementRange = Pathfinding.GetRange(battleController.Nodes, nodeParent, Speed);

        Node node = null;
        if (!canMove)
        {
            moveScore = -1000;
            return null;
        }

        //IF cant attack or consume, then moving is the only option
        if(!canAttack && !canConsume)
        {
            print("cant attack or comsume");
            moveScore = 1000;
        }
        //TODO do stuff if health is slow aka RETREAT
        //if health low
        //Move as far as you can away
        //Use healing consumable

       
        int minDistance = int.MinValue; 
        if(CurrentHealth <= MaxHealth/2 && canConsume)
        {
            //Find node farthest from target but within range
            foreach(Node _node in movementRange)
            {
                int distance = Pathfinding.GetDistance(_node, target.nodeParent);
                if(distance > minDistance)
                {
                    node = _node;
                    minDistance = distance;
                }
            }
            moveScore += 200;
        }

        //Get highest range attack
        Attack highestRangeAttack = Attacks[0];
        for(int i = 1; i < Attacks.Count; i++)
        {
            if(Attacks[i].Range > highestRangeAttack.Range)
            {
                highestRangeAttack = Attacks[i];
            }
        }

        
       if(node == null)
       {
            //Get closest node that is within the range
            foreach (Node _node in movementRange)
            {
                int distance = Pathfinding.GetDistance(_node, target.nodeParent);
                if(distance < minDistance && distance >= highestRangeAttack.Range)
                {
                    node = _node;
                    minDistance = distance;
                }
            }
       }
       

        minDistance = int.MaxValue;

        if(node == null)
        {
            foreach(Node _node in movementRange)
            {
                int distance = Pathfinding.GetDistance(_node, target.nodeParent);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    node = _node;
                }
            }
        }
        moveScore += 20;

        return node;
    }


    /// <summary>
    /// Resets the scores
    /// </summary>
    public void ResetScores()
    {
        attackingScore = consumableScore = moveScore = 0;
    }

}
