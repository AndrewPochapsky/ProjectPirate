using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle Data")]
public class BattleData : ScriptableObject {
	public List<EntityData> Friendlies = new List<EntityData>();
	public List<EntityData> Enemies = new List<EntityData>();
	public int InfamyReward = 0;

	public void ResetData()
	{
		Friendlies = new List<EntityData>();
		Enemies = new List<EntityData>();
	}
}
