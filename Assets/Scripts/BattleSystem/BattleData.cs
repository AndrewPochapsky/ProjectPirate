using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle Data")]
public class BattleData : ScriptableObject {
	public List<EntityData> Friendlies = new List<EntityData>();
	public List<EntityData> Enemies = new List<EntityData>();
	
	public int InfamyReward = 0;
	public List<ISellable> Items = new List<ISellable>();
    //The enemy gameobject which is destroyed after the battle(if player won)

	//maybe add bool like playerWon or something
    public GameObject enemyObject = null;

	public void ResetData()
	{
		Friendlies = new List<EntityData>();
		Enemies = new List<EntityData>();
        Items = new List<ISellable>();
	}
}
