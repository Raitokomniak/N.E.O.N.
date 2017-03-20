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
        player = GameObject.FindGameObjectWithTag("Player");
        moving = GetComponent<EnemyPatrollingMovement>();
        circle = GetComponent<CircleCollider2D>();
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        box = player.GetComponent<BoxCollider2D>();
    }

    void Start () {
        playerSeen = false;
        exclamationMarkSprite.enabled = false;
        timer = 0;
        anotherTimer = 0;
    }
    void Update()
    {
        Debug.Log(timer);
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
                            Debug.DrawRay(eyes.position, direction, Color.red);
                            playerIsAt = new Vector2(box.transform.position.x, box.transform.position.y) + box.offset;
                        }
                        else
                        {
                            if (playerSeen)
                            {
                                playerSeen = checkIfPlayerIsFront();
                            }
                            // timer = 0;
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
        else
        {
            playerSeen = false;
            timer = 0;
            anotherTimer = 0;
        }
    }

    bool checkIfPlayerIsFront()
    {
        anotherTimer += Time.deltaTime;
        int dir = moving.facingRight() ? 1 : -1;
        Vector2 directionToTarget = transform.position - player.transform.position;
        float angle = Vector2.Angle(transform.right * dir, directionToTarget);
        float distance = directionToTarget.magnitude;

        return (angle >= 90&& anotherTimer <= 5) ? true : false;
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
