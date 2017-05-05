using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSpeech : MonoBehaviour {

    GameControllerScript gScript;

    void Awake()
    {
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (gScript.allGuardsAlerted())
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Enemy sounds/Guard (cyborg grunt)/Vocalizations/Engaging", transform.position);
            }
            else
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Enemy sounds/Guard (cyborg grunt)/Vocalizations/Acknowledged", transform.position);
            }        
        }
    }
}
