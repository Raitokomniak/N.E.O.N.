using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerScript : MonoBehaviour {

    // Use this for initialization
    Rigidbody2D daggerRig;
    Light lite;
    int amountOfHits;
    int maxHit;
	void Awake ()
    {
        daggerRig = GetComponent<Rigidbody2D>();
        lite = GetComponentInChildren<Light>();
        maxHit = 6;
	}

    private void FixedUpdate()
    {
        lite.intensity = Random.Range(1.8f, 2.2f);
        lite.range = Random.Range(0.8f, 1.268372f);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag != "Player"&&!col.isTrigger)
        {
            if (col.gameObject.tag == "Enemy" && daggerRig.velocity.sqrMagnitude > 1)
            {
                col.gameObject.GetComponent<EnemyPatrollingMovement>().getDaggerHit();
                daggerRig.gameObject.SetActive(false);
            }
           // daggerRig.velocity = Vector2.Reflect(daggerRig.velocity, transform.right);
           
            daggerRig.gameObject.SetActive(false);
        }
       
    }

}
