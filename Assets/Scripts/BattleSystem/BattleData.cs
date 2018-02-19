using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle Data")]
public class BattleData : Data {
	public List<EntityData> Friendlies = new List<EntityData>();
	public List<EntityData> Enemies = new List<EntityData>();
	
	public int InfamyReward = 0;
	public List<ISellable> Items = new List<ISellable>();
    //The enemy gameobject which is destroyed after the battle(if player won)

	public override void ResetData()
	{
		Friendlies = new List<EntityData>();
		Enemies = new List<EntityData>();
        Items = new List<ISellable>();
		InfamyReward = 0;
	}
}
