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
	}
	void Start()
    {
        startPlace = this.transform.position;
    }
	// Update is called once per frame
	void Update () {
		if (Vector2.Distance(startPlace, this.transform.position) > 100f)
        {
            Destroy(gameObject);
        }
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject == player)
        {
            //Damage to player
            health.takeDamage(50);
        }
        Destroy(gameObject);
    }

}
