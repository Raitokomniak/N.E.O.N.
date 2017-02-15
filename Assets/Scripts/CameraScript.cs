using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    // Use this for initialization
    public float smoothing = 4;
    GameObject player;
    TimeHandler timeHandler;
    Camera cam;
    float originalorthoSize;
    float distance;
    float doubleOrtho;
    Vector3 offset;
    GameControllerScript gScript;
    void Awake () {
        cam = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
        timeHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<TimeHandler>();
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }

    void Start()
    {
        distance = this.transform.position.z;
        originalorthoSize = cam.orthographicSize;
        doubleOrtho = originalorthoSize * 2;
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
    void LateUpdate()
    {
       /* if (timeHandler.fullSlowMotion())
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, doubleOrtho, smoothing * Time.deltaTime);
        }
        else
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, originalorthoSize, smoothing * Time.deltaTime);
        }*/
    }
}
