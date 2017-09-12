﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour {

    WorldGenerator worldGenerator;

    const int battleTileSize = 50;

    const int width = 10;
    const int height = 10;

    Tile lastSelectedTile;

    Entity player;

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

        GameObject playerObj = Instantiate(Resources.Load("Entity"), Vector3.zero, Quaternion.identity) as GameObject;

        playerObj.transform.localScale = new Vector3(battleTileSize, 1, battleTileSize);

        int index = rnd.Next(WorldGenerator.Nodes.Count);

        Node startingLocation = WorldGenerator.Nodes[index];

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

    void OnPathUpdated(List<Node> nodes)
    {
        Entity entity = player.GetComponent<Entity>();
        if (!entity.IsMoving && Input.GetMouseButtonDown(0))
        {
            //MoveEntity(player.GetComponent<Entity>(), nodes);
            entity.SetPathNodes(nodes);
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


    
}
