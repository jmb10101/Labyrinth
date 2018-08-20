using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cubifier : MonoBehaviour {

    private float _counter;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        _counter += Time.deltaTime;

        float spawnInterval = 0.5f;
        if (_counter > spawnInterval)
        {
            _counter = 0f;
            //GameObject go = Instantiate(gameObject, transform.position, Quaternion.identity);
            //Destroy(go.GetComponent<Cubifier>());
        }
	}
}
