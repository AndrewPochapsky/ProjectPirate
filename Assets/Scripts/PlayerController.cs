using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	float movementSpeed = 500;
	float rotationSpeed = 100;
	float surfaceModifier;

	Rigidbody rb;
	
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		surfaceModifier = 5;
	}
	
	// Update is called once per frame
	void Update () {
		//If not moving do bobbing

		Movement();
		ShipBobbing();
		

	}

	bool Movement()
	{
		float translation = Input.GetAxisRaw("Vertical") * movementSpeed;
		float rotation = Input.GetAxisRaw("Horizontal") * rotationSpeed;
		translation *= Time.deltaTime;
		rotation *= Time.deltaTime;
		transform.Translate(0, 0, translation);
		transform.Rotate(0, rotation, 0);

		return translation != 0;
	}

	float CalculateSurface(float x, float modifier)
	{
		float y = (Mathf.Sin(x * 1.0f + (Time.time) * 1.0f)
			+ Mathf.Sin(x * 2.3f + (Time.time) * 1.5f)
			+ Mathf.Sin(x * 3.3f + (Time.time)))
			* modifier;

		return y;
	}

	float CalculateSurfaceLite(float x, float modifier)
	{
		float y = Mathf.Sin(x + (Time.time));
		return y;
	}

	/// <summary>
	/// Sets the y portion of the boat equal to the bob of the waves
	/// </summary>
	void ShipBobbing()
	{
		Vector3 bobbing = new Vector3(transform.position.x, 
			CalculateSurface(transform.position.x, surfaceModifier) +
			CalculateSurface(transform.position.z, surfaceModifier) + WorldController.oceanTileOffset,
			transform.position.z);

		transform.position = bobbing;//Vector3.Lerp(transform.position, bobbing, 0.5f);
	}

	
}
