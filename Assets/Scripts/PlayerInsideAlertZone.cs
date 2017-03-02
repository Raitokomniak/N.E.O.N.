using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInsideAlertZone : MonoBehaviour {

    public bool playerInAlertZone = false;

    // Update is called once per frame

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            playerInAlertZone = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInAlertZone = false;
        }
    }

    public bool getPlayerInAlerZone()
    {
        return playerInAlertZone;
    }
}