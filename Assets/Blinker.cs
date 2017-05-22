using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinker : MonoBehaviour {

    // Use this for initialization
    Light lite;
    bool switching = false;
    private void Awake()
    {
        lite = GetComponent<Light>();
    }

    // Update is called once per frame
    
	
	// Update is called once per frame
	void Update () {
		if (!switching)
        {
            Invoke("turnLightOnOff", Random.Range(0.5f, 1.5f));
            switching = true;
        }
	}

    void turnLightOnOff()
    {
        lite.enabled = lite.enabled ? false : true;
        switching = false;
    }
}
