using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerScript : MonoBehaviour {

    // Use this for initialization
    Rigidbody2D daggerRig;
	void Awake ()
    {
        daggerRig = GetComponent<Rigidbody2D>();
	}

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag != "Player"&&!col.isTrigger)
        {
            if (col.gameObject.tag == "Enemy" && daggerRig.velocity.sqrMagnitude > 1)
            {
                col.gameObject.GetComponent<EnemyPatrollingMovement>().getDaggerHit();
            }
            daggerRig.gameObject.SetActive(false);
        }
    }

}
