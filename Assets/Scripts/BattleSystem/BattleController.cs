using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleController : MonoBehaviour {

    TileGenerator tileGenerator;

    const int battleTileSize = 50;

    const int width = 10;
    const int height = 10;

    Entity player;

    Transform parent;

    List<Node> nodes;
    List<Node> range;

    private void Awake()
    {
        parent = GameObject.FindGameObjectWithTag("BattleZone").transform;
        tileGenerator = FindObjectOfType<TileGenerator>();
        nodes = tileGenerator.AddNodes(width, height, battleTileSize);
        tileGenerator.GenerateTileMap("GrassTile", nodes, removeNodes: false, tileSize: battleTileSize, parent: parent);

        //TODO remove this, just for testing currently
        System.Random rnd = new System.Random();
        int index = rnd.Next(nodes.Count);
        Node startingLocation = nodes[index];

        GameObject playerObj = Instantiate(Resources.Load("Entity"), Vector3.zero, Quaternion.identity) as GameObject;
        playerObj.transform.localScale = new Vector3(battleTileSize, 1, battleTileSize);
        playerObj.transform.SetParent(startingLocation.transform);
        playerObj.transform.localPosition = Vector3.zero;
        player = playerObj.GetComponent<Entity>();

        Pathfinding.OnPathUpdatedEvent += OnPathUpdated;

        range = Pathfinding.GetRange(nodes, player.GetComponentInParent<Node>(), 1);
        
    }

    private void Update()
    {
        foreach (Node node in range)
        {
            node.Child.Select();
        }

        //Tile tile = MouseRaycast();

        //TODO dont call this every frame
        //if(!player.IsMoving && tile != null)
            //Pathfinding.FindPath(player.GetComponentInParent<Node>(), GetTargetNode(tile));

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
        Entity entity = player.GetComponent<Entity>();
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
}
