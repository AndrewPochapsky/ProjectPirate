using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MovementController {

	float acceleration = 200;
	float maxSpeed = 300;
	float rotationSpeed = 200;
	
	Transform model;

	/*protected override void Awake()
	{
		base.Awake();
		//player = GetComponent<Player>();
	}*/

	// Use this for initialization
	void Start () {
		SideTiltAmount = 10;
		FrontTiltAmount = 8;
		model = transform.GetChild(0);
		//TODO get a better solution to this
		
	}

	protected override void FixedUpdate()
	{
		Movement();
		PassiveMovment(model);
	}

	//TODO fix bug with rotation and movement (A weird drifting effect) - Seems to be improved with using torque
	protected override void Movement(Transform target = null)
	{
		float translation = Input.GetAxisRaw("Vertical");
		float rotation = Input.GetAxisRaw("Horizontal") * rotationSpeed;
		rotation *= Time.fixedDeltaTime;

		if(!player.anchorDropped)
		{
			//Rotation
			//Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, rotation, 0));
			//rb.MoveRotation(rb.rotation * deltaRotation);
			rb.AddTorque(transform.up * rotation, ForceMode.Acceleration);

			//Acceleration
			rb.AddForce(transform.forward * acceleration * translation, ForceMode.Acceleration);
		}
		
		
		//The max velocity
		Vector3 max = maxSpeed * transform.forward;
		
		//Limiting the max velocity
		if(rb.velocity.magnitude > max.magnitude)
		{
			rb.velocity = max;
		}

		//Decelleration
		if(translation == 0 && rb.velocity.magnitude > 0)
		{
			rb.velocity *= 0.9f;
		}
	}
	
}
