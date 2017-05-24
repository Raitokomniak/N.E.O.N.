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
    bool showRB;
    void Awake ()
    {
        lite = GetComponentInChildren<Light>();
        intensity = lite.intensity;
        player = GameObject.FindGameObjectWithTag("Player");
        playMov = player.GetComponent<PlayerMovement>();
        gHook = player.GetComponent<GrapplingHook>();
        rB.enabled = false;
        showRB = false;

    }
    void Start()
    {
        
    }

    void Update()
    {
        if (showRB)
        {
            rB.enabled = true;
            rB.color = Vector4.Lerp(rB.color, new Vector4(1, 1, 1, 1), 10 * Time.unscaledDeltaTime);
        }
        else
        {
            rB.color = Vector4.Lerp(rB.color, new Vector4(1, 1, 1, 0), 10 * Time.unscaledDeltaTime);

            if (rB.color.a < 0.2)
            {
                rB.enabled = false;
            }
        }
    }
	
	// Update is called once per frame
	void OnTriggerStay2D(Collider2D col) {
		if (col.gameObject == player&&playMov.gizmo())
        {
            if (this.gameObject == player.GetComponent<GrapplingHook>().spotShoot())
            {
                //   rB.enabled = true;
                showRB = true;
                setLight(0.5f);
            }
            else
            {
                // rB.enabled = false;
                showRB = false;
                setLight(0);
            }
            gHook.setGHookable(this.gameObject);
        }
	}

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == player && playMov.gizmo())
        {
            gHook.unSetGHookable(this.gameObject);
            showRB = false;
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
