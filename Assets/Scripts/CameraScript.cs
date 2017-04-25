using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    // Use this for initialization
    public float smoothing = 4;
    
    GameObject player;
    GameControllerScript gScript;
    PlayerMovement playMov;
    float z;
    bool usedFromOutside;

    void Awake () {
        player = GameObject.FindGameObjectWithTag("Player");
        playMov = player.GetComponent<PlayerMovement>();
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        z = this.transform.position.z;
        this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, z);
        usedFromOutside = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gScript.isDead()&&!usedFromOutside)
        {
           Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y, z);
           this.transform.position = Vector3.Lerp(this.transform.position, targetPos, smoothing * Time.deltaTime);
        }
    }

    void LateUpdate()
    {
        if (!usedFromOutside)
        {
            float zValue = getDistance(z);
            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(this.transform.position.x, this.transform.position.y, zValue), 1.5f * Time.deltaTime);
        }
    }


    float getDistance(float value)
    {
        if (!playMov.playerMoving())
        {
            value = 1.25f * z;
        }
        return value;
    }

    public void usedSomeWhereElse(bool option)
    {

    }


}
