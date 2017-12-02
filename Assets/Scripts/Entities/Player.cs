using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity {
    public delegate void OnInfoUpdated(int infamy, int gold);
    public event OnInfoUpdated OnInfoUpdatedEvent;

    public bool anchorDropped { get; private set; }

    CameraFollow cam;

    List<CrewMember> crew;

    private void Awake()
    {
        Attacks = new List<Attack>
        {
            new Attack("Basic Attack", 2, 3),
            new Attack("Super Attack", 5, 4)
        };

        crew = new List<CrewMember>
        {
            new CrewMember("Dave"),
            new CrewMember("Joe")
        };

        Consumables = new List<Consumable>();
        Speed = 4;
        MaxHealth = 15;
        CurrentHealth = MaxHealth;
        Infamy = 0;
        Gold = 0;
    }
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    protected override void Start()
    {   
        base.Start();
        cam = FindObjectOfType<CameraFollow>();
        cam.SetTarget(this.transform);
        if(OnInfoUpdatedEvent != null)
            OnInfoUpdatedEvent(Infamy, Gold);
    }
    
    void OnTriggerStay(Collider other)
    {
        IslandTile island = other.gameObject.GetComponent<IslandTile>(); 
        if(island != null)
        {
            if(WorldController.Instance.currentIsland == null)
            {
                 WorldController.Instance.currentIsland = island.info;
            }

            if(Input.GetKeyDown(KeyCode.E))
            {
                island.info.Visited = true;
                island.SetUI();
                anchorDropped = true;
                MainUIController.Instance.ToggleWorldUI(false);
                MainUIController.Instance.SetIslandInfo(island.info.Name, "It is a bright and sunny day on " + island.info.Name);
                //TODO: Don't do this
                cam.zoomOffset = cam.zoomValue;
                cam.SetTarget(island.transform);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        IslandTile island = other.gameObject.GetComponent<IslandTile>();
        if(island != null)
        {
            WorldController.Instance.currentIsland = null;
        }        
    }
	
    public void RaiseAnchor()
    {
        anchorDropped = false;
        cam.zoomOffset = Vector3.zero;
        cam.SetTarget(this.transform);
    }
	
}
