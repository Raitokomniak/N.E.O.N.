using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    // Use this for initialization
    public float smoothing = 4;
    GameObject player;
    Camera cam;
    GameControllerScript gScript;
    void Awake () {
        cam = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }

    void Start()
    {
        this.transform.position = new Vector2(player.transform.position.x, player.transform.position.y);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gScript.isDead())
        {
            Vector2 targetPos = new Vector2(player.transform.position.x, player.transform.position.y);
            this.transform.position = Vector2.Lerp(this.transform.position, targetPos, smoothing * Time.deltaTime);
        }
    }

}
