using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {

    public GameObject cubeUnit;
    public float unitSize;
    public Vector2 mazeSize;

    private List<GameObject> _blocks;

	// Use this for initialization
	void Start () {
        _blocks = new List<GameObject>();
        Generate();
	}
	
	// Update is called once per frame
	void Update () {

        // regenerate maze?
        if (Input.GetKeyDown(KeyCode.R))
        {
            Generate();
        }
	}

    // Generate Maze
    public void Generate()
    {
        // destroy old blocks
        foreach (GameObject go in _blocks)
            Destroy(go);

        // initial values
        Vector3 p0 = transform.position;
        float r = unitSize;
        float r2 = r * 2;
        Vector3 topLeft = p0 + new Vector3( -Mathf.Floor(mazeSize.x / 2) * r2, 0, Mathf.Floor(mazeSize.y / 2) * r2);

        // create maze
        for (int x=0; x<mazeSize.x; x++)
        {
            for (int y = 0; y < mazeSize.y; y++)
            {
                bool isEdge = (x == 0 || x == mazeSize.x - 1 || y == 0 || y == mazeSize.y - 1);
                if (!isEdge && Random.Range(0, 2) == 0)
                    continue;

                Vector3 p = topLeft + new Vector3(x * r2, 0, -y * r2);
                GameObject block = Instantiate(cubeUnit, p, Quaternion.identity);
                _blocks.Add(block);
            }
        }
    }
}
