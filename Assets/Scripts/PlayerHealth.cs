using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    // Use this for initialization
    int health;
    GameControllerScript gScript;
    void Awake()
    {
       // gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }
    void Start () {
        health = 100;
        
	}
	
	// Update is called once per frame

    public void takeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            die();
        }
    }

    public void getHealth(int amount)
    {
        health += amount;
        if (health > 100)
        {
            health = 100;
        }
    }

    void die()
    {
        gScript.setPlayerDead();
    }

}
