using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleController : MonoBehaviour {

    public delegate void OnUIValuesChanged(string turnText);
    public event OnUIValuesChanged OnUIValuesChangedEvent;

    public delegate void OnEnemyTurn(List<Entity> targets);
    public event OnEnemyTurn OnEnemyTurnEvent;

    public enum Turn { Player, Enemy }
    public Turn CurrentTurn { get; private set; } = Turn.Enemy;

    TileGenerator tileGenerator;
    BattleSystemUI uiController;

    const int battleTileSize = 50;

    const int width = 10;
    const int height = 10;

    List<Entity> friendlies;

    List<Entity> enemies;

    Transform parent;

    public List<Node> Nodes { get; private set; }

    private void Awake()
    {
        friendlies = new List<Entity>();
        enemies = new List<Entity>();

        uiController = FindObjectOfType<BattleSystemUI>();

        //Generate the map
        parent = GameObject.FindGameObjectWithTag("BattleZone").transform;
        tileGenerator = FindObjectOfType<TileGenerator>();
        Nodes = tileGenerator.AddNodes(width, height, battleTileSize);
        tileGenerator.GenerateTileMap("GrassTile", Nodes, removeNodes: false, tileSize: battleTileSize, parent: parent);

        //TODO remove this, IN
        System.Random rnd = new System.Random();
        int index = rnd.Next(Nodes.Count);
        Node playerStartingLocation = Nodes[index];
        index = rnd.Next(Nodes.Count);
        Node enemyStartingLocation = Nodes[index];

        friendlies.Add(SetupEntity(nameof(Entity), playerStartingLocation.transform));
        enemies.Add(SetupEntity(nameof(SampleEnemy), enemyStartingLocation.transform));        

        Pathfinding.OnPathUpdatedEvent += OnPathUpdated;

        friendlies[0].OnEndTurnEvent += OnEndTurn;
        enemies[0].OnEndTurnEvent += OnEndTurn;

        uiController.CreateGrid(width, height, battleTileSize);
    }

    private void Start()
    {
        OnUIValuesChangedEvent(CurrentTurn.ToString());
        /*if(CurrentTurn == Turn.Enemy)
        {
            OnEnemyTurnEvent(friendlies);
        }*/
    }

    private void Update()
    {
        Tile tile = MouseRaycast();

        //TODO change this
        if(CurrentTurn == Turn.Player)
        {
            if (!friendlies[0].IsMoving && tile != null)
                Pathfinding.FindPath(friendlies[0].nodeParent, GetTargetNode(tile), reverse: true);
        }
        //TODO do not do this
        else
        {
            print("ON enemy turn");
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
    void OnPathUpdated(List<Node> nodes)
    {
        Entity entity = friendlies[0].GetComponent<Entity>();
        if (!entity.IsMoving && Input.GetMouseButtonDown(0))
        {
            entity.SetPathNodes(nodes);
        }
    }

    /// <summary>
    /// Raycasts from mouse
    /// </summary>
    /// <returns>The tile</returns>
    private Tile MouseRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        Physics.Raycast(ray, out hit, int.MaxValue);

        if(hit.collider != null)
            return hit.collider.gameObject.GetComponent<Tile>();

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
        obj.transform.localScale = new Vector3(battleTileSize, 1, battleTileSize);
        obj.transform.SetParent(parent);
        obj.transform.localPosition = Vector3.zero;

        Entity entity = obj.GetComponent<Entity>();
        entity.RefreshParent();
        entity.nodeParent.isTraversable = false;

        return entity;
    }

    private void OnEndTurn()
    {
        print("Ending turn");
        if (CurrentTurn == Turn.Enemy)
        {
            CurrentTurn = Turn.Player;
        }
        else
        {
            CurrentTurn = Turn.Enemy;
        }
        OnUIValuesChangedEvent(CurrentTurn.ToString());
    }
}
