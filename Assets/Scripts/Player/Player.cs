using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO: create a new subset of classes devoted to battle scene only
//call them something link BattlePlayer, BattleEnemy, base class can be called BattleEntity
//Also, refactor normal Entity class to be just for sea based entities

public class Player : Entity {
    public delegate void OnInfoUpdated(int infamy, int gold);
    public event OnInfoUpdated OnInfoUpdatedEvent;

    public delegate void OnNewTileEntered(List<BaseNode> nodes);
    public event OnNewTileEntered OnNewTileEnteredEvent;

    public bool anchorDropped { get; private set; }

    CameraFollow cam;

    private void Awake()
    {
        entityData = new EntityData();
        
        entityData.Attacks.Add(new Attack("Basic Attack", 2, 3));
        entityData.Attacks.Add(new Attack("Super Attack", 5, 4));
        
        entityData.Crew.Add(new CrewMember("Joe"));
        entityData.Crew.Add(new CrewMember("Dave"));

        //TODO: merge this list into inventory
        //Note: Requires editing the enemy AI thing
        entityData.Consumables = new List<Consumable>();
        entityData.Speed = 4;
        entityData.MaxHealth = 1;
        entityData.CurrentHealth = base.entityData.MaxHealth;
        entityData.Infamy = 0;
        entityData.Gold = 0;

       
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {   
        cam = FindObjectOfType<CameraFollow>();
        cam.SetTarget(this.transform);
        if(OnInfoUpdatedEvent != null)
            OnInfoUpdatedEvent(base.entityData.Infamy, base.entityData.Gold);
    }
    
    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        BaseNode node = other.transform.GetComponentInParent<BaseNode>();
        Enemy enemy = other.GetComponent<Enemy>();
        if (node != null)
        {
            List<BaseNode> nodes = WorldController.Instance.GetNodesNearPlayer(this.transform, node);
            OnNewTileEnteredEvent(nodes);
        }
        
        if(enemy != null && !enemy.dead)
        {
            BattleData battleData = Resources.Load<BattleData>("Data/BattleData");
            LocalData localData = Resources.Load<LocalData>("Data/LocalData");

            battleData.ResetData();
            battleData.Friendlies.Add(this.entityData);
            battleData.Enemies.Add(enemy.entityData);
            battleData.enemyObject = enemy.gameObject;

            localData.playerShipPos = transform.position;
            
            MainUIController.Instance.fadingInPanel = true;
            MainUIController.Instance.scene = "Battle";

            //Temporary remove later
            //enemy.gameObject.SetActive(false);
        }
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
                MainUIController.Instance.fadingInIslandUI = true;
                
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

        int containsIndex = entityData.Inventory.FindIndex(r => r.Name == resource.Name);

        if(containsIndex != -1)
            existingResource = entityData.Inventory.Where(r => r.Name == resource.Name).First();
            
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
            entityData.Inventory.Add(newResource);
        }
    }

    //Temporary function to return a formatted string of the player's inventory
    public string FormattedInventory()
    {
        string s = "";
        foreach(var item in entityData.Inventory)
        {
            s += item.Name + "(" + item.Amount + ") ";
        }

        return s;
    }
	
}
