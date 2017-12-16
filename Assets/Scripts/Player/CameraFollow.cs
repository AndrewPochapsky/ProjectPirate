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
	
	// Update is called once per frame
	void LateUpdate () {
		//If not at island than set position to the target(the player in this case)
		if(!player.anchorDropped)
		{
			//transform.position = new Vector3(target.position.x, 0, target.position.z);
		}
		//Otherwise lerp smoothly to this other transform(Island most likely)
		else
		{
			/*Vector3 start = transform.position;
			Vector3 end = target.position - zoomOffset;
			float speed = 2;
			Vector3 newArmPos = Vector3.Lerp(start, end, speed * Time.deltaTime);
			transform.position = newArmPos;*/
		}
		Vector3 start = transform.position;
		Vector3 end = target.position - zoomOffset;
		float speed = 2;
		Vector3 newArmPos = Vector3.Lerp(start, end, speed * Time.deltaTime);
		transform.position = newArmPos;
	}

	public void SetTarget(Transform _target)
	{
		target = _target;
	}
}
