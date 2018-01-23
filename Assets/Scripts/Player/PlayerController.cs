using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MovementController {

	float acceleration = 200;
	float speed = 250;
	float maxSpeed = 300;
	float rotationSpeed = 100;
	
	Transform model;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		SideTiltAmount = 15;
		FrontTiltAmount = 10;
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
		if(!player.anchorDropped)
		{
			if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			{
				//rb.AddForce(transform.forward * acceleration, ForceMode.Force);
				rb.MovePosition(transform.position + transform.forward * Time.deltaTime * speed);
			}
			if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			{
				Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, 1 * rotationSpeed * Time.deltaTime, 0));
				rb.MoveRotation(rb.rotation * deltaRotation);
			}
			else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow))
			{
				Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, -1 * rotationSpeed * Time.deltaTime, 0));
                rb.MoveRotation(rb.rotation * deltaRotation);
			}



		}
		
		/*
		//The max velocity
		Vector3 max = maxSpeed * transform.forward;
		
		//Limiting the max velocity
		if(rb.velocity.magnitude > max.magnitude)
		{
			rb.velocity = max;
		}

		//Decelleration
		if(!Input.GetKey(KeyCode.W) && rb.velocity.magnitude > 0)
		{
			rb.velocity *= 0.9f;
		}*/
	}
	
}
