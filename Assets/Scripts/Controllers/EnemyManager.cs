using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


/*TODO: create an object pooling system for this instead of just instantiating*/
public class EnemyManager : MonoBehaviour {

	LocalData localData;

	Player player;

	float enemySpawnChance = 0.2f;
	int enemyAmount = 0;
	int maxEnemyAmount = 3;

	public static List<GameObject> enemiesInWorld;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		localData = Resources.Load<LocalData>("Data/LocalData");
		
		foreach (var enemy in localData.enemies)
        {
            print("Destroying");
			if(enemy != null)
            	Destroy(enemy.gameObject);
        }	
		
		enemiesInWorld = new List<GameObject>();
		
		if(localData.enemies != null)
		{
			foreach(GameObject enemy in localData.enemies)
			{
				if(enemy != null)
					enemiesInWorld.Add(SpawnEnemyFromData(enemy));
			}
		}
	}

	// Use this for initialization
	void Start () {
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

			if(Time.timeSinceLevelLoad > 2f && f <= enemySpawnChance && enemyAmount < maxEnemyAmount && existingEnemy == null)
			{
				//spawn enemy at node
				enemyAmount++;
				GameObject obj = Instantiate(Resources.Load("Enemy"), new Vector3(node.transform.position.x, 60, node.transform.position.z), Quaternion.identity) as GameObject;
				enemiesInWorld.Add(obj);
				
				Material mat = obj.GetComponent<Enemy>().model.GetComponent<MeshRenderer>().material;

				mat.DOFade(1, 1f);

				obj.transform.SetParent(node.transform);

			}
		}

	}

	private GameObject SpawnEnemyFromData(GameObject enemy)
	{
		GameObject gameObject = Instantiate(Resources.Load("Enemy"), enemy.transform.position, enemy.transform.rotation) as GameObject;
		Material mat = gameObject.GetComponent<Enemy>().model.GetComponent<MeshRenderer>().material;
		mat.DOFade(1, 0);

        return gameObject;
	}
}
