using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    // Use this for initialization
    public float recoveryTime = 10;
    public int maxHealth = 100;
    int health;
    GameControllerScript gScript;
    bool adding;
    bool takingDamage;
    float damageTimer;
    void Awake()
    {
       gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }
    void Start () {
        health = 100;
        Mathf.Clamp(health, 0, maxHealth);
        damageTimer = 0;
        takingDamage = false;
        adding = false;
	}
	
	// Update is called once per frame

    void Update()
    {
        Debug.Log(health);
        damageTimer += Time.deltaTime;
        if (damageTimer >= recoveryTime)
        {
            if (health < maxHealth&&!adding)
            {
                adding = true;
                StartCoroutine(recover());
            }
        }
    }
    public void takeDamage(int amount)
    {
        health -= amount;
        takingDamage = true;
        damageTimer = 0;
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

    IEnumerator recover()
    {
        yield return new WaitForSeconds(0.2f);
        adding = false;
        health++;
    }

    void die()
    {
        gScript.setPlayerDead();
    }

}
