using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public delegate void OnEndTurn();
    public event OnEndTurn OnEndTurnEvent; 

    /// <summary>
    /// Number of grid spaces that an enemy can move
    /// </summary>
    protected int Speed { get; set; }
    protected int MaxHealth { get; set; }
    public int CurrentHealth { get; protected set; }

    public bool canMove = true;
    protected bool reached = false;

    [HideInInspector]
    public Node nodeParent;

    float movementSpeed;

    Node nextLocation;
    protected List<Node> pathNodes;

    int currentNodeIndex = 0;
    public bool IsMoving { get; private set; } = false;

    protected virtual void Start()
    {
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
            if (currentNodeIndex + 1 <= pathNodes.Count)
            {
                if (ReachedNode(pathNodes[currentNodeIndex]))
                {
                    currentNodeIndex++;
                    return pathNodes[currentNodeIndex];
                }
            }
            else
            {
                transform.parent = pathNodes[pathNodes.Count - 1].transform;
                nodeParent = GetComponentInParent<Node>();
                IsMoving = false;
                reached = true;
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
        
        //TODO maybe use an event instead of doing this here
        foreach(Node node in _nodes)
        {
            node.Child.Deselect();
        }
        reached = false;
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
