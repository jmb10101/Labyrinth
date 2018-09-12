// MazeGenerator.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public Camera cam;
    public GameObject cubeUnit;
    public GameObject pathBreadcrumb;
    public GameObject seeker;
    public float unitSize;
    public Vector2I mazeSize;
    public float pathPtLerpSpeed;

    private List<GameObject> _blocks;
    private Grid _grid;
    private Vector2[] _path;
    private Transform _seeker;
    private int _pathPtSeekIdx;
    private float _pathPtProgress;
    private Vector3 _startPathPtPos;
    private bool _camFollow;
    private Vector3 _arialCamPos;

	// Use this for initialization
	void Start () {
        _blocks = new List<GameObject>();
        _grid = new Grid();
        cubeUnit.SetActive(false);
        _seeker = Instantiate(seeker, Vector3.zero, Quaternion.identity).GetComponent<Transform>();
        _seeker.gameObject.SetActive(true);
        _arialCamPos = cam.transform.position;

        Generate();
	}
	
	// Update is called once per frame
	void Update () {

        // regenerate maze?
        if (Input.GetKeyDown(KeyCode.R))
        {
            Generate();
        }

        // change camera angle?
        if (Input.GetKeyDown(KeyCode.C))
        {
            _camFollow = !_camFollow;
            if (!_camFollow)
                cam.transform.position = _arialCamPos;
        }

        // update seeker position along path
        if (_path != null && _pathPtSeekIdx < _path.Length)
        {
            Vector3 endPt = new Vector3(_path[_pathPtSeekIdx].x, 0.5f, _path[_pathPtSeekIdx].y);
            _pathPtProgress += Time.deltaTime * pathPtLerpSpeed;
            _seeker.position = Vector3.Lerp(_startPathPtPos, endPt, _pathPtProgress);

            if (_pathPtProgress >= 1)
            {
                _startPathPtPos = endPt;
                _pathPtSeekIdx++;
                _pathPtProgress = 0;
            }
        }

        // update cam position
        if (_camFollow)
        {
            cam.transform.position = new Vector3(_seeker.position.x, 10, _seeker.position.z);
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

        // create empty maze to start with (containing borders)
        for (int x = 0; x < mazeSize.x; x++)
        {
            for (int y = 0; y < mazeSize.y; y++)
            {
                bool isEdge = ((x == 0) || (x == mazeSize.x - 1) || (y == 0) || (y == mazeSize.y - 1));
                _grid.AddNode(new Node(botLeft + new Vector3(x * r2, 0, y * r2), new Vector2I(x, y), !isEdge, isEdge));
            }
        }

        // create maze
        for (int x=0; x<mazeSize.x; x++)
        {
            for (int y = 0; y < mazeSize.y; y++)
            {
                bool isEdge = ((x == 0) || (x == mazeSize.x - 1) || (y == 0) || (y == mazeSize.y - 1));
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
                if (!traversable)
                {
                    findPath();
                    if (_path == null)
                    {
                        // path does not exist after adding current block so we need to destroy it, make the node traversable, and move on
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

        // calculate path and set up for seeking
        findPath();
        if (_path != null)
        {
            // draw path
            foreach (Vector2 pt in _path)
            {
                GameObject block = Instantiate(pathBreadcrumb, new Vector3(pt.x, 0.5f, pt.y), Quaternion.identity);
                block.SetActive(true);
                block.name = "block(path)";
                _blocks.Add(block);
            }

            // reset path seeker values
            _seeker.position = botLeft + new Vector3(1 * r2, 0, 1 * r2);
            _pathPtSeekIdx = 0;
            _pathPtProgress = 0;
            _startPathPtPos = new Vector3(_path[0].x, 0.5f, _path[0].y);
        }
    }

    void findPath()
    {
        _path = AStarPathFinder.FindPath(_grid, _grid.grid[1, 1], _grid.grid[mazeSize.x - 2, mazeSize.y - 2], false);
    }
}
