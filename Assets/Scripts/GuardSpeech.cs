using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSpeech : MonoBehaviour {

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Enemy sounds/Guard (cyborg grunt)/Vocalizations/Acknowledged");
        }
    }
}
