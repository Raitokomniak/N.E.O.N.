using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeHandler : MonoBehaviour {

    // Use this for initialization
    PlayerMovement playerMov;
    Rigidbody2D playerRig;
    GameControllerScript gScript;
    float originalTimeScale;
    float originalFixedDeltaTime;
    bool fullSlowMo = false;

	void Awake() {
        playerMov = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerRig = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        gScript = GetComponent<GameControllerScript>();
        
    }
    void Start()
    {
        originalTimeScale = 1;
        originalFixedDeltaTime = 0.02f;
    }
	
	// Update is called once per frame
	void Update () {
        handleTime();
    }

    void handleTime()
    {
        if (!gScript.isDead())
        {
            if (!playerMov.playerMoving() && playerRig.velocity.sqrMagnitude < 1)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, 0.01f, 30 * Time.deltaTime);
                Time.fixedDeltaTime = Mathf.Lerp(Time.fixedDeltaTime, 0.02F * Time.timeScale, 30 * Time.fixedDeltaTime);
                fullSlowMo = true;
            }
            else if (!playerMov.playerMoving() && playerRig.velocity.y < 0)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, 0.1f, 10 * Time.deltaTime);
                Time.fixedDeltaTime = Mathf.Lerp(Time.fixedDeltaTime, 0.02F * Time.timeScale, 10 * Time.fixedDeltaTime);
                fullSlowMo = false;
            }
            else
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, originalTimeScale, 30 * Time.deltaTime);
                Time.fixedDeltaTime = Mathf.Lerp(Time.fixedDeltaTime, originalFixedDeltaTime, 30 * Time.fixedDeltaTime);
                fullSlowMo = false;
            }
        }
        else
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, originalTimeScale, 10 * Time.deltaTime);
            Time.fixedDeltaTime = Mathf.Lerp(Time.fixedDeltaTime, originalFixedDeltaTime, 10 * Time.fixedDeltaTime);
            fullSlowMo = false;
        }
    }

    public bool fullSlowMotion()
    {
        return fullSlowMo;
    }
    bool falling()
    {
        if (playerRig.velocity.x == 0 &&playerRig.velocity.y < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
