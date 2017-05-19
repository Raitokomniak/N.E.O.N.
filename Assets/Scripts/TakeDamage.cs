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
            float multiplier = (targetIntensity == 30) ? 1 : 0.5f;
            destructLight.intensity = Mathf.Lerp(destructLight.intensity, targetIntensity / 2, multiplier* Time.unscaledDeltaTime);
            destructLight.range = destructLight.intensity*2;
            sr.enabled = false;
            foreach (SpriteRenderer srend in this.transform.parent.gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                srend.color = Vector4.Lerp(srend.color, new Vector4(1, 1, 1, 0),  Time.unscaledDeltaTime);
            }
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
        if (col.gameObject == player&&player.GetComponent<PlayerMovement>().gizmo())
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
        Invoke("setTakeDownOff", 1);
        StartCoroutine(kill());
    }

    IEnumerator kill()
    {
        died = true;
        sr.enabled = false;
        destructLight.enabled = true;
        yield return new WaitForSecondsRealtime(targetIntensity == 30 ? 3 : 5);
        setTakeDownOff();
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
                player.GetComponent<PlayerMovement>().playAnimation("Takedown");
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
                player.GetComponent<PlayerMovement>().playAnimation("Takedown");
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
            setTakeDownOff();
            
        }

    }

    void setTakeDownOff()
    {
        player.GetComponent<PlayerMovement>().setPerformAction(false);
    }

}
