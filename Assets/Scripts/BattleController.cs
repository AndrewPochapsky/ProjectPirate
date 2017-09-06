using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour {

    WorldGenerator worldGenerator;

    const int battleTileSize = 50;

    const int width = 10;
    const int height = 10;

    Tile lastSelectedTile;

	private void Awake()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
        worldGenerator.AddNodes(width, height, battleTileSize);
        worldGenerator.GenerateTileMap("GrassTile", removeNodes: false, tileSize: battleTileSize);
    }

    private void Update()
    {
        MouseRaycast();
    }

    private void MouseRaycast()
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
    }
}
