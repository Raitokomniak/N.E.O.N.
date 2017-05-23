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
    public SpriteRenderer rB;
    void Awake ()
    {
        lite = GetComponentInChildren<Light>();
        intensity = lite.intensity;
        player = GameObject.FindGameObjectWithTag("Player");
        playMov = player.GetComponent<PlayerMovement>();
        gHook = player.GetComponent<GrapplingHook>();
        rB.enabled = false;

    }
    void Start()
    {
        
    }
	
	// Update is called once per frame
	void OnTriggerStay2D(Collider2D col) {
		if (col.gameObject == player&&playMov.gizmo())
        {
            if (this.gameObject == player.GetComponent<GrapplingHook>().spotShoot())
            {
                rB.enabled = true;
                setLight(3);
            }
            else
            {
                rB.enabled = false;
                setLight(1);
            }
            gHook.setGHookable(this.gameObject);
        }
	}

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == player && playMov.gizmo())
        {
            gHook.unSetGHookable(this.gameObject);
            rB.enabled = false;
            setLight(intensity);
        }
    }

    public void setRB(bool option)
    {
        rB.enabled = option;
    }
    
    public void setLight(float intes)
    {
        lite.intensity = intes;
    }
}
