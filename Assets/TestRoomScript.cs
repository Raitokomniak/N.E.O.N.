using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRoomScript : MonoBehaviour {

    // Use this for initialization
    public GameObject[] testTubes;
    List<Light> testTubeLights;

	void Awake () {
        testTubeLights = new List<Light>();
        foreach (GameObject tube in testTubes)
        {
            testTubeLights.Add(tube.GetComponent<Light>());
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
