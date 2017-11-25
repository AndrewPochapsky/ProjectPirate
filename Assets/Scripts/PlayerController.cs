using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	float acceleration = 150;
	float maxSpeed = 300;
	float rotationSpeed = 80;
	float zRotationAmount = 6;

	float surfaceModifier;
	
	bool isMoving = false;

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

	void Movement()
	{
		float translation = Input.GetAxisRaw("Vertical");
		float rotation = Input.GetAxisRaw("Horizontal") * rotationSpeed;
		rotation *= Time.deltaTime;

		//Rotation
		Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, rotation, 0));
		rb.MoveRotation(rb.rotation * deltaRotation);

		//Acceleration
		if(translation != 0 )
		{
			rb.AddForce(transform.forward * acceleration * translation);
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
			//rb.AddForce(-transform.forward * acceleration);
			rb.velocity *= 0.9f;
		}

		//Rotation code that also works
		/*float x = transform.eulerAngles.x;
		float y = transform.eulerAngles.y;
		float z = transform.eulerAngles.z;
		Vector3 desiredRotation = new Vector3(x, y + rotation, z);
		transform.rotation = Quaternion.Euler(desiredRotation);*/
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
			CalculateSurface((transform.position.z), surfaceModifier) + WorldController.oceanTileOffset,
			transform.position.z);
		
		transform.position = Vector3.SmoothDamp(transform.position, bobbingMotion, ref velocity, smoothTime: 0.2f);

		Quaternion from = Quaternion.Euler(zRotationAmount, 90, 0);
		Quaternion to =Quaternion.Euler(-zRotationAmount, 90, 0);
		float t = Mathf.PingPong(Mathf.Sin(Time.time * 0.5f) + Mathf.Sin(Time.time * 0.35f), 1);

		model.localRotation = Quaternion.Slerp(from, to, t);
	}

	
}
