// Grid.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public static class AStarPathFinder
{
    // global props
    public static int maxMSAllowedPerSearch = 32;

    // pathfinding
    public static Vector2[] FindPath(Grid grid, Node startNode, Node endNode, bool simplePath = true)
    {
        // if the start or end node is not traversable, then exit now since we cant get to it
        if (!startNode.traversable || !endNode.traversable)
            return null;

        Stopwatch sw = new Stopwatch();
        sw.Start();

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        List<Node> neighbors = new List<Node>();

        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            if (sw.ElapsedMilliseconds > maxMSAllowedPerSearch)
            {
                //UnityEngine.Debug.Log("Max time reached for A* search!");
                return null;
            }

            // find node in open set with lowest f cost (and set to current)
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || // if fCost are equal, check for lowest hCost
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    currentNode = openSet[i];
            }

            // remove current node from the openSet and add it to the closedSet
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // check if path has been found
            if (currentNode == endNode)
                return RetracePath(startNode, endNode, simplePath);

            // loop through each neighbor of the current node
            grid.Neighbors(currentNode, neighbors);
            foreach (Node neighbor in neighbors)
            {
                // if neighbor is not traversable or neighbor is in closedSet, skip to next neighbor
                if (!neighbor.traversable || closedSet.Contains(neighbor))
                    continue;

                // check if new path to neighbor is shorter OR neighbor is not in openSet
                int newCostToNeighbor = currentNode.gCost + Distance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    // set fCost of neighbor
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = Distance(neighbor, endNode);

                    // set parent of neighbor to current
                    neighbor.parent = currentNode;

                    // if neighbor is not in openSet, add it to openSet
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
        return null;
    }

    // retrace path
    public static Vector2[] RetracePath(Node startNode, Node endNode, bool simplePath = true)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode && currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        // simplify node list and convert to Vector2 array
        return SimplifyPath(path, simplePath);
    }

    // Simplify path
    public static Vector2[] SimplifyPath(List<Node> path, bool simplePath = true)
    {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2I oldDir = new Vector2I();

        for (int i = 1; i < path.Count; i++)
        {
            // node is waypoint if changing direction or first or last node
            Vector2I newDir = path[i - 1].gridCoords - path[i].gridCoords;
            if (newDir != oldDir || i == (path.Count - 1) || !simplePath)
                waypoints.Add(path[i].worldPosition);

            oldDir = newDir;
        }
        return waypoints.ToArray();
    }

    // Get distance between 2 nodes
    public static int Distance(Node nodeA, Node nodeB)
    {
        // d(x, y) = 14y + 10(x-y) <- if x > y
        int dx = Mathf.Abs(nodeA.gridCoords.x - nodeB.gridCoords.x);
        int dy = Mathf.Abs(nodeA.gridCoords.y - nodeB.gridCoords.y);

        if (dx > dy)
            return (14 * dy + 10 * (dx - dy));

        return (14 * dx + 10 * (dy - dx));
    }

}
