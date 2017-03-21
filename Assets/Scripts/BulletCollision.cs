using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour {

    // Use this for initialization
    GameObject player;
    bool touched;
    bool playerTouch;
    Vector2 startPlace;
    PlayerHealth health;
    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        health = player.GetComponent<PlayerHealth>();
        startPlace = this.transform.position;
    }
	void Start()
    {
        
    }
	// Update is called once per frame
	void Update () {
		if (Vector2.Distance(startPlace, this.transform.position) > 100f)
        {
            Destroy(gameObject);
        }
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.isTrigger)
        {
            if (col.gameObject == player)
            {
                //Damage to player
                health.takeDamage(50);
            }
            Destroy(gameObject);
        }
    }

}
