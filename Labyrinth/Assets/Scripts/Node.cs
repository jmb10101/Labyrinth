// Node.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    public Vector2 worldPosition;
    public Vector2I gridCoords;
    public bool traversable;
    public int gCost;
    public int hCost;
    public Node parent;

    // properties
    public int fCost { get { return gCost + hCost; } }

    // constructor
    public Node(Vector2 _worldPosition, Vector2I _gridCoords, bool _traversable)
    {
        worldPosition = _worldPosition;
        gridCoords = _gridCoords;
        traversable = _traversable;
    }
}
