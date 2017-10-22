using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public delegate void OnEndTurn();
    public event OnEndTurn OnEndTurnEvent; 

    /// <summary>
    /// Number of grid spaces that an enemy can move
    /// </summary>
    public int Speed { get; protected set; }
    protected int MaxHealth { get; set; }
    public int CurrentHealth { get; protected set; }

    [HideInInspector]
    public bool canMove = true;

    public List<Attack> Attacks { get; protected set; }
    public List<Consumable> Consumables { get; protected set; }

    [HideInInspector]
    public Node nodeParent;

    float movementSpeed;

    Node nextLocation;
    protected List<Node> pathNodes;

    int currentNodeIndex = 0;
    public bool IsMoving { get; private set; } = false;

    protected virtual void Start()
    {
        pathNodes = new List<Node>();
        nodeParent = GetComponentInParent<Node>();
        movementSpeed = 200 * Time.deltaTime;
    }

    private void Update()
    {
        HandleMoving();
    }

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
        if(pathNodes!= null)
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
                if(pathNodes.Count > 0)
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
        currentNodeIndex = 0;
        pathNodes = _nodes;
        nextLocation = _nodes[currentNodeIndex];

        IsMoving = true; 
    }

    /// <summary>
    /// Moves this to specified location
    /// </summary>
    /// <param name="location">Location to move to</param>
    private void MoveToLocation(Node location)
    {
        if(location!=null && IsMoving)
            transform.position = Vector3.MoveTowards(transform.position, location.transform.position, movementSpeed);
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
    {
        nodeParent = GetComponentInParent<Node>();
        nodeParent.isTraversable = false;
    }

    public void ModifyHealth(int value)
    {
        CurrentHealth += value;
        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;
    }
       
    protected void RaiseEndTurnEvent()
    {
        OnEndTurnEvent();
    }
   
}
