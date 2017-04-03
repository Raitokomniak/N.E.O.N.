using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    GameControllerScript gScript;
    bool flag = false;
    //not in use
    void Awake()
    {
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !flag)
        {
            flag = true;
            gScript.setCheckpoint(this.transform.position);
        }
    }
}
