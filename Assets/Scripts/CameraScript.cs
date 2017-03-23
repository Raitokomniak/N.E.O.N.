using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    // Use this for initialization
    public float smoothing = 4;
    
    GameObject player;
    Camera cam;
    GameControllerScript gScript;
    PlayerMovement playMov;
    bool follow;
    float z;

    void Awake () {
        cam = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
        playMov = player.GetComponent<PlayerMovement>();
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        z = this.transform.position.z;
        this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, z);
        follow = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gScript.isDead())
        {
            Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y, z);
            if (follow)
            {
            
            this.transform.position = Vector3.Lerp(this.transform.position, targetPos, smoothing * Time.deltaTime);
            }
            if (!playMov.playerMoving())
            {
                this.transform.position = Vector3.Lerp(this.transform.position, targetPos, smoothing * Time.deltaTime);
            }
        }
    }

    void LateUpdate()
    {
        float zValue = getDistance(z);
        
        this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(this.transform.position.x, this.transform.position.y, zValue), 2 * Time.deltaTime);
        
    }

    float getDistance(float value)
    {
        if (!playMov.playerMoving())
        {
            value = 1.25f * z;
        }
        return value;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == player)
        {
            follow = true;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == player&&Mathf.Approximately(this.transform.position.x, player.transform.position.x))
        {
            follow = false;

        }
    }

}
