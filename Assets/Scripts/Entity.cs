using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    float speed;

    Node nextLocation;
    List<Node> nodes;

    int currentNodeIndex = 0;
    public bool IsMoving { get; private set; } = false;

    private void Start()
    {
        speed = 200 * Time.deltaTime;
    }

    /// <summary>
    /// Gets the next location to move to from list
    /// </summary>
    public void GetNextLocation()
    {
        if(nodes!= null)
        {
            if (currentNodeIndex + 1 <= nodes.Count)
            {
                if (Mathf.Round(transform.position.x) == Mathf.Round(nodes[currentNodeIndex].transform.position.x) &&
                   Mathf.Round(transform.position.z) == Mathf.Round(nodes[currentNodeIndex].transform.position.z))
                {
                    print("reached");
                    currentNodeIndex++;
                    nextLocation = nodes[currentNodeIndex];
                }
            }
            else
            {
                IsMoving = false;
                transform.parent = nodes[nodes.Count - 1].transform;
            }
               
        }
        
    }

    /// <summary>
    /// Sets the path of nodes
    /// </summary>
    /// <param name="_nodes">List of nodes which is set to this.nodes</param>
    public void SetPathNodes(List<Node> _nodes)
    {
        currentNodeIndex = 0;
        nodes = _nodes;
        nextLocation = _nodes[currentNodeIndex];
        
        //TODO maybe use an event instead of doing this here
        foreach(Node node in _nodes)
        {
            node.Child.Deselect();
        }

        IsMoving = true;
    }

    /// <summary>
    /// Moves this to specified location
    /// </summary>
    /// <param name="location">Location to move to</param>
    private void MoveToLocation(Node location)
    {
        if(IsMoving)
            transform.position = Vector3.MoveTowards(transform.position, location.transform.position, speed);
    }

    private void Update()
    {
        GetNextLocation();
        MoveToLocation(nextLocation);
    }

    
}
