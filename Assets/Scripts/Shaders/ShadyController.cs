using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadyController : MonoBehaviour {

	public GameObject plane;
	private GameObject originalPlane;

	bool done = false;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	private void Awake()
	{
		Shader.SetGlobalFloat("BEGIN_WAVES", 1);
	}
	
	// Use this for initialization
	void Start () {
		originalPlane = Instantiate(plane, new Vector3(9, 0, 0), Quaternion.identity) as GameObject;
		originalPlane.GetComponent<Renderer>().materials[0].SetFloat("isIsland", 1);
		
		Invoke("CreatePlane", 2);
	}
	
	// Update is called once per frame
	void Update () {
		if(!done)
			Shader.SetGlobalFloat("START_TIME", Time.time);
		else
		{
			Shader.SetGlobalFloat("START_TIME",0);
		}
		
	}
	void CreatePlane(){
		print("Spawning");
		GameObject obj = Instantiate(plane, new Vector3(769, 0, 0), Quaternion.identity) as GameObject;
		done = true;
		//obj.transform.localScale = new Vector3(76, 0, 76);
		//obj.GetComponent<Renderer>().materials[0].SetFloat("_StartTime", Time.timeSinceLevelLoad);
		//print("New plane's StartTime: " + obj.GetComponent<Renderer>().materials[0].GetFloat("START_TIME"));
		//print("Old plane's StartTime: " + originalPlane.GetComponent<Renderer>().materials[0].("START_TIME"));
		//obj.GetComponent<Renderer>().sharedMaterials[0].SetGlobalFloat("_BeginWaves", 1);
		//Shader.SetGlobalFloat("BEGIN_WAVES", 1);
	}
}
