using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IslandTile : Tile {

    public enum IslandSize
    {               //Sizes:
        Regular,    // 1x1
        Long,       // 2x1
        Tall,       // 1x2
        Large       // 2x2
    }

    public List<GameObject> meshObjects;

    public IslandSize Size { get; set; }

    public UniqueIslandData UniqueIslandData { get; private set; }

    private IslandGenerator generator;

    private void Awake()
    {
        meshObjects = new List<GameObject>();
        generator = GetComponent<IslandGenerator>();
        UniqueIslandData = new UniqueIslandData();

        UniqueIslandData.Initialize();
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
    public static Vector3 GetIslandOffset(IslandTile islandTile, int tileSize)
    {
        switch (islandTile.Size)
        {
            case IslandSize.Long:
                return new Vector3(tileSize / 2, 0, 0);

            case IslandSize.Tall:
                return new Vector3(0, 0, tileSize / 2);

            case IslandSize.Large:
                return new Vector3(tileSize / 2, 0, tileSize / 2);
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Gets the correct oceanTileSize for the given island's oceanTile child
    /// </summary>
    /// <param name="islandTile">The island tile</param>
    /// <param name="tileSize">The tileSize</param>
    /// <returns>The size</returns>
    public void CombineOceanMeshes(Transform parent)
    {
        MeshFilter[] meshFilters = meshObjects.Select(o => o.GetComponent<MeshFilter>()).ToArray();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        GameObject newMeshObject = Instantiate(Resources.Load("Tiles/"+nameof(OceanTile) + "EXP"), new Vector3(0, WorldController.oceanTileOffset, 0), Quaternion.identity) as GameObject;
        int i = 0;
        while(i < meshFilters.Length)
        {
            print("happening");
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            //meshFilters[i].gameObject.SetActive(false);
            Destroy(meshFilters[i].gameObject);
            
            combine[i].transform = transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;
            i++;
        }
       
        MeshFilter newMeshFilter = newMeshObject.GetComponent<MeshFilter>();

        newMeshFilter.mesh = new Mesh(); 
        newMeshFilter.mesh.CombineMeshes(combine);
        newMeshObject.GetComponent<Renderer>().materials[0].SetFloat("isIsland", 1);
        
        newMeshObject.gameObject.SetActive(true);
        newMeshObject.transform.SetParent(transform);
        newMeshObject.transform.localPosition = Vector3.zero;
        newMeshObject.transform.localScale = Vector3.one;

    }

}
