using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	float movementSpeed = 250;
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

	void Update()
	{
		
		isMoving = Movement();		
	}

	// Update is called once per frame
	void LateUpdate () 
	{
		ShipBobbing(isMoving);

		//Tilting:
		//transform.localRotation = Quaternion.Euler((Mathf.Lerp(transform.eulerAngles.x, transform.position.y, Time.time)) - WorldController.oceanTileOffset, transform.eulerAngles.y, 0f);
	}

	bool Movement()
	{
		float translation = Input.GetAxisRaw("Vertical") * movementSpeed;
		float rotation = Input.GetAxisRaw("Horizontal") * rotationSpeed;
		translation *= Time.deltaTime;
		rotation *= Time.deltaTime;
		transform.Translate(0, 0, translation);
		//transform.Rotate(0, rotation, 0);
		
		float x = transform.eulerAngles.x;
		float y = transform.eulerAngles.y;
		float z = transform.eulerAngles.z;
		Vector3 desiredRotation = new Vector3(x, y + rotation, z);
		transform.rotation = Quaternion.Euler(desiredRotation);


		return translation != 0;
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
	/// Sets the y portion of the boat equal to the bob of the waves
	/// </summary>
	void ShipBobbing(bool moving)
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
