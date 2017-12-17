using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : Entity {
    public delegate void OnInfoUpdated(int infamy, int gold);
    public event OnInfoUpdated OnInfoUpdatedEvent;

    public bool anchorDropped { get; private set; }

    [HideInInspector]
    public List<CrewMember> crew { get; private set; }

    public List<ISellable> Inventory { get; private set; }

    CameraFollow cam;

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

        //TODO: merge this list into inventory
        Consumables = new List<Consumable>();
        Speed = 4;
        MaxHealth = 15;
        CurrentHealth = MaxHealth;
        Infamy = 0;
        Gold = 0;

        Inventory = new List<ISellable>();
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
                //MainUIController.Instance.ToggleWorldUI(false);
                MainUIController.Instance.islandInteractionUI.SetIslandInfo(island.info.Name, "It is a bright and sunny day on " + island.info.Name);
                MainUIController.Instance.fadingIn = true;
                
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

    public void GatherResources(CrewMember crewMember, IslandInfo island)
    {
        //TODO replace hardcoded number with one dependent on crew member's skill
        for(int i = 0; i < 3; i++)
        {
            int resourceIndex = Random.Range(0, island.Resources.Count);
            if(island.Resources.Count > 0)
            {
                 Resource resource = island.Resources[resourceIndex];
                int amount = Random.Range(1, resource.Amount + 1);
                AddResource(resource, amount, island);
            }
        }
    }

    public void AddResource(Resource resource, int amount, IslandInfo island)
    {
        ISellable existingResource = null;
        resource.Amount-=amount;

        int containsIndex = Inventory.FindIndex(r => r.Name == resource.Name);

        if(containsIndex != -1)
            existingResource = Inventory.Where(r => r.Name == resource.Name).First();
            
        //Resource already in list
        if(existingResource != null)
        {
            existingResource.Amount += amount;
        }
        //Resource is not in list
        else 
        {
            Resource newResource = new Resource(resource);
            newResource.Amount = amount;
            Inventory.Add(newResource);
        }
    }

    //Temporary function to return a formatted string of the player's inventory
    public string FormattedInventory()
    {
        string s = "";
        foreach(var item in Inventory)
        {
            s += item.Name + "(" + item.Amount + ") ";
        }

        return s;
    }
	
}
