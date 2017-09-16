using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour {

    TileGenerator tileGenerator;

    const int battleTileSize = 50;

    const int width = 10;
    const int height = 10;

    Tile lastSelectedTile;

    Entity player;

    Transform parent;

	private void Awake()
    {
        parent = GameObject.FindGameObjectWithTag("World").transform;
        tileGenerator = FindObjectOfType<TileGenerator>();
        tileGenerator.AddNodes(width, height, battleTileSize);
        tileGenerator.GenerateTileMap("GrassTile", removeNodes: false, tileSize: battleTileSize, parent: parent);

        Pathfinding.OnPathUpdatedEvent += OnPathUpdated;
    }

    private void Start()
    {
        //TODO remove this, just for testing currently
        System.Random rnd = new System.Random();
        int index = rnd.Next(TileGenerator.Nodes.Count);
        Node startingLocation = TileGenerator.Nodes[index];

        GameObject playerObj = Instantiate(Resources.Load("Entity"), Vector3.zero, Quaternion.identity) as GameObject;
        playerObj.transform.localScale = new Vector3(battleTileSize, 1, battleTileSize);
        playerObj.transform.SetParent(startingLocation.transform);
        playerObj.transform.localPosition = Vector3.zero;
        player = playerObj.GetComponent<Entity>();
    }

    private void Update()
    {
        //TODO dont call this every frame
        if(!player.IsMoving)
            Pathfinding.FindPath(player.GetComponentInParent<Node>(), GetTargetNode(MouseRaycast()));
    }

    /// <summary>
    /// Called when Pathfinding.OnPathUpdatedEvent is invoked
    /// </summary>
    /// <param name="nodes"></param>
    void OnPathUpdated(List<Node> nodes)
    {
        Entity entity = player.GetComponent<Entity>();
        if (!entity.IsMoving && Input.GetMouseButtonDown(0))
        {
            entity.SetPathNodes(nodes);
        }
    }

    /// <summary>
    /// Raycasts from mouse
    /// </summary>
    /// <returns>If hits tile, returns</returns>
    private Tile MouseRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        bool hasHit = Physics.Raycast(ray, out hit, int.MaxValue);

        if (hasHit)
        {
            Tile tile = hit.collider.gameObject.GetComponent<Tile>();
            if (tile != null)
            {
                if (lastSelectedTile != null && tile != lastSelectedTile)
                {
                    lastSelectedTile.Deselect();
                }
                tile.Select();
                lastSelectedTile = tile;
            }
        }
        return lastSelectedTile;
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
}
