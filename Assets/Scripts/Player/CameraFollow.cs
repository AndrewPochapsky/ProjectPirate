using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	Player player;
	
	Transform target;

	[HideInInspector]
	public Vector3 zoomOffset;
	
	[HideInInspector]
	public Vector3 zoomValue;
	// Use this for initialization
	void Start () {
		player = GameObject.FindObjectOfType<Player>();
		zoomValue = new Vector3(0, 250, -165);
		zoomOffset = Vector3.zero;
	}

	void FixedUpdate()
	{
		//transform.position = (0.9f * transform.position) + (0.1f * target.position);

		transform.position += (target.position - transform.position) *0.05f;
	}

	public void SetTarget(Transform _target)
	{
		target = _target;
	}
}
