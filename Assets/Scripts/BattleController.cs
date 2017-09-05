using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour {

    WorldGenerator worldGenerator;

	private void Awake()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
        worldGenerator.GenerateWorld(10, 10);
        worldGenerator.GenerateTileMap("GrassTile", false);
    }
}
