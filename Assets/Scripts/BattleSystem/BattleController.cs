﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.EventSystems;

/*Battle rework idea:
    You can assign crew to have a job on the ship
    Possible Jobs:
        -Steering
        -Repairs
        -Cannons
    -How effective each action is depends on the skill level of the crew member
    -This wont change enemy class too much, just need to do some assigning the the start
    -Not a big deal since the same code will exist on the player to allow for auto assign
    -This will also possibly eliminate the need for consumables as it seems kinda hard to implement them
    at the moment

    -The whole idea is somewhat similar to FTL but is not realtime
    -Every turn you can assign crew to different tasks
    -AI will probably not do this but we'll see 

    -This rework also makes crew more valuable which is necessary since they dont do much at the moment
    -I dont think that this will be done by the first playable demo but should be the goal for the final game
*/

public class BattleController : MonoBehaviour {

    public delegate void OnTurnValueChanged(Turn turn);
    public event OnTurnValueChanged OnTurnValueChangedEvent;

    public delegate void OnPlayerInfoChanged(int? maxHealth = null, int? currentHealth = null, string canMove = null);
    public event OnPlayerInfoChanged OnPlayerInfoChangedEvent;

    public delegate void OnEnemyTurn(List<BattleEntity> targets);
    public event OnEnemyTurn OnEnemyTurnEvent;

    public delegate void OnBattleOver(BattleStatus status);
    public event OnBattleOver OnBattleOverEvent;

    public enum Turn { Player, Enemy }
    public enum BattleStatus { PlayerVic, EnemyVic, InProgress }

    public Turn CurrentTurn { get; private set; } = Turn.Player;

    BattleSystemUI uiController;

    [SerializeField]
    private Camera raycastCamera;

    const int battleTileSize = 50;

    const int width = 10;
    const int height = 10;

    List<BattleEntity> friendlies;

    List<BattleEnemy> enemies;

    Transform parent;

    List<Node> playerMovementRange;
    [HideInInspector]
    public Attack lastSelectedAttack;

    /// <summary>
    /// The nodes that make up the battle grid
    /// </summary>
    public List<Node> Nodes { get; private set; }

    /// <summary>
    /// Determines if path should be displayed
    /// </summary>
    bool canDisplayPathTiles = true;
    public bool Attacking { get; set; } = false;
    private bool canMove = true;
    private bool eventCalled = false;

    //Colours:
    Color pathColour, movementRangeColour;

    BattleData battleData;

    private void Awake()
    {
        //Doesnt require singleton patttern since this is the only time it is being accessed
        uiController = GetComponent<BattleSystemUI>();

        friendlies = new List<BattleEntity>();
        enemies = new List<BattleEnemy>();

        battleData = Resources.Load<BattleData>("Data/BattleData");

        //Colours:
        pathColour = Color.grey;
        movementRangeColour = new Color32(183, 183, 183, 1);

        //Generate the map
        parent = GameObject.FindGameObjectWithTag("BattleZone").transform;
        Nodes = TileGenerator.Instance.AddNodes(width, height, battleTileSize, nodeType: nameof(Node)).Select(n => n as Node).ToList();
        
        //Generate the actual tiles
        TileGenerator.Instance.GenerateTileMap("BattleOceanQuadTile", Nodes, removeNodes: false, tileSize: battleTileSize, parent: parent);

        //Generate the cubes underneath the tiles
        TileGenerator.Instance.GenerateTileMap("BattleOceanCubeTile", Nodes, removeNodes: false, tileSize: battleTileSize, parent: parent, isBase: true);

        Node playerStartingLocation = Nodes.Where(n => n.location.x == width - 1 && n.location.y == 0).Single();
        Node enemyStartingLocation = Nodes.Where(n => n.location.x == 0 && n.location.y == height - 1).Single();

        //TODO: choose the player(friendly) location in this loop so each location is different
        for(int i = 0; i < battleData.Friendlies.Count; i++)
        {
            friendlies.Add(SetupBattleEntity(nameof(BattlePlayer), playerStartingLocation.transform, battleData.Friendlies[i]));
        }
        
        //TODO: choose the enemy location in this loop so each location is different
        for (int i = 0; i < battleData.Enemies.Count; i++)
        {
            enemies.Add(SetupBattleEntity(nameof(BattleEnemy), enemyStartingLocation.transform, battleData.Enemies[i]) as BattleEnemy);
        }

        Pathfinding.OnPathUpdatedEvent += OnPathUpdated;

        //TODO: not sure if this line is necessary
        //friendlies[0].OnEndTurnEvent += OnEndTurn;

        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].OnEndTurnEvent += OnEndTurn;
            enemies[i].OnEntityDeathEvent += OnEntityDeath;
        }

        for (int i = 0; i < friendlies.Count; i++)
        {
            friendlies[i].OnEntityDeathEvent += OnEntityDeath;
        }

        playerMovementRange = Pathfinding.GetRange(Nodes, friendlies[0].nodeParent, friendlies[0].data.Speed);

        //Generate the UI
        uiController.CreateGrid(width, height, battleTileSize);
        uiController.GenerateAttackButtons(friendlies[0]);
    }

    private void Start()
    {
        OnTurnValueChangedEvent(CurrentTurn);
        OnPlayerInfoChangedEvent(friendlies[0].data.MaxHealth, friendlies[0].data.CurrentHealth, canMove.ToString());
        /*if(CurrentTurn == Turn.Enemy)
        {
            OnEnemyTurnEvent(friendlies);
        }*/
    }

    private void Update()
    {
        //TODO dont constantly raycast
        GameObject raycastObject = MouseRaycast(playerMovementRange);
        //TODO change this
        if(CurrentTurn == Turn.Player)
        {
            if(raycastObject != null)
            {
                if (!friendlies[0].IsMoving && !Attacking)
                {
                    List<Node> path = Pathfinding.FindPath(friendlies[0].nodeParent, GetTargetNode(raycastObject.GetComponent<Tile>()), reverse: true);
                    if (canDisplayPathTiles)
                    {
                        Pathfinding.SelectNodes(playerMovementRange, movementRangeColour);
                        Pathfinding.SelectNodes(path, pathColour);
                    }
                }

                else if (Attacking)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        BattleEntity entity = raycastObject.GetComponent<BattleEntity>();
                        //print("Attacking: " + entity.name);
                        Attack.AttackTarget(lastSelectedAttack, entity);
                        //End turn once attacked
                        canMove = true;
                        OnPlayerInfoChangedEvent(canMove: canMove.ToString());
                        OnEndTurn();
                    }
                }
                //Else if repairing do similar stuff as the above if 
                else if(true)
                {

                }
            }
           
        }
        //TODO do not do this
        else if(CurrentTurn==Turn.Enemy && !eventCalled)
        {
            eventCalled = true;
            OnEnemyTurnEvent(friendlies);
        }
    }

    /// <summary>
    /// Called when Pathfinding.OnPathUpdatedEvent is invoked
    /// </summary>
    /// <param name="nodes"></param>
    private void OnPathUpdated(List<Node> nodes)
    {
        BattleEntity entity = friendlies[0];
        if (!entity.IsMoving && canMove && Input.GetMouseButtonDown(0))
        {
            entity.SetPathNodes(nodes);
            Pathfinding.DeselectNodes(nodes);
            canDisplayPathTiles = false;
            canMove = false;
            OnPlayerInfoChangedEvent(canMove: canMove.ToString());
        }
    }

    /// <summary>
    /// Raycasts from mouse
    /// </summary>
    /// <returns>The tile</returns>
    private GameObject MouseRaycast(List<Node> movementRange)
    {
        Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
        
        Vector3 rayDirection = Input.mousePosition - raycastCamera.transform.position;
        //Ray ray = new Ray(raycastCamera.transform.position, rayDirection);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity);

        if(hit.collider != null && !EventSystem.current.IsPointerOverGameObject())
        {
            Tile tile = hit.collider.gameObject.GetComponent<Tile>();
            BattleEnemy enemy = hit.collider.gameObject.GetComponent<BattleEnemy>();

            //For movement
            if (tile != null && !Attacking && movementRange.Contains(GetTargetNode(tile)))
                return hit.collider.gameObject;
            //For attacking
            else if (enemy != null && Pathfinding.GetRange(Nodes, friendlies[0].nodeParent, lastSelectedAttack.Range).Contains(enemy.nodeParent))
                return hit.collider.gameObject;
        }
        return null;
        
    }

    /// <summary>
    /// Gets the node component from the specifed tile
    /// </summary>
    /// <param name="tile">The tile</param>
    /// <returns>The node component</returns>
    private Node GetTargetNode(Tile tile)
    {
        return tile.transform.GetComponentInParent<Node>();
    }

    /// <summary>
    /// Sets up an entity
    /// </summary>
    /// <param name="parent">The node parent</param>
    /// <returns>The created player</returns>
    private BattleEntity SetupBattleEntity(string type, Transform parent, EntityData data)
    {
        GameObject obj = Instantiate(Resources.Load(type), Vector3.zero, Quaternion.identity) as GameObject;
        obj.transform.localScale = new Vector3(battleTileSize, battleTileSize, battleTileSize);
        obj.transform.SetParent(parent);
        obj.transform.localPosition = new Vector3(0, battleTileSize / 2, 0);

        BattleEntity entity = obj.GetComponent<BattleEntity>();
        entity.data = data;
        entity.RefreshParent();

        return entity;
    }

    public void OnEndTurn()
    {
        if (CurrentTurn == Turn.Enemy)
        {
            foreach(BattleEnemy enemy in enemies)
            {
                enemy.ResetScores();
            }
            eventCalled = false;
            canDisplayPathTiles = true;
            canMove = true;

            playerMovementRange = Pathfinding.GetRange(Nodes, friendlies[0].nodeParent, friendlies[0].data.Speed);
            OnPlayerInfoChangedEvent(friendlies[0].data.MaxHealth, friendlies[0].data.CurrentHealth, canMove.ToString());
            CurrentTurn = Turn.Player;
        }
        else
        {
            CurrentTurn = Turn.Enemy;
            //This line is required or else a super weird bug happens
            canMove = false;
        }
        OnTurnValueChangedEvent(CurrentTurn);
    }

    private void OnEntityDeath(BattleEntity entity)
    {
        BattleEnemy enemy = entity as BattleEnemy;
        if(enemy != null)
        {
            enemies.Remove(enemy);
        }
        else
        {
            friendlies.Remove(entity);
        }
        Destroy(entity.gameObject);

        BattleStatus status = CheckBattleStatus();
        ProcessBattleStatus(status);
    }

    /// <summary>
    /// Checks if player or enemy lost/won or if battle in progress
    /// </summary>
    /// <returns>BattleStatus value depending on verdict</returns>
    private BattleStatus CheckBattleStatus()
    {
        if(friendlies.Count == 0)
        {
            return BattleStatus.EnemyVic;
        }
        else if(enemies.Count == 0)
        {
            return BattleStatus.PlayerVic;
        }

        return BattleStatus.InProgress;
    }

    private void ProcessBattleStatus(BattleStatus status)
    {
        if(status == BattleStatus.InProgress)
        {
            return; 
        }
        battleData.InfamyReward = Random.Range(10, 29);

        if(status == BattleStatus.EnemyVic)
            battleData.InfamyReward *= -1;

        if(status == BattleStatus.EnemyVic || status == BattleStatus.PlayerVic)
        {
            //uiController.fadeOut = true;
            friendlies = new List<BattleEntity>();
            enemies = new List<BattleEnemy>();
            Pathfinding.OnPathUpdatedEvent -= OnPathUpdated;
            //only do this if player won
            //battleData.enemyObject.GetComponent<Enemy>().dead = true;
            OnBattleOverEvent(status);
        }
    }

}
