using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileGenerator : MonoBehaviour {

    public static TileGenerator Instance;

    enum TileType { OceanTile, IslandTile }

    private Vector3 tileLocation;

    /// <summary>
    /// Value which increases the ocean tile's scale slightly to prevent weird gaps
    /// </summary>
    private Vector3 tileScaleAddition;

    float islandSpawnChance = 0.1f;//TODO fiddle around with this value

    private void Awake()
    {
        if(Instance != null && Instance != this){
			Destroy(gameObject);
		}else{
			Instance = this;
		}
        tileScaleAddition = new Vector3(50, 50, 0);
        tileLocation = Vector3.zero;
    }

    /// <summary>
    /// Adds the nodes to the map
    /// </summary>
    /// <param name="worldWidth">The width</param>
    /// <param name="worldHeight">The height</param>
    /// <param name="tileSize">The tileSize</param>
    /// <param name="parent">The parent</param>
    public List<BaseNode> AddNodes(int worldWidth, int worldHeight, int tileSize, int offset = 0, Transform parent = null, string nodeType = nameof(BaseNode))
    {
        //int offset = 8;
        List<BaseNode> nodes = new List<BaseNode>();
        for (int y = 0; y < worldHeight; y++)
        {
            for (int x = 0; x < worldWidth; x++)
            {
                Vector2 location = new Vector2(x, y);
                nodes.Add(AddNode(location, worldWidth, tileSize-offset, parent, nodeType));
            }
        }

        foreach(BaseNode node in nodes)
        {
            node.SetAdjacents(nodes, worldWidth);
        }

        return nodes;
    }

    /// <summary>
    /// Add a node to the grid
    /// </summary>
    /// <param name="location">The grid location to give to the instantiated tile(row, column)</param>
    /// <param name="worldWidth">The world width</param>
    /// <param name="tileSize">The tileSize</param>
    /// <param name="parent">The parent</param>
    /// <returns>The created tile</returns>
    private BaseNode AddNode(Vector2 location, int worldWidth, int tileSize, Transform parent, string nodeType = nameof(BaseNode))
    {
        GameObject obj = Instantiate(Resources.Load("Tiles/" + nodeType), Vector3.zero, Quaternion.identity) as GameObject;
        BaseNode tile = obj.GetComponent<BaseNode>();

        if (parent != null)
            tile.transform.SetParent(parent);

        obj.transform.localPosition = tileLocation;

        tile.location = location;

        tileLocation = GetNextNodeLocation(tile, worldWidth, tileSize);

        return tile;
    }

    /// <summary>
    /// Generates the ocean
    /// </summary>
    /// <param name="nodes">The nodes</param>
    /// <param name="addIslands">True if want to include islands</param>
    /// <param name="removeNodes">True if nodes should be removed</param>
    /// <param name="tileSize">The tileSize</param>
    /// <param name="parent">The parent</param>
    public void GenerateOceanTiles(List<BaseNode> nodes, bool addIslands, bool removeNodes, int tileSize, Transform parent)
    {
        Tile createdTile = null;

        foreach (BaseNode node in nodes)
        {
            if (node.isAvailable)
            {
                float num = Random.Range(0f, 1f);

                if (addIslands && num <= islandSpawnChance)
                {
                    IslandTile.IslandSize size = IslandTile.DetermineIslandSize();

                    if (CanGenerate(node, size))
                    {
                        createdTile = AddIslandTile(node, size, tileSize, parent);
                        //AddAnyTile(nameof(OceanTile), node, tileSize/10, parent, forIsland: true);
                    }
                    else
                    {
                        createdTile = AddAnyTile(nameof(OceanTile), node, tileSize/10, parent, WorldController.Instance.oceanTileOffset);
                    }
                }
                else
                {
                    createdTile = AddAnyTile(nameof(OceanTile), node, tileSize/10, parent, WorldController.Instance.oceanTileOffset);
                }
            }

            if (removeNodes)
            {
                Destroy(node.gameObject);
            }
            else
            {
                createdTile.transform.SetParent(node.transform);
            }
        }
    }

    /// <summary>
    /// Adds either an ocean or island tile at a specified location
    /// </summary>
    /// <param name="node">The node</param>
    /// <param name="islandSize">The islandSize</param>
    /// <param name="tileSize">The tileSize</param>
    /// <param name="parent">The parent</param>
    /// <returns>The created tile</returns>
    private Tile AddIslandTile(BaseNode node, IslandTile.IslandSize islandSize, int tileSize, Transform parent)
    {
        Vector3 position = new Vector3(node.transform.position.x, node.transform.position.y, node.transform.position.z);
        GameObject obj = Instantiate(Resources.Load("Tiles/IslandTile"), position, Quaternion.identity) as GameObject;

        IslandTile islandTile = obj.GetComponent<IslandTile>();
        islandTile.Size = islandSize;

        islandTile.GenerateIsland();

        SetUpIsland(node, obj, tileSize, parent);

        obj.transform.SetParent(parent);

        return obj.GetComponent<Tile>();
    }

    /// <summary>
    /// Adds the specified tile at node's location
    /// </summary>
    /// <param name="tileName">The name of the tile to create</param>
    /// <param name="node">The node</param>
    /// <param name="tileSize">The tileSize</param>
    /// <param name="parent">The parent</param>
    /// <param name="forIsland">True if forIsland</param>
    /// <returns>The created tile</returns>
    private Tile AddAnyTile(string tileName, BaseNode node, int tileSize, Transform parent, int offset, bool forIsland = false, bool forBattle = false)
    {
        Vector3 position = new Vector3(node.transform.position.x, node.transform.position.y + offset, node.transform.position.z);

        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        GameObject obj = Instantiate(Resources.Load("Tiles/"+ tileName), position, rotation) as GameObject;

        //TODO change the quads to planes in the battle scene to avoid having to do this
        if(!forBattle)
        {
            obj.transform.localScale = new Vector3(tileSize, 1, tileSize);
        }
        else
        {
            obj.transform.localScale = new Vector3(tileSize, tileSize, 1);
            obj.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }


        obj.transform.SetParent(parent);

        if(forIsland)
        {
            obj.GetComponent<Renderer>().materials[0].SetFloat("isIsland", 1);
        }

        return obj.GetComponent<Tile>();
    }

    /// <summary>
    /// Generates a map of only a specific tile, used for battles
    /// </summary>
    /// <param name="tileName">Tile name to create</param>
    /// <param name="removeNodes">If true nodes will be removed</param>
    /// <param name="tileSize">The tileSize</param>
    public void GenerateTileMap(string tileName, List<Node> nodes, bool removeNodes, int tileSize, Transform parent)
    {
        print(nodes.Count);
        foreach(Node node in nodes)
        {
            Tile createdTile = AddAnyTile(tileName, node, tileSize, parent, offset: 0, forBattle: true);

            if (removeNodes)
            {
                Destroy(node.gameObject);
            }
            else
            {
                createdTile.transform.SetParent(node.transform);
                node.transform.SetParent(parent);
            }
        }
    }

    /// <summary>
    /// Adjust island based on its size
    /// </summary>
    /// <param name="node">Node used as a location reference</param>
    /// <param name="obj">The instantiated island</param>
    /// <param name="tileSize">The tileSize</param>
    private void SetUpIsland(BaseNode node, GameObject obj, int tileSize, Transform parent)//TODO maybe move this to IslandTile class?
    {
        IslandTile islandTile = obj.GetComponent<IslandTile>();

        DisableRedundantNodes(islandTile, node, tileSize, parent);

        islandTile.transform.position += islandTile.GetIslandOffset(tileSize);

        islandTile.CombineOceanMeshes(parent);
    }

    /// <summary>
    /// Get the location to add the next node
    /// </summary>
    /// <param name="node">The tile whose location is used</param>
    /// <param name="worldWidth">The world width</param>
    /// <param name="tileSize">The tileSize</param>
    /// <returns>A Vector3 of the next location</returns>
    private Vector3 GetNextNodeLocation(BaseNode node, int worldWidth, int tileSize)
    {
        Vector3 nextLocation = Vector3.zero;

        //The tile is currently on the edge so the next one should start new row
        if(node.location.x + 1 == worldWidth)
        {
            nextLocation = new Vector3(0, 0, tileLocation.z + tileSize);
        }
        else
        {
            nextLocation = new Vector3(tileLocation.x + tileSize, 0, tileLocation.z);
        }

        return nextLocation;
    }

    /// <summary>
    /// Disables nodes which are now taken up by larger island
    /// </summary>
    /// <param name="islandTile">The islandTile</param>
    /// <param name="node">The node</param>
    private void DisableRedundantNodes(IslandTile islandTile, BaseNode node, int tileSize, Transform parent)
    {
        islandTile.meshObjects.Add(AddAnyTile(nameof(OceanTile), node, tileSize/10, parent, WorldController.Instance.oceanTileOffset, forIsland: true).gameObject);
        node.isAvailable = false;
        switch (islandTile.Size)
        {
            case IslandTile.IslandSize.Long:
                islandTile.meshObjects.Add(AddAnyTile(nameof(OceanTile), node.RightEmpty, tileSize/10, parent, WorldController.Instance.oceanTileOffset, forIsland: true).gameObject);
                node.RightEmpty.isAvailable = false;
                break;

            case IslandTile.IslandSize.Tall:
                islandTile.meshObjects.Add(AddAnyTile(nameof(OceanTile), node.TopEmpty, tileSize/10, parent, WorldController.Instance.oceanTileOffset, forIsland: true).gameObject);
                node.TopEmpty.isAvailable = false;
                break;

            case IslandTile.IslandSize.Large:

                if (node.RightEmpty != null)
                {
                    islandTile.meshObjects.Add(AddAnyTile(nameof(OceanTile), node.RightEmpty, tileSize/10, parent, WorldController.Instance.oceanTileOffset, forIsland: true).gameObject);
                    node.RightEmpty.isAvailable = false;
                }
                if (node.TopEmpty != null)
                {
                    if (node.TopEmpty.RightEmpty != null)
                    {
                        islandTile.meshObjects.Add(AddAnyTile(nameof(OceanTile), node.TopEmpty.RightEmpty, tileSize/10, parent, WorldController.Instance.oceanTileOffset, forIsland: true).gameObject);
                        node.TopEmpty.RightEmpty.isAvailable = false;
                    }
                    islandTile.meshObjects.Add(AddAnyTile(nameof(OceanTile), node.TopEmpty, tileSize/10, parent, WorldController.Instance.oceanTileOffset, forIsland: true).gameObject);
                    node.TopEmpty.isAvailable = false;
                }
                break;
        }
    }

    /// <summary>
    /// Returns true if there are no conflicts with generating the specifed size of island
    /// </summary>
    /// <param name="node">The node</param>
    /// <param name="size">The specified size</param>
    /// <returns>Whether or not the specified size can be generated at node's location</returns>
    private bool CanGenerate(BaseNode node, IslandTile.IslandSize size)
    {
        switch (size)
        {
            case IslandTile.IslandSize.Regular:
                return true;

            case IslandTile.IslandSize.Long:
                if(node.RightEmpty != null)
                {
                    if (node.RightEmpty.isAvailable)
                    {
                        return true;
                    }
                }
                
                return false;

            case IslandTile.IslandSize.Tall:
                if(node.TopEmpty != null)
                {
                    if (node.TopEmpty.isAvailable)
                    {
                        return true;
                    }
                }
                
                return false;

            case IslandTile.IslandSize.Large:
                if(node.RightEmpty != null && node.TopEmpty != null && node.TopEmpty.RightEmpty != null)
                {
                    if (node.RightEmpty.isAvailable && node.TopEmpty.isAvailable && node.TopEmpty.RightEmpty.isAvailable)
                    {
                        return true;
                    }
                }
                
                return false;
        }

        Debug.Log("Something went wrong with island generation");
        return true;
    }

    /// <summary>
    /// Sets tileLocation vector3 to zero
    /// </summary>
    public void ResetTileLocation()
    {
        tileLocation = Vector3.zero;
    }
}

       

