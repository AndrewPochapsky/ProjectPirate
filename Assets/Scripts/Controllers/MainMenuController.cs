using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		BattleData data = Resources.Load<BattleData>("Data/BattleData");
		data.ResetData();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
