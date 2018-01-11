using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity {
    
    Transform player;
    [HideInInspector]
    public Transform model;
    public bool dead = false;
    private int speed = 120;

    new Rigidbody rigidbody;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        model = transform.GetChild(0);
    }

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		entityData = new EntityData();
		entityData.Speed = 3;
        entityData.MaxHealth = 10;
        entityData.CurrentHealth = entityData.MaxHealth;

        entityData.Attacks = new List<Attack>
        {
            new Attack("Basic Attack", 2, 4),
            new Attack("Super Attack", 3, 4)
        };

        entityData.Consumables = new List<Consumable>
        {
            new HealingConsumable("Basic Potion", 3),
            new HealingConsumable("Super Potion", 6)
        };

        player = GameObject.FindObjectOfType<Player>().transform;
        rigidbody = GetComponent<Rigidbody>();
        
        
	}

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        Vector3 destination = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        transform.LookAt(player);
        rigidbody.MovePosition(destination);
    }
}
