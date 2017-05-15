using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour {

    // Use this for initialization
    
    GameControllerScript gScript;
    GameObject player;
    EnemyPatrollingMovement enemyMov;
    public Sprite leftStick;
    public Sprite rightStick;
    public Sprite xButton;
    public SpriteRenderer sr;
    public float takeDownTime = 2f;
    float timer;
    bool takedownStarted;
    int direction;
    bool playerInTheArea = false;
    
    void Awake () {
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemyMov = GetComponentInParent<EnemyPatrollingMovement>();
        timer = 0;
        direction = 0;
        takedownStarted = false;
        sr.enabled = false;
    }
   
    // Update is called once per frame
    private void Update()
    {
        if (!playerInTheArea && sr.enabled)
        {
            sr.enabled = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == player)
        {
            playerInTheArea = true;
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject == player)
        {
            int dir = enemyMov.facingRight() ? 1 : -1;
            Vector2 directionToTarget = transform.position - player.transform.position;
            float angle = Vector2.Angle(transform.right * dir, directionToTarget);
            if (angle < 90 && enemyMov.facingRight() == player.GetComponent<PlayerMovement>().isFacingRight())
            {
                sr.sprite = xButton;
                sr.enabled = true;
                performSilentTakedown();
            }
            else
            {
                sr.enabled = false;
            }
            if (takedownStarted)
            {
                timer += Time.unscaledDeltaTime;
                switchOffSystem();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == player)
        {
            playerInTheArea = false;
            sr.enabled = false;
        }
    }
    void performSilentTakedown()
    {
        if (Input.GetButton("melee")&&!takedownStarted)
        {
            timer = 0;
            player.GetComponent<PlayerMovement>().setPerformAction(true);
            int facing = enemyMov.facingRight() ? 1 : -1;
            player.GetComponent<PlayerMovement>().setFacing(facing);
            enemyMov.silentKill(true);
            float mark = Random.Range(-2, 1);
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
        
        sr.sprite = (direction == 1) ? rightStick : leftStick;
        sr.enabled = true;
        if (timer < takeDownTime)
        {
            if (stickDir == direction)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Character sounds/GIZMO/Takedown (silent)", transform.position);
                die();
            }
            else if (stickDir == direction * -1)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Character sounds/GIZMO/Takedown (loud)", transform.position);
                gScript.setAlertState(true);
                die();
            }           
        }
        else
        {
            enemyMov.silentKill(false);
            //enemyMov.playerIsHeard(player.transform.position);
            GetComponentInParent<EnemyAISensing>().setPlayerInSight(true);
            takedownStarted = false;
            sr.enabled = false;
            player.GetComponent<PlayerMovement>().setPerformAction(false);
            
        }

    }


}
