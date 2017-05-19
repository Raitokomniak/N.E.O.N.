using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VantagePointScript : MonoBehaviour {

    // Use this for initialization
    GameObject player;
    GrapplingHook gHook;
    Light lite;
    float intensity;
    PlayerMovement playMov;
    void Awake ()
    {
        lite = GetComponentInChildren<Light>();
        intensity = lite.intensity;
        player = GameObject.FindGameObjectWithTag("Player");
        playMov = player.GetComponent<PlayerMovement>();
        gHook = player.GetComponent<GrapplingHook>();

    }
    void Start()
    {
        
    }
	
	// Update is called once per frame
	void OnTriggerStay2D(Collider2D col) {
		if (col.gameObject == player&&playMov.gizmo())
        {
            gHook.setGHookable(this.gameObject);
        }
	}

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == player && playMov.gizmo())
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
