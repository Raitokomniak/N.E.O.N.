using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour {

    // Use this for initialization
    GameControllerScript gScript;
    GameObject player;
    void Awake () {
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        player = GameObject.FindGameObjectWithTag("Player");
	}

    // Update is called once per frame
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject == player)
        {
            //UI tip for the player that silent takedown is enabled
            int dir = GetComponentInParent<EnemyPatrollingMovement>().facingRight() ? 1 : -1;
            Vector2 directionToTarget = transform.position - player.transform.position;
            float angle = Vector2.Angle(transform.right * dir, directionToTarget);
            if (angle < 90)
            {
                performSilentTakedown();
            }

        }
    }

    void performSilentTakedown()
    {
        if (Input.GetButtonDown("melee"))
        {
            //something fancy here
            player.GetComponent<PlayerMovement>().setPerformAction(true);
            GetComponentInParent<EnemyPatrollingMovement>().silentKill(true);
            die();
            player.GetComponent<PlayerMovement>().setPerformAction(false);
        }
    }

    void die()
    {
        gScript.killGuard(this.transform.parent.gameObject);
    }
}
