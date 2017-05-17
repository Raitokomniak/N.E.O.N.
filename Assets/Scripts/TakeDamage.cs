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
    public Light destructLight;
    public float takeDownTime = 2f;
    float timer;
    bool takedownStarted;
    int direction;
    bool died;
    bool playerInTheArea = false;
    float targetIntensity = 100;


    void Awake () {
        destructLight.enabled = false;
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemyMov = GetComponentInParent<EnemyPatrollingMovement>();
        timer = 0;
        direction = 0;
        takedownStarted = false;
        sr.enabled = false;
        died = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!playerInTheArea && sr.enabled)
        {
            sr.enabled = false;
        }
        if (died)
        {
            destructLight.intensity = Mathf.Lerp(destructLight.intensity, targetIntensity, 2* Time.unscaledDeltaTime);
            destructLight.range = destructLight.intensity;
        }
        else
        {
            destructLight.enabled = false;
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
        StartCoroutine(kill());
        player.GetComponent<PlayerMovement>().setPerformAction(false);
    }

    IEnumerator kill()
    {
        died = true;
        sr.enabled = false;
        destructLight.enabled = true;
        yield return new WaitForSeconds(1f);
        gScript.killGuard(this.transform.parent.gameObject);
    }

    void switchOffSystem()
    {
        int stickDir = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        sr.sprite = (direction == 1) ? rightStick : leftStick;
        sr.enabled = true;
        if (timer < takeDownTime)
        {
            if (stickDir == direction)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Character sounds/GIZMO/Takedown (silent)", transform.position);
                player.GetComponent<PlayerMovement>().takeDown();
                targetIntensity = 30;
               // destructLight.color = Color.green;
                die();              
            }
            else if (stickDir == direction * -1)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Character sounds/GIZMO/Takedown (loud)", transform.position);
                gScript.setAlertState(true);
            //    destructLight.color = Color.red;
                targetIntensity = 50;
                player.GetComponent<PlayerMovement>().takeDown();
                die();
            }           
        }
        else if (timer > takeDownTime&&!died)
        {
            enemyMov.silentKill(false);
            enemyMov.playerIsHeard(player.transform.position);
            enemyMov.moveToDirection(new Vector3(player.transform.position.x, this.transform.parent.position.y, 0));
            GetComponentInParent<EnemyAISensing>().setPlayerInSight(true);
            GetComponentInParent<EnemyAISensing>().setPlayerIsAt(player.transform.position);
            takedownStarted = false;
            sr.enabled = false;
            player.GetComponent<PlayerMovement>().setPerformAction(false);
            
        }

    }


}
