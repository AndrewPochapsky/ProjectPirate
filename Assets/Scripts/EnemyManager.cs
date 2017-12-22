using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
	Player player;

	float enemySpawnChance = 0.2f;

	// Use this for initialization
	void Start () {
		player = GameObject.FindObjectOfType<Player>();
		player.OnNewTileEnteredEvent += SpawnEnemies;
	}

	private void SpawnEnemies(List<BaseNode> nodes)
	{
		foreach(BaseNode node in nodes)
		{
			float f = Random.Range(0, 1);
			if(f <= enemySpawnChance)
			{
				//spawn enemy at node
			}
		}
	}
}
