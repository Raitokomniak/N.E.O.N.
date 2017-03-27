using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAISensing : MonoBehaviour {

    // Use this for initialization
    public SpriteRenderer exclamationMarkSprite;
    public float enemyFieldOfView = 110f;
    public float detectionTime = 0.5f;
    bool playerSeen;
    EnemyPatrollingMovement moving;
    CircleCollider2D circle;
    BoxCollider2D box;
    Vector2 playerIsAt;
    public Transform eyes;
    GameObject player;
    GameControllerScript gScript;
    float timer;
    float anotherTimer;
    void Awake()
    {
       
        moving = GetComponent<EnemyPatrollingMovement>();
        circle = GetComponent<CircleCollider2D>();
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        
    }

    void Start () {
        playerSeen = false;
        exclamationMarkSprite.enabled = false;
        timer = 0;
        anotherTimer = 0;
        player = GameObject.FindGameObjectWithTag("Player");
        box = player.GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        if (playerSeen)
        {
            gScript.setAlertState(true);
        }
    }

    bool detectionHandler(bool seen)
    {
        timer += Time.deltaTime;
        if (timer >= detectionTime)
        {
            seen = true;
            StartCoroutine(alert());
        }
        return seen;
    }
	
    void OnTriggerStay2D(Collider2D col)
    {
        if (!gScript.isDead())
        {
            seeing(col);
            if (!playerSeen && col.gameObject == player)
            {
                hearing(col);
            }
        }
        else
        {
            playerSeen = false;
            timer = 0;
            anotherTimer = 0;
        }
    }

    void hearing(Collider2D col)
    {
        if (player.GetComponent<PlayerMovement>().getState()== "run")
        {
            playerIsAt = col.transform.position;
            moving.playerIsHeard(playerIsAt);            
        }
    }

    void seeing(Collider2D col)
    {
        int dir = 1;
        if (!moving.facingRight())
        {
            dir *= -1;
        }

        if (col.gameObject == player)
        {
            Vector2 direction = col.transform.position - eyes.position;
            float angle = Vector2.Angle(direction, eyes.right * dir);
            if (angle < enemyFieldOfView * 0.5f)
            {
                RaycastHit2D see = Physics2D.Raycast(eyes.position, direction);
                if (see)
                {
                    if (see.collider.gameObject == player)
                    {
                        if (!playerSeen)
                        {
                            playerSeen = detectionHandler(playerSeen);
                        }
                        else
                        {
                            playerSeen = true;
                        }
                        playerIsAt = new Vector2(box.transform.position.x, box.transform.position.y) + box.offset;
                    }
                    else
                    {
                        if (playerSeen)
                        {
                            playerSeen = false;
                        }
                        else
                        {
                            playerSeen = false;
                            anotherTimer = 0;
                        }
                        timer = (playerSeen) ? 0 : timer;
                    }
                }
            }
        }
        
    }

    bool checkIfPlayerIsFront()
    {
        anotherTimer += Time.deltaTime;
        int dir = moving.facingRight() ? 1 : -1;
        Vector2 directionToTarget = transform.position - player.transform.position;
        float angle = Vector2.Angle(transform.right * dir, directionToTarget);
        float distance = directionToTarget.magnitude;

        return (angle >= 90) ? true : false;
    }

    IEnumerator detect()
    {
        yield return new WaitForSeconds(0.5f);
        playerSeen = true;
    }

    IEnumerator alert()
    {
        exclamationMarkSprite.enabled = true;
        yield return new WaitForSeconds(2f);
        exclamationMarkSprite.enabled = false;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (!gScript.isDead())
        {
            if (col.gameObject == player)
            {
                playerSeen = false;
            }
        }
    }
    public bool playerInSight()
    {
        return playerSeen;
    }

    public Vector2 playerLastSeenPosition()
    {
        return playerIsAt;
    }

}
