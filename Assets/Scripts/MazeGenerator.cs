// MazeGenerator.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {

    public GameObject cubeUnit;
    public GameObject pathBreadcrumb;
    public float unitSize;
    public Vector2I mazeSize;

    private List<GameObject> _blocks;
    private Grid _grid;

	// Use this for initialization
	void Start () {
        _blocks = new List<GameObject>();
        _grid = new Grid();
        cubeUnit.SetActive(false);
        Generate();
	}
	
	// Update is called once per frame
	void Update () {

        // regenerate maze?
        if (Input.GetKeyDown(KeyCode.R))
        {
            Generate();
            Vector2[] path = AStarPathFinder.FindPath(_grid, _grid.grid[1, 1], _grid.grid[mazeSize.x-2, mazeSize.y-2], false);

            // DEBUG: draw path
            string outStr = "";
            if (path != null)
            {
                foreach (Vector2 pt in path)
                {
                    outStr += pt;
                    GameObject block = Instantiate(pathBreadcrumb, new Vector3(pt.x, 0.5f, pt.y), Quaternion.identity);
                    block.SetActive(true);
                    block.name = "block(path)";
                    _blocks.Add(block);
                }
            }
            Debug.Log(outStr);
        }
	}

    // Generate Maze
    public void Generate()
    {
        // destroy old blocks
        _grid.InitGrid(mazeSize.x, mazeSize.y);
        foreach (GameObject go in _blocks)
            Destroy(go);

        // initial values
        Vector3 p0 = transform.position;
        float r = unitSize;
        float r2 = r * 2;
        Vector3 botLeft = p0 + new Vector3(-Mathf.Floor(mazeSize.x / 2) * r2, 0.5f, -Mathf.Floor(mazeSize.y / 2) * r2);

        // create empty maze to start with
        for (int x = 0; x < mazeSize.x; x++)
        {
            for (int y = 0; y < mazeSize.y; y++)
            {
                _grid.AddNode(new Node(botLeft + new Vector3(x * r2, 0, y * r2), new Vector2I(x, y), true, false));
            }
        }

        // create maze
        for (int x=0; x<mazeSize.x; x++)
        {
            for (int y = 0; y < mazeSize.y; y++)
            {
                bool isEdge = (x == 0 || x == mazeSize.x - 1 || y == 0 || y == mazeSize.y - 1);
                bool traversable = true;
                Vector3 p = botLeft + new Vector3(x * r2, 0, y * r2);

                // generate block if needed
                GameObject block = null;
                if (isEdge || Random.Range(0, 2) == 0)
                {                  
                    traversable = false;
                    block = Instantiate(cubeUnit, p, Quaternion.identity);
                    block.SetActive(true);
                    block.name = "block(" + x + ", " + y + ")";
                    _blocks.Add(block);
                }

                // add node to grid
                Node node = new Node(new Vector2(p.x, p.z), new Vector2I(x, y), traversable, isEdge);
                _grid.AddNode(node);

                // test path to make sure finish is accessible from start
                if (!traversable && !isEdge)
                {
                    Vector2[] path = AStarPathFinder.FindPath(_grid, _grid.grid[1, 1], _grid.grid[mazeSize.x - 2, mazeSize.y - 2], false);
                    if (path == null)
                    {
                        node.traversable = true;
                        if (block != null)
                        {
                            _blocks.Remove(block);
                            Destroy(block);
                        }
                    }
                }
            }
        }
    }
}
