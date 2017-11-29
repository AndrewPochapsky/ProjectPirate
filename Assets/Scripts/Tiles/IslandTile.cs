using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

public class IslandTile : Tile {

    public string Name { get; protected set; }

    BoxCollider col;
    IslandUI islandUI;

    public enum IslandSize
    {               //Sizes:
        Regular,    // 1x1
        Long,       // 2x1
        Tall,       // 1x2
        Large       // 2x2
    }

    [HideInInspector]
    public List<GameObject> meshObjects;

    public IslandSize Size { get; set; }

    public UniqueIslandData UniqueIslandData { get; private set; }

    private IslandGenerator generator;

    private void Awake()
    {
        islandUI = GetComponent<IslandUI>();
        col = GetComponent<BoxCollider>();
        meshObjects = new List<GameObject>();
        generator = GetComponent<IslandGenerator>();
        UniqueIslandData = new UniqueIslandData();

        UniqueIslandData.Initialize();
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
         Name = DataGenerator.Instance.GenerateIslandName(Random.Range(2, 4)); 
         islandUI.SetUI(Name);
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        print("Welcome to " + Name + "!");
    }

    public override void Enable(bool value)
    {

    }

    /// <summary>
    /// Calls functions which generate island
    /// </summary>
    public void GenerateIsland()
    {
        generator.DetermineSize();
        generator.GenerateIsland();
    }


    /// <summary>
    /// Determine the size of which to generate the island
    /// </summary>
    /// <returns>The determined size</returns>
    public static IslandSize DetermineIslandSize()
    {
        float randomValue = Random.Range(0f, 1f);

        if (randomValue <= 0.5f)
        {
            return IslandSize.Regular;
        }
        else if (randomValue <= 0.7f)
        {
            return IslandSize.Long;
        }
        else if (randomValue <= 0.9f)
        {
            return IslandSize.Tall;
        }
        return IslandSize.Large;
    }

    /// <summary>
    /// Gets the correct island offset given its size
    /// </summary>
    /// <param name="islandTile">The island tile</param>
    /// <param name="tileSize">The tileSize</param>
    /// <returns>The offset</returns>
    public Vector3 GetIslandOffset(int tileSize)
    {
        switch (Size)
        {
            case IslandSize.Long:
                col.size = new Vector3(1300, 500, 600);
                return new Vector3(tileSize / 2, 0, 0);

            case IslandSize.Tall:
                col.size = new Vector3(600, 500, 1300);
                return new Vector3(0, 0, tileSize / 2);

            case IslandSize.Large:
                col.size = new Vector3(1300, 500, 1300);
                return new Vector3(tileSize / 2, 0, tileSize / 2);
            
            default:
                col.size = new Vector3(600, 500, 600);
                break;
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Combines all of the oceanTiles underneath this island into one single mesh
    /// </summary>
    /// <param name="parent">The parent of the new mesh</param>    
    public void CombineOceanMeshes(Transform parent)
    {
        MeshFilter[] meshFilters = meshObjects.Select(o => o.GetComponent<MeshFilter>()).ToArray();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        GameObject newMeshObject = Instantiate(Resources.Load("Tiles/IslandOceanTile"), new Vector3(0, WorldController.Instance.oceanTileOffset, 0), Quaternion.identity) as GameObject;
        int i = 0;
        while(i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            Destroy(meshFilters[i].gameObject);
            
            combine[i].transform = transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        MeshFilter newMeshFilter = newMeshObject.GetComponent<MeshFilter>();
        Renderer newRenderer = newMeshObject.GetComponent<Renderer>();

        newMeshFilter.mesh = new Mesh(); 
        newMeshFilter.mesh.CombineMeshes(combine);

        //The local min and maxes
        Vector2 min = new Vector2(newMeshFilter.mesh.bounds.min.x, newMeshFilter.mesh.bounds.min.z);
        Vector2 max = new Vector2(newMeshFilter.mesh.bounds.max.x, newMeshFilter.mesh.bounds.max.z);
        float spanX = Mathf.Abs(min.x - max.x);
        float spanZ = Mathf.Abs(min.y - max.y);
        
        Vector3[] verticies = newMeshFilter.mesh.vertices;
        Vector2[] uvs = newMeshFilter.mesh.uv;

        for(int v = 0; v < verticies.Length; v++)
        {
            Vector2 position = new Vector2(verticies[v].x, verticies[v].z);

            //Thank you to /u/DarKRoastJames
            float newX = (position.x - min.x)/spanX;
            float newZ = (position.y - min.y)/spanZ;
            uvs[v] = new Vector2(newX, newZ);
        }
        newMeshFilter.mesh.uv = uvs;
        newRenderer.material.SetFloat("isIsland", 1);

        newMeshObject.gameObject.SetActive(true);
        newMeshObject.transform.SetParent(transform);
        newMeshObject.transform.localPosition = new Vector3(0, 0, 0);
        newMeshObject.transform.localScale = Vector3.one;
    }
}
