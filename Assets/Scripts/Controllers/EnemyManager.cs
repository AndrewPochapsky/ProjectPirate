using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


/*TODO: create an object pooling system for this instead of just instantiating*/
public class EnemyManager : MonoBehaviour {

	LocalData localData;
	BattleData battleData;

	Player player;

	float enemySpawnChance = 0.05f;
	int enemyAmount = 0;
	int maxEnemyAmount = 3;

	public static List<GameObject> enemiesInWorld;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		localData = Resources.Load<LocalData>("Data/LocalData");
		battleData = Resources.Load<BattleData>("Data/BattleData");

		foreach(Enemy enemy in GameObject.FindObjectsOfType<Enemy>())
		{
			Destroy(enemy.gameObject);
		}


		enemiesInWorld = new List<GameObject>();
		
		if(localData.enemyData != null)
		{
			for(int i = 0; i < localData.enemyData.Count; i++)
			{
				if(localData.enemyData[i] != null)
					enemiesInWorld.Add(SpawnEnemyFromData(localData, i));
			}
		}
		player = GameObject.FindObjectOfType<Player>();
        player.OnNewTileEnteredEvent += SpawnEnemies;
	}

	private void SpawnEnemies(List<BaseNode> nodes)
	{
		//TODO: add check to prevent multiple enemies on a single tile
		//TODO: prevent enemies from spawning in tile adjacent to player
		foreach(BaseNode node in nodes)
		{
			float f = Random.Range(0f, 1f);
			EnemyController existingEnemy = null;
		
			if(node.transform.childCount >= 2)
				existingEnemy = node.transform.GetChild(1).GetComponent<EnemyController>();

			if(Time.timeSinceLevelLoad > 2f && enemiesInWorld.Count < maxEnemyAmount && f <= enemySpawnChance && existingEnemy == null)
			{
				//spawn enemy at node
				
				GameObject obj = Instantiate(Resources.Load("Enemy"), new Vector3(node.transform.position.x, 60, node.transform.position.z), Quaternion.identity) as GameObject;
				enemiesInWorld.Add(obj);
				
				Material mat = obj.GetComponent<Enemy>().model.GetComponent<MeshRenderer>().material;

				mat.DOFade(1, 1f);

				obj.transform.SetParent(node.transform);

			}
		}

	}

	private GameObject SpawnEnemyFromData(LocalData data, int index)
	{
		GameObject gameObject = Instantiate(Resources.Load("Enemy"), data.enemyPositions[index], data.enemyRotations[index]) as GameObject;
		Material mat = gameObject.GetComponent<Enemy>().model.GetComponent<MeshRenderer>().material;
		mat.DOFade(1, 0);

        return gameObject;
	}

	public static void PopulateLocalDataEnemyInfo(LocalData data, GameObject exclusion)
	{
		int exclusionIndex = enemiesInWorld.IndexOf(exclusion);
		for(int i = 0; i < enemiesInWorld.Count; i++)
		{
			if(i == exclusionIndex)
			{
				continue;
			}
			data.enemyData.Add(enemiesInWorld[i].GetComponent<Enemy>().entityData);
			data.enemyPositions.Add(enemiesInWorld[i].transform.position);
			data.enemyRotations.Add(enemiesInWorld[i].transform.rotation);
		}
	}

	public static void KillAllEnemies()
	{
		foreach(var enemy in enemiesInWorld)
		{
			Destroy(enemy.gameObject);
		}

		enemiesInWorld = new List<GameObject>();
	}
}
