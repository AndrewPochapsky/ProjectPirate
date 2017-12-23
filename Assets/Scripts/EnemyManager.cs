using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
	Player player;

	float enemySpawnChance = 0.2f;
	int enemyAmount = 0;
	int maxEnemyAmount = 3;


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
			if(f <= enemySpawnChance && enemyAmount < maxEnemyAmount)
			{
				//spawn enemy at node
				enemyAmount++;
			}
		}
	}
}
