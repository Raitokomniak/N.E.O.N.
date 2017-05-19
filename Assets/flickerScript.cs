using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flickerScript : MonoBehaviour {
    public Light source;
    float rand;
    public float max;
    public float min;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        rand = Random.Range(min, max);
        source.intensity = rand;
	}
}
