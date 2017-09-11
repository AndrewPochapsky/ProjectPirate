using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour {

    WorldGenerator worldGenerator;

    const int battleTileSize = 50;

    const int width = 10;
    const int height = 10;

    Tile lastSelectedTile;

    GameObject player;

	private void Awake()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
        worldGenerator.AddNodes(width, height, battleTileSize);
        worldGenerator.GenerateTileMap("GrassTile", removeNodes: false, tileSize: battleTileSize);

        Pathfinding.OnPathUpdatedEvent += OnPathUpdated;
    }

    private void Start()
    {
        //TODO remove this, just for testing currently
        System.Random rnd = new System.Random();

        player = Instantiate(Resources.Load("Player"), Vector3.zero, Quaternion.identity) as GameObject;

        player.transform.localScale = new Vector3(battleTileSize, 1, battleTileSize);

        int index = rnd.Next(WorldGenerator.Nodes.Count);

        Node startingLocation = WorldGenerator.Nodes[index];

        player.transform.SetParent(startingLocation.transform);

        player.transform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        //TODO dont call this every frame
        Pathfinding.FindPath(player.GetComponentInParent<Node>(), GetTargetNode(MouseRaycast()));
    }

    void OnPathUpdated(List<Node> nodes)
    {
        if (Input.GetMouseButtonDown(0))
        {
            MoveEntity(player, nodes[nodes.Count - 1]);
        }
    }

    private Tile MouseRaycast()
    {
        //TODO refactor raycasting into other class
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

    private Node GetTargetNode(Tile tile)
    {
        return tile.transform.GetComponentInParent<Node>();
    }

    /// <summary>
    /// Move the given entity to the given position
    /// </summary>
    /// <param name="entity">Entity to move</param>
    /// <param name="targetLocation">Position to move to</param>
    private void MoveEntity(GameObject entity, Node targetLocation)
    {
        //TODO change GameObject to Entity and add actual moving rather than instant tp
        entity.transform.SetParent(targetLocation.transform);
        entity.transform.localPosition = Vector3.zero;
    }

    
}
