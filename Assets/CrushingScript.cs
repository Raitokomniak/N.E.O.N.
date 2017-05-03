using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrushingScript : MonoBehaviour {
    int crushers = 0;
    GameControllerScript gScript;
    // Use this for initialization
    void Awake()
    {
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }

    void Start ()
    {
        crushers = 0;
	}
	

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.isTrigger)
        {
            if (col.gameObject.tag == "Player")
            {
                gScript.crushing++;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (!col.isTrigger)
        {
            if (col.gameObject.tag == "Player")
            {
                gScript.crushing--;
            }
        }
    }
}
