using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAISensing : MonoBehaviour {

    // Use this for initialization
    public float enemyFieldOfView = 110f;
    bool playerSeen;
    EnemyAIMovement moving;
    CircleCollider2D circle;
    Vector2 playerIsAt;
    public Transform eyes;
    GameObject player;
    GameControllerScript gScript;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        moving = GetComponent<EnemyAIMovement>();
        circle = GetComponent<CircleCollider2D>();
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }

    void Start () {
        playerSeen = false;
        
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
                            pInSight = true;
                            Debug.DrawRay(eyes.position, direction, Color.red);
                            playerIsAt = direction;
                        }
                        else
                        {
                            pInSight = false;
                        }
                    }

                }
                playerSeen = pInSight;
            }

        }
        else
        {
            playerSeen = false;
        }
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
