using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentationScript : MonoBehaviour {

    // Use this for initialization
    GameObject player;
    Light lite;
    string speech = "event:/Environmental sounds/Chamber of Horrors ad";
    FMOD.Studio.EventInstance ad;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        lite = this.transform.parent.gameObject.GetComponentInChildren<Light>();
        lite.enabled = false;
        ad = FMODUnity.RuntimeManager.CreateInstance(speech);
    }
	
	void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == player)
        {
            ad.start();
            GetComponent<BoxCollider2D>().enabled = false;
            lite.enabled = true;
        }
    }
}
