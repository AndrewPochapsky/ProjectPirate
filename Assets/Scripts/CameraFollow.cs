using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	Transform player;

	// Use this for initialization
	void Start () {
		player = GameObject.FindObjectOfType<PlayerController>().transform;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = new Vector3(player.position.x, 0, player.position.z);
	}
}
