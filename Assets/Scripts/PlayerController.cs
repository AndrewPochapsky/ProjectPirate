using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	float  surfaceModifier;
	
	// Use this for initialization
	void Start () {
		surfaceModifier = 5;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(transform.position.x, 
			CalculateSurface(transform.position.x, surfaceModifier) +
			CalculateSurface(transform.position.z, surfaceModifier) + WorldController.oceanTileOffset,
			transform.position.z);
	}

	float CalculateSurface(float x, float modifier)
	{
		float y = (Mathf.Sin(x * 1.0f + (Time.time) * 1.0f)
			+ Mathf.Sin(x * 2.3f + (Time.time) * 1.5f)
			+ Mathf.Sin(x * 3.3f + (Time.time)))
			* modifier;

		return y;
	}

	/*float calculateSurface(float x, float modifier) {
   		 	float y = (sin(x * 1.0 + (_Time[1] - START_TIME) * 1.0) 
				+ sin(x * 2.3 + (_Time[1] - START_TIME) * 1.5) 
				+ sin(x * 3.3 + (_Time[1] - START_TIME) )) 
				* modifier	;
    		return y;
		} */
}
