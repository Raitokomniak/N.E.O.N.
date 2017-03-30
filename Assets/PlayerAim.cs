using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour {

    // Use this for initialization
    PlayerMovement playMov;
    GameControllerScript gScript;
    SpriteRenderer sr;
	
    void Awake()
    {
        playMov = GetComponentInParent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }

    void Start()
    {
        sr.enabled = false;
    }
	// Update is called once per frame
	void Update () {

        if (Input.GetAxis("Aim") !=0 && !gScript.pauseOn)
        {
            aim();
            sr.enabled = true;
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0.001f, 30 * Time.deltaTime);
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }
        else
        {
            sr.enabled = false;
        }
    }

    void aim()
    {
        float x = Input.GetAxisRaw("RHorizontal");
        float y = Input.GetAxisRaw("RVertical") * -1;
        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        if (playMov.isFacingRight())
         {
            angle = Mathf.Clamp(angle, -80, 80);
         }

        Debug.Log(angle);
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void throwDagger()
    {

    }


}
