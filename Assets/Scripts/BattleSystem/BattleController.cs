using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.EventSystems;

public class BattleController : MonoBehaviour {

    public delegate void OnTurnValueChanged(Turn turn);
    public event OnTurnValueChanged OnTurnValueChangedEvent;

    public delegate void OnPlayerInfoChanged(int? maxHealth = null, int? currentHealth = null, string canMove = null);
    public event OnPlayerInfoChanged OnPlayerInfoChangedEvent;

    public delegate void OnEnemyTurn(List<Entity> targets);
    public event OnEnemyTurn OnEnemyTurnEvent;

    public enum Turn { Player, Enemy }
    public Turn CurrentTurn { get; private set; } = Turn.Player;

    TileGenerator tileGenerator;
    BattleSystemUI uiController;

    [SerializeField]
    private Camera raycastCamera;

    const int battleTileSize = 50;

    const int width = 10;
    const int height = 10;

    List<Entity> friendlies;

    List<Enemy> enemies;

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

    private void Awake()
    {
        friendlies = new List<Entity>();
        enemies = new List<Enemy>();

        //Colours:
        pathColour = Color.grey;
        movementRangeColour = new Color32(183, 183, 183, 1);

        uiController = FindObjectOfType<BattleSystemUI>();

        //Generate the map
        parent = GameObject.FindGameObjectWithTag("BattleZone").transform;
        tileGenerator = FindObjectOfType<TileGenerator>();

        //Nodes = Nodes.Select(n => n as Node).ToList();

        Nodes = tileGenerator.AddNodes(width, height, battleTileSize, nodeType: nameof(Node)).Select(n => n as Node).ToList();
        tileGenerator.GenerateTileMap("GrassTile", Nodes, removeNodes: false, tileSize: battleTileSize, parent: parent);

        print("here");

        //TODO remove this, IN
        System.Random rnd = new System.Random();
        int index = rnd.Next(Nodes.Count);
        Node playerStartingLocation = Nodes[index];
        index = rnd.Next(Nodes.Count);
        Node enemyStartingLocation = Nodes[index];

        friendlies.Add(SetupEntity(nameof(Player), playerStartingLocation.transform));
        enemies.Add(SetupEntity(nameof(SampleEnemy), enemyStartingLocation.transform) as Enemy);        

        Pathfinding.OnPathUpdatedEvent += OnPathUpdated;

        friendlies[0].OnEndTurnEvent += OnEndTurn;
        enemies[0].OnEndTurnEvent += OnEndTurn;

        playerMovementRange = Pathfinding.GetRange(Nodes, friendlies[0].nodeParent, friendlies[0].Speed);

        //Generate the UI
        uiController.CreateGrid(width, height, battleTileSize);
        uiController.GenerateAttackButtons(friendlies[0]);
    }

    private void Start()
    {
        OnTurnValueChangedEvent(CurrentTurn);
        OnPlayerInfoChangedEvent(friendlies[0].MaxHealth, friendlies[0].CurrentHealth, canMove.ToString());
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
                        Entity entity = raycastObject.GetComponent<Entity>();
                        //print("Attacking: " + entity.name);
                        Attack.AttackTarget(lastSelectedAttack, entity);
                        //End turn once attacked
                        canMove = true;
                        OnPlayerInfoChangedEvent(canMove: canMove.ToString());
                        OnEndTurn();
                    }
                }
            }
           
        }
        //TODO do not do this
        else if(CurrentTurn==Turn.Enemy && !eventCalled)
        {
            eventCalled = true;
            OnEnemyTurnEvent(friendlies);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Pathfinding.OnPathUpdatedEvent -= OnPathUpdated;
            SceneManager.LoadScene(0);
        }
    }

    /// <summary>
    /// Called when Pathfinding.OnPathUpdatedEvent is invoked
    /// </summary>
    /// <param name="nodes"></param>
    private void OnPathUpdated(List<Node> nodes)
    {
        Entity entity = friendlies[0].GetComponent<Entity>();
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
            Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();

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
    private Entity SetupEntity(string type, Transform parent)
    {
        GameObject obj = Instantiate(Resources.Load(type), Vector3.zero, Quaternion.identity) as GameObject;
        obj.transform.localScale = new Vector3(battleTileSize, battleTileSize, battleTileSize);
        obj.transform.SetParent(parent);
        obj.transform.localPosition = new Vector3(0, battleTileSize / 2, 0);

        Entity entity = obj.GetComponent<Entity>();
        entity.RefreshParent();

        return entity;
    }

    public void OnEndTurn()
    {
        //print("Ending Turn...");
        if (CurrentTurn == Turn.Enemy)
        {
            foreach(Enemy enemy in enemies)
            {
                enemy.ResetScores();
            }
            eventCalled = false;
            canDisplayPathTiles = true;
            canMove = true;

            playerMovementRange = Pathfinding.GetRange(Nodes, friendlies[0].nodeParent, friendlies[0].Speed);
            OnPlayerInfoChangedEvent(friendlies[0].MaxHealth, friendlies[0].CurrentHealth, canMove.ToString());
            CurrentTurn = Turn.Player;
        }
        else
        {
            CurrentTurn = Turn.Enemy;
        }
        OnTurnValueChangedEvent(CurrentTurn);
    }


}
