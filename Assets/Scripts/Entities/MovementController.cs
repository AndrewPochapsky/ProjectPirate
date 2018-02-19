using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour {

	protected float SideTiltAmount { get; set; }
	protected float FrontTiltAmount { get; set; }

	protected Rigidbody rb;
	protected Player player;
	protected float surfaceModifier;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
	}

	// Use this for initialization
	protected virtual void Start () {
		surfaceModifier = FindObjectOfType<OceanTile>().GetComponent<Renderer>().material.GetFloat("_OceanWaveModifier");
	}
	
	protected virtual void FixedUpdate()
	{
		Movement();
		PassiveMovment();
	}

	protected float CalculateSurface(float x, float modifier)
    {
        float y = (Mathf.Sin(x * 1.0f + (Time.timeSinceLevelLoad) * 1.0f)
            + Mathf.Sin(x * 2.3f + (Time.timeSinceLevelLoad) * 1.5f)
            + Mathf.Sin(x * 3.3f + (Time.timeSinceLevelLoad)))
            * modifier;

        return y;
    }

	protected virtual void Movement(Transform target = null)
	{
		//Generic enemy movement?
	}
	

    /// <summary>
    /// The movement of the boat which happens all of the time.
    /// 
    /// Bobbing up and down, tilting side to side
    /// </summary>
    protected virtual void PassiveMovment(Transform _model = null)
    {
        Vector3 velocity = Vector3.zero;
        Vector3 bobbingMotion = new Vector3(transform.position.x,
            CalculateSurface((transform.position.x), surfaceModifier) +
            CalculateSurface((transform.position.z), surfaceModifier) +
            WorldController.Instance.oceanTileOffset,
            transform.position.z);

		if(_model != null)
		{
			_model.position = Vector3.SmoothDamp(_model.position, bobbingMotion, ref velocity, smoothTime: 0.2f);
		}
		else
		{
			transform.position = Vector3.SmoothDamp(transform.position, bobbingMotion, ref velocity, smoothTime: 0.2f);
		}


        Quaternion from = Quaternion.Euler(SideTiltAmount, 90, FrontTiltAmount);
        Quaternion to = Quaternion.Euler(-SideTiltAmount, 90, -FrontTiltAmount);
        float t = Mathf.PingPong(Mathf.Sin(Time.time * 0.5f) + Mathf.Sin(Time.time * 0.35f), 1);
		if(_model != null)
        	_model.localRotation = Quaternion.Slerp(from, to, t);
    }


}
