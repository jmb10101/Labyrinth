// Grid.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid {

    private Node[,] _grid;
    private Vector2I _gridSize;

    // properties
    public Node[,] grid { get { return _grid; } }
    public Vector2I gridSize { get { return _gridSize; } }

    public void InitGrid(int x, int y)
    {
        _grid = new Node[x, y];
        _gridSize = new Vector2I(x, y);
    }

    public void AddNode(Node node)
    {
        _grid[node.gridCoords.x, node.gridCoords.y] = node;
    }

    public void Neighbors(Node node, List<Node> neighborList)
    {
        neighborList.Clear();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)   // skip current node iteration (center of 9 block)
                    continue;

                // skip corners?
                if ((x == -1 && y == 1) || (x == 1 && y == 1) || (x == -1 && y == -1) || (x == 1 && y == -1))
                    continue;

                // ensure the coord we want to check is within the grid bounds
                    Vector2I checkCoord = new Vector2I(node.gridCoords.x + x, node.gridCoords.y + y);
                if (checkCoord.x >= 0 && checkCoord.x < _gridSize.x &&
                    checkCoord.y >= 0 && checkCoord.y < _gridSize.y)
                {
                    // neighbor within grid bounds, add to list
                    neighborList.Add(_grid[checkCoord.x, checkCoord.y]);
                }
            }
        }
    }

    public override string ToString()
    {
        string outStr = "";
        for (int y = _gridSize.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                outStr += _grid[x, y].traversable ? "0" : "1";
            }
            outStr += "\n";
        }
        return outStr;
    }
}
