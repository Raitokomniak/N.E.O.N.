using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gizmo : MonoBehaviour {
    public GameObject gizmo;
    // Use this for initialization

    private void Awake()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>().setGizmo(true);
           GetComponent<Gizmo>().enabled = false;
           gizmo.SetActive(false);
        }
    }
}
