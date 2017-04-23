using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTubeScript : MonoBehaviour {

    // Use this for initialization
    Light tubeLight;
    GameObject player;

    void Awake () {
        tubeLight = this.transform.parent.gameObject.GetComponentInChildren<Light>();
        player = GameObject.FindGameObjectWithTag("Player");
        tubeLight.enabled = false;
	}

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            tubeLight.enabled = true;
        }
    }
}
