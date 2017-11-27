using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	float acceleration = 200;
	float maxSpeed = 300;
	float rotationSpeed = 200;
	float frontTiltAmount = 8;
	float sideTiltAmount = 10;
	float surfaceModifier;
	

	Rigidbody rb;
	Transform model;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		model = transform.GetChild(0);
		//TODO get a better solution to this
		surfaceModifier = FindObjectOfType<OceanTile>().GetComponent<Renderer>().material.GetFloat("_OceanWaveModifier");
	}

	void FixedUpdate()
	{	
		Movement();	
		PassiveMovment();	
	}	

	//TODO fix bug with rotation and movement (A weird drifting effect) - Seems to be improved with using torque
	void Movement()
	{
		float translation = Input.GetAxisRaw("Vertical");
		float rotation = Input.GetAxisRaw("Horizontal") * rotationSpeed;
		rotation *= Time.fixedDeltaTime;

		//Rotation
		//Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, rotation, 0));
		//rb.MoveRotation(rb.rotation * deltaRotation);
		rb.AddTorque(transform.up * rotation, ForceMode.Acceleration);

		//Acceleration
		rb.AddForce(transform.forward * acceleration * translation, ForceMode.Acceleration);
		
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

	float CalculateSurface(float x, float modifier)
	{
		float y = (Mathf.Sin(x * 1.0f + (Time.timeSinceLevelLoad) * 1.0f)
			+ Mathf.Sin(x * 2.3f + (Time.timeSinceLevelLoad) * 1.5f)
			+ Mathf.Sin(x * 3.3f + (Time.timeSinceLevelLoad)))
			* modifier;

		return y;
	}

	/// <summary>
	/// The movement of the boat which happens all of the time.
	/// 
	/// Bobbing up and down, tilting side to side
	/// </summary>
	void PassiveMovment()
	{
		Vector3 velocity = Vector3.zero;
		Vector3 bobbingMotion = new Vector3(transform.position.x, 
			CalculateSurface((transform.position.x), surfaceModifier) +
			CalculateSurface((transform.position.z), surfaceModifier) + 
			WorldController.oceanTileOffset,
			transform.position.z);
		
		transform.position = Vector3.SmoothDamp(transform.position, bobbingMotion, ref velocity, smoothTime: 0.2f);

		Quaternion from = Quaternion.Euler(sideTiltAmount, 90, frontTiltAmount);
		Quaternion to = Quaternion.Euler(-sideTiltAmount, 90, -frontTiltAmount);
		float t = Mathf.PingPong(Mathf.Sin(Time.time * 0.5f) + Mathf.Sin(Time.time * 0.35f), 1);

		model.localRotation = Quaternion.Slerp(from, to, t);
	}

	
}
