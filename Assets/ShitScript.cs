using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShitScript : MonoBehaviour {

    // Use this for initialization
    public GameObject bubble;
    public float startTime = 5f;
	void Awake () {
        // bubble.enabled = false;
        bubble.SetActive(false);
        StartCoroutine(start());
	}
	
	// Update is called once per frame
	IEnumerator start () {
        yield return new WaitForSecondsRealtime(startTime);
        bubble.SetActive(true);
	}
}
