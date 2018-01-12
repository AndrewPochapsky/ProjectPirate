using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Local Data")]
public class LocalData : Data {
	//position of player ship used to put player at same position after scene load
	public Vector3 playerShipPos;

	//Used to respawn enemies in the same positions after scene load
	public List<GameObject> enemies;

    public override void ResetData()
    {
        //playerShipPos = new Vector3(0, 30, 0);
		enemies = new List<GameObject>();
    }
}
