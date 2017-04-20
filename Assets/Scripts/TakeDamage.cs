using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour {

    // Use this for initialization
    
    GameControllerScript gScript;
    GameObject player;
    EnemyPatrollingMovement enemyMov;
    public float takeDownTime = 2f;
    float timer;
    bool takedownStarted;
    int direction;
    
    void Awake () {
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemyMov = GetComponentInParent<EnemyPatrollingMovement>();
        timer = 0;
        direction = 0;
        takedownStarted = false;
	}
   
    // Update is called once per frame
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject == player)
        {
            //UI tip for the player that silent takedown is enabled
            int dir = enemyMov.facingRight() ? 1 : -1;
            Vector2 directionToTarget = transform.position - player.transform.position;
            float angle = Vector2.Angle(transform.right * dir, directionToTarget);
            if (angle < 90)
            {

                performSilentTakedown();
            }
            if (takedownStarted)
            {
                
                switchOffSystem();
            }
        }
    }

    void performSilentTakedown()
    {
        if (Input.GetButtonDown("melee")&&!takedownStarted)
        {
            timer = 0;
            player.GetComponent<PlayerMovement>().setPerformAction(true);
            enemyMov.silentKill(true);
            float mark = Random.Range(-2, 1);
            Debug.Log("MARK " + mark);
            direction = (mark < 0) ? -1 : 1;
            takedownStarted = true;
        }
    }

    void die()
    {
        gScript.killGuard(this.transform.parent.gameObject);
        player.GetComponent<PlayerMovement>().setPerformAction(false);
    }

    void switchOffSystem()
    {
        Debug.Log(direction);
        int stickDir = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        timer += Time.deltaTime;
        if (timer < takeDownTime)
        {
            if (stickDir == direction)
            {
                die();
                Debug.Log(direction + " " + stickDir);
            }
            else if (stickDir == direction * -1)
            {
                gScript.setAlertState(true);
                die();
                Debug.Log(direction + " " + stickDir);
            }
            
        }
        else
        {
            enemyMov.silentKill(false);
            enemyMov.playerIsHeard(player.transform.position);
            takedownStarted = false;
            player.GetComponent<PlayerMovement>().setPerformAction(false);
        }

    }


}
