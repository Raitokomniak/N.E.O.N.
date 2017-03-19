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
    Vector2 playerIsAt;
    public Transform eyes;
    GameObject player;
    GameControllerScript gScript;
    float timer;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        moving = GetComponent<EnemyPatrollingMovement>();
        circle = GetComponent<CircleCollider2D>();
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }

    void Start () {
        playerSeen = false;
        exclamationMarkSprite.enabled = false;
        timer = 0;
    }

    void Update()
    {
        
        Debug.Log(timer);
    }

    bool detectionHandler(bool seen)
    {
        timer += Time.deltaTime;
        if (timer >= detectionTime)
        {
            seen = true;
        }
        return seen;
    }
	
    void OnTriggerStay2D(Collider2D col)
    {
        if (!gScript.isDead())
        {
            int dir = 1;
            if (!moving.facingRight())
            {
                dir *= -1;
            }

            if (col.gameObject == player)
            {
                bool pInSight = false;
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
                                pInSight = detectionHandler(pInSight);
                            }
                            else
                            {
                                pInSight = true;
                            }
                            Debug.DrawRay(eyes.position, direction, Color.red);
                            playerIsAt = direction;
                        }
                        else
                        {
                            pInSight = false;
                            timer = 0;
                        }
                    }

                }
                if (pInSight)
                {
                    gScript.setAlertState(true);
                    if (!playerSeen)
                    {
                        StartCoroutine(alert());
                    }
                }
                if (playerSeen)
                {
                    pInSight = checkIfPlayerIsFront();
                }
                playerSeen = pInSight;
            }

        }
        else
        {
            playerSeen = false;
        }
    }

    bool checkIfPlayerIsFront()
    {
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

    public Vector2 playerDirection()
    {
        return playerIsAt;
    }

}
