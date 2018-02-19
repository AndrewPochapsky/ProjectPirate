using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEntity : MonoBehaviour {

	public delegate void OnEndTurn();
    public event OnEndTurn OnEndTurnEvent;

    public delegate void OnEntityDeath(BattleEntity entity);
    public event OnEntityDeath OnEntityDeathEvent;

    public EntityData data { get; set; }
    private StatusIndicator statusIndicator;

    [HideInInspector]
    public bool canMove = true;

    public Node nodeParent;
	/// <summary>
	/// Speed on grid
	/// </summary>
	float movementSpeed;
    Node nextLocation;
    protected List<Node> pathNodes;

    int currentNodeIndex = 0;
    public bool IsMoving { get;  set; } = false;

    protected virtual void Start()
    {
        //data = new EntityData();
        pathNodes = new List<Node>();
        nodeParent = GetComponentInParent<Node>();
        movementSpeed = 200; 
        try
        {
            statusIndicator = transform.GetChild(0).GetComponent<StatusIndicator>();
        }
        catch(UnityException)
        {
            Debug.LogError(this.name + " doesnt have a status indicator!");
        }
       
    }

    protected virtual void Update()
    {
        HandleMoving();
    }

    /// <summary>
    /// Movement for grid
    /// </summary>
    private void HandleMoving()
    {
        if (nextLocation != null && ReachedNode(nextLocation))
            nextLocation = GetNextLocation();

        MoveToLocation(nextLocation);
    }

    /// <summary>
    /// Returns the next location to move to from list
    /// </summary>
    /// <returns>The location</returns>
    public Node GetNextLocation()
    {
        if (pathNodes != null)
        {
            if (currentNodeIndex + 1 < pathNodes.Count)
            {
                if (ReachedNode(pathNodes[currentNodeIndex]) && currentNodeIndex + 1 != pathNodes.Count)
                {
                    currentNodeIndex++;
                    return pathNodes[currentNodeIndex];
                }
            }
            else
            {
                if (pathNodes.Count > 0)
                {
                    nodeParent.isTraversable = true;
                    transform.parent = pathNodes[pathNodes.Count - 1].transform;
                    nodeParent = GetComponentInParent<Node>();
                    pathNodes = new List<Node>();
                    IsMoving = false;
                }
            }

        }
        return nextLocation;
    }

    /// <summary>
    /// Sets the path of nodes
    /// </summary>
    /// <param name="_nodes">List of nodes which is set to this.nodes</param>
    public void SetPathNodes(List<Node> _nodes)
    {
        if(_nodes != null)
        {
            currentNodeIndex = 0;
            pathNodes = _nodes;

            nextLocation = _nodes[currentNodeIndex];
            IsMoving = true;
        }
    }

    /// <summary>
    /// Moves this to specified location on a grid
    /// </summary>
    /// <param name="location">Location to move to</param>
    private void MoveToLocation(Node location)
    {
        if (location != null && IsMoving)
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(location.transform.position.x,
                transform.position.y, location.transform.position.z), movementSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Returns true when rounded x and z floats are equal to node
    /// </summary>
    /// <param name="node">The node</param>
    /// <returns>True if reached l</returns>
    private bool ReachedNode(Node node)
    {
        return Mathf.Round(transform.position.x) == Mathf.Round(node.transform.position.x) &&
                   Mathf.Round(transform.position.z) == Mathf.Round(node.transform.position.z);
    }

    public void RefreshParent()
    {   if(this != null)
        {
            nodeParent = GetComponentInParent<Node>();
            nodeParent.isTraversable = false;
        } 
    }

    public void ModifyHealth(int value)
    {
        data.CurrentHealth += value;
        if (data.CurrentHealth > data.MaxHealth)
            data.CurrentHealth = data.MaxHealth;
        
        if(statusIndicator != null)
            statusIndicator.SetHealth();

        if(data.CurrentHealth <= 0)
            OnEntityDeathEvent(this);
    }

    protected void RaiseEndTurnEvent()
    {
        canMove = true;
        OnEndTurnEvent();
    }
}
