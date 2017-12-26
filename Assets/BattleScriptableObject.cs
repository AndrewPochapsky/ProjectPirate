using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle Data")]
public class BattleScriptableObject : ScriptableObject {
	public List<EntityData> Friendlies = new List<EntityData>();
	public List<EntityData> Enemies = new List<EntityData>();
}
