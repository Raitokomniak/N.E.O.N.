using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedboxCollision : MonoBehaviour {
    GameObject player;
    PlayerHealth health;
    FMOD.ATTRIBUTES_3D Attributes;
    FMOD.Studio.EventInstance electricSound;
    void Awake()
    {
        Vector3 position = transform.position;
        Attributes = FMODUnity.RuntimeUtils.To3DAttributes(position);
        player = GameObject.FindGameObjectWithTag("Player");
        health = player.GetComponent<PlayerHealth>();
        electricSound = FMODUnity.RuntimeManager.CreateInstance("event:/Environmental sounds/Electric trap");
        electricSound.set3DAttributes(Attributes);
        electricSound.start();
        //FMODUnity.RuntimeManager.PlayOneShot("event:/Environmental sounds/Electric trap", transform.position);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.isTrigger)
        {
            if (col.gameObject == player)
            {
                //Damage to player
                health.takeDamage(100);
            }
        }
    }
    ~RedboxCollision()
    {
        electricSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
