using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


/*TODO: create an object pooling system for this instead of just instantiating*/
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
		//TODO: add check to prevent multiple enemies on a single tile
		foreach(BaseNode node in nodes)
		{
			float f = Random.Range(0f, 1f);
			EnemyController existingEnemy = null;
		
			if(node.transform.childCount >= 2)
				existingEnemy = node.transform.GetChild(1).GetComponent<EnemyController>();

			if(Time.timeSinceLevelLoad > 2f && f <= enemySpawnChance && enemyAmount < maxEnemyAmount && existingEnemy == null)
			{
				//spawn enemy at node
				enemyAmount++;
				GameObject obj = Instantiate(Resources.Load("Enemy"), new Vector3(node.transform.position.x, 60, node.transform.position.z), Quaternion.identity) as GameObject;
				Material mat = obj.GetComponent<MeshRenderer>().material;

				mat.DOFade(1, 1f);

				obj.transform.SetParent(node.transform);

			}
		}

	}
}
