﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedboxCollision : MonoBehaviour {
    GameObject player;
    PlayerHealth health;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        health = player.GetComponent<PlayerHealth>();
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
}