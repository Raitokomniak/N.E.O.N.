using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VantagePointScript : MonoBehaviour {

    // Use this for initialization
    GameObject player;
    GrapplingHook gHook;
    Light lite;
    float intensity;
    void Awake ()
    {
        lite = GetComponentInChildren<Light>();
        intensity = lite.intensity;
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gHook = player.GetComponent<GrapplingHook>();
    }
	
	// Update is called once per frame
	void OnTriggerStay2D(Collider2D col) {
		if (col.gameObject == player)
        {
            gHook.setGHookable(this.gameObject);
        }
	}

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == player)
        {
            gHook.unSetGHookable(this.gameObject);
            setLight(intensity);
        }
    }
    
    public void setLight(float intes)
    {
        lite.intensity = intes;
    }
}
