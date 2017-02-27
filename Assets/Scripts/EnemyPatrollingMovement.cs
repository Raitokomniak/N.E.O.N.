﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrollingMovement : MonoBehaviour {

    public Transform[] waypoints;
    public float speed = 5;
    public int currentWayPoint;
    bool patrol = true;
    /*
    bool alert = false;
    bool caution = false;
    */
    public Vector2 target;
    public Vector2 moveDirection;
    public Vector2 velocity;
    Rigidbody2D enemyRig;
    SpriteRenderer spriteRend;
    EnemyAISensing sensing;
    bool grounded;
    bool ledgeSpotted;
    bool obstacleSpotted;
    float timer;
    float timerLimit = 5f;


    enum facingDir
    {
        right,
        left
    };
    facingDir facing;

    void Awake()
    {
        enemyRig = GetComponent<Rigidbody2D>();
        spriteRend = GetComponent<SpriteRenderer>();
        sensing = GetComponent<EnemyAISensing>();
        timer = 0f;
    }

    void Start()
    {
        grounded = false;
    }

    void Update ()
    {
        //Debug.Log(timer);
        if (!sensing.playerInSight() && patrol)
        {
            timer = 0f;
            WaypointPatrol();
        }
        else if (sensing.playerInSight())
        {
            patrol = false;
            Alert();
        }
        else if (!sensing.playerInSight() && !patrol)
        {
            timer = timer + Time.deltaTime;
            if(timer >= timerLimit)
            {
                patrol = true;
            }
            else
            {
                Alert();
            }
        }
        flipHandler();		
	}

    void WaypointPatrol()
    {
        Debug.Log("Patrolling");
        if (currentWayPoint < waypoints.Length)
        {
            target = waypoints[currentWayPoint].position;
            moveDirection = new Vector2(target.x - enemyRig.transform.position.x, 0f);
            velocity = enemyRig.velocity;

            if(moveDirection.magnitude < 1)
            {
                currentWayPoint++;
            }
            else
            {
                velocity = moveDirection.normalized * speed;
            }
        }

        else
        {
            if (patrol)
            {
                currentWayPoint = 0;
            }
            else
            {
                velocity = Vector2.zero;
            }
        }

        enemyRig.velocity= velocity;
        if(enemyRig.velocity.x > 0)
        {
            facing = facingDir.right;
        }
        else if(enemyRig.velocity.x < 0)
        {
            facing = facingDir.left;
        }
        //transform.LookAt(target);
    }

    void flipHandler()
    {
        if (facing == facingDir.right)
        {
            spriteRend.flipX = false;
        }
        else if (facing == facingDir.left)
        {
            spriteRend.flipX = true;
        }
    }
    public bool facingRight()
    {
        if (facing == facingDir.right)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void Caution()
    {
        //moves outside of patrol routes
        //return to patrol in x seconds
        /*
                ObstacleCheck();
        if (!sensing.playerInSight())
        {
            //if (wayPoints == null)
            //   {
            int dir = 1;
            if (facing == facingDir.left)
            {
                dir *= -1;
            }

            if (ledgeSpotted || obstacleSpotted)
            {
                enemyRig.velocity = new Vector2(0, enemyRig.velocity.y);
                if (facing == facingDir.right)
                {
                    facing = facingDir.left;
                }
                else
                {
                    facing = facingDir.right;
                }
            }
            else
            {
                if (grounded)
                {
                    if (facing == facingDir.right)
                    {
                        enemyRig.velocity = new Vector2(speed, enemyRig.velocity.y);
                    }
                    else
                    {
                        enemyRig.velocity = new Vector2(speed * -1, enemyRig.velocity.y);
                    }
                }
            }
            // }
        }
        */

    }
    void Alert()
    {
        //if player in sight break out from the patrol movement
        //return to caution in X seconds
        
        /*NEED TO CHANGE: this funcktion works at the moment like Caution()
        need change it to follow player more closely*/
        Debug.Log("Alerted");
        ObstacleCheck();
        if (!sensing.playerInSight())
        {
            //if (wayPoints == null)
            //   {
            int dir = 1;
            if (facing == facingDir.left)
            {
                dir *= -1;
            }

            if (ledgeSpotted || obstacleSpotted)
            {
                enemyRig.velocity = new Vector2(0, enemyRig.velocity.y);
                if (facing == facingDir.right)
                {
                    facing = facingDir.left;
                }
                else
                {
                    facing = facingDir.right;
                }
            }
            else
            {
                if (grounded)
                {
                    if (facing == facingDir.right)
                    {
                        enemyRig.velocity = new Vector2(speed, enemyRig.velocity.y);
                    }
                    else
                    {
                        enemyRig.velocity = new Vector2(speed * -1, enemyRig.velocity.y);
                    }
                }
            }
            // }
        }
    }

    void ObstacleCheck()
    {
        RaycastHit2D see;
        RaycastHit2D obstacleSpotter;
        if (facing == facingDir.right)
        {
            see = Physics2D.Raycast(this.transform.position, new Vector2(1, -1));
            obstacleSpotter = Physics2D.Raycast(this.transform.position, this.transform.right);
        }
        else
        {
            see = Physics2D.Raycast(this.transform.position, new Vector2(-1, -1));
            obstacleSpotter = Physics2D.Raycast(this.transform.position, -this.transform.right);

        }
        RaycastHit2D ground = Physics2D.Raycast(this.transform.position, -this.transform.up);
        if (grounded && Vector2.Distance(ground.point, see.point) > ground.distance * 2)
        {
            // Debug.Log("Ledge spotted");
            ledgeSpotted = true;
        }
        else
        {
            ledgeSpotted = false;
        }

        // Debug.Log(obstacleSpotter.distance);
        if (obstacleSpotter)
        {
            if (Vector2.Distance(this.transform.position, obstacleSpotter.point) < 1f)
            {
                obstacleSpotted = true;
            }
            else
            {
                obstacleSpotted = false;
            }
        }
        else
        {
            obstacleSpotted = false;
        }
    }
    void OnCollisionStay2D(Collision2D col)
    {
        RaycastHit2D ground = Physics2D.Raycast(this.transform.position, -this.transform.up);
        //RaycastHit2D left = Physics2D.Raycast(this.transform.position, this.transform.right);
        if (ground)
        {
            if (ground.collider.gameObject == col.gameObject)
            {
                grounded = true;
            }
            else
            {
                grounded = false;
            }
        }
    }
}
