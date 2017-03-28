﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrollingMovement : MonoBehaviour {

   // [FMODUnity.EventRef]
   // public string inputSound = "event:/Input_1";
    public Transform[] waypoints;
    Transform waypoint;
    public float patrollingSpeed = 4;
    public float cautionSpeed = 6;
    public float alertSpeed = 9;
    float speed = 8;
    float maxSpeed = 4;
    public int currentWayPoint;
    bool patrol = true;
    public Vector2 target;
    public Vector2 moveDirection;
    public Vector2 velocity;
    Rigidbody2D enemyRig;
    SpriteRenderer spriteRend;
	public SpriteRenderer headSpriteRend;
    EnemyAISensing sensing;
    bool grounded;
    bool ledgeSpotted;
    bool obstacleSpotted;
    float timer;
    public float cautionTimer = 5f;
    GameControllerScript gScript;
    public Transform gunBarrell;
    public GameObject bullet;
    public GameObject player;
    AudioSource gunAudio;
    public float bulletVelocity = 20f;
    public float timeBetweenBullets = 0.5f;
    float bulletTimer;
    float timeToShoot;
    PlayerInsideAlertZone AlertZone;
    float startingSpeed;
    float searchTimer;
    int facing;
    bool personalAlert;
    float oldpoint;
    Vector3 lastDetectedPosition;
    bool playerHeard;
    enum states
    {
        normal,
        caution,
        alert
    };
    states state;
    void Awake()
    {
        enemyRig = GetComponent<Rigidbody2D>();
        spriteRend = GetComponent<SpriteRenderer>();
        sensing = GetComponent<EnemyAISensing>();
        timer = 0f;
        gunAudio = GetComponent<AudioSource>();
        AlertZone = GetComponentInChildren<PlayerInsideAlertZone>();
        player = GameObject.FindGameObjectWithTag("Player");
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        facing = 1;
        waypoint = (facing == 1) ? waypoints[0] : waypoints[1];
        searchTimer = 0;
        grounded = false;
        startingSpeed = speed;
        oldpoint = 0;
    }

    void Update()
    {
        behaviorHandler();
        flipHandler();
    }

    int enemyDirection(int dir)
    {
        //float velocity = (this.transform.position.x - oldpoint) / Time.deltaTime;
       // oldpoint = this.transform.position.x;
        if (enemyRig.velocity.x > 0)
        {
            dir = 1;
        }
        else if (enemyRig.velocity.x < 0)
        {
            dir = -1;
        }
        return dir;
    }

    void behaviorHandler()
    {
        
        ObstacleCheck();
        if (!obstacleSpotted && !ledgeSpotted)
        {
            if (sensing.playerInSight())
            {
                Alert();
                maxSpeed = alertSpeed;
                state = states.alert;
                personalAlert = true;
            }
            else if (personalAlert)
            {
                checkLastPosition();
                maxSpeed = patrollingSpeed;
            }
            else if (gScript.allGuardsAlerted() && !sensing.playerInSight()&&!personalAlert)
            {
                Caution();
                maxSpeed = cautionSpeed;
                state = states.caution;
            }
            else
            {
                WaypointPatrol();
                maxSpeed = patrollingSpeed;
                state = states.normal;
            }
        }
        else
        {
            if (sensing.playerInSight())
            {
                stop();
            }
            else
            {
                if (state == states.caution)
                {
                    turnAround();
                }
                else if (personalAlert)
                {
                    StartCoroutine(checkPos());
                    //stop();
                    //personalAlert = false;
                }
                else
                {
                    reachedWaypoint();
                }
            }
        }
    }


    void moveToDirection(Vector3 point)
    {
        if (grounded)
        {
            targetOnRightOrLeft(point);
            Vector3 direction = (point - transform.position).normalized;
            //  enemyRig.MovePosition(transform.position + direction * speed * Time.deltaTime);
            enemyRig.AddForce(direction * speed);
            if (Mathf.Abs(enemyRig.velocity.x) > maxSpeed)
            {
                enemyRig.velocity = new Vector2(maxSpeed*facing, enemyRig.velocity.y);
            }
        }
    }

    void WaypointPatrol()
    {
        timer = 0;
        moveToDirection(waypoint.position);
        if (Vector2.Distance (this.transform.position, waypoint.position) < 1)
        {
            reachedWaypoint();
        }
    }

    void reachedWaypoint()
    {
        stop();
        if (waypoint.Equals(waypoints[0]))
        {
            waypoint = waypoints[1];
        }
        else
        {
            waypoint = waypoints[0];
        }
    }

    void flipHandler()
    {
        if (facing == 1)
        {
            spriteRend.flipX = false;
			headSpriteRend.flipX = false;
        }
        else
        {
            spriteRend.flipX = true;
			headSpriteRend.flipX = true;
        }
    }

    public bool facingRight()
    {
        return (facing == 1) ? true : false;
    }
    void Caution()
    {
        timer = timer + Time.deltaTime;
        moveToDirection(transform.position + this.transform.right * facing);
    }

    void turnAround()
    {
        stop();
        facing *= -1;
    }

    void stop()
    {
        enemyRig.velocity = new Vector2(0, enemyRig.velocity.y);
    }

    IEnumerator checkPos()
    {
        stop();
        yield return new WaitForSeconds(1f);
        personalAlert = false;
        state = states.caution;
    }

    void checkLastPosition()
    {
        if (lastDetectedPosition != null)
        {
            moveToDirection(lastDetectedPosition);
            if (Vector2.Distance(this.transform.position, lastDetectedPosition) <= 2)
            {
                StartCoroutine(checkPos());
            }
        }
    }
    void Alert()
    {
        Vector3 playerIsAt = sensing.playerLastSeenPosition();
        Vector3 direction = new Vector3(playerIsAt.x, this.transform.position.y);
        lastDetectedPosition = direction;
        bool playerInShootingRange = false;
        if (sensing.playerInSight())
        {
            searchTimer = 0;
            if (Vector2.Distance(this.transform.position, playerIsAt)<= 5)
            {
                playerInShootingRange = true;
            }
 
        }
       // facing = enemyDirection(facing);
        if (!playerInShootingRange)
        {
            moveToDirection(direction);
        }
        else
        {
            stop();
        }
        Shoot();
    }

    void targetOnRightOrLeft(Vector2 target)
    {
        if (target.x < this.transform.position.x)
        {
            facing = -1;
        }
        else if (target.x > this.transform.position.x)
        {
            facing = 1;
        }
    }

    void Shoot()
    {
        if (sensing.playerInSight())
        {
            bulletTimer += Time.deltaTime;
            if (bulletTimer >= timeBetweenBullets)
            {
                GameObject projectile = (GameObject)Instantiate(bullet, gunBarrell.position, gunBarrell.rotation);
                Rigidbody2D rigidbody = projectile.GetComponent<Rigidbody2D>();
                 gunAudio.Play();
               // FMODUnity.RuntimeManager.PlayOneShot(inputSound);
                rigidbody.velocity = projectile.transform.right * bulletVelocity;
                bulletTimer = 0;
            }  
        }
        else
        {
            bulletTimer = 0;
        }
        gunAudio.pitch = Time.timeScale;
    }
    void ObstacleCheck()
    {
        RaycastHit2D ledgeSpotter;
        RaycastHit2D obstacleSpotter;
        int dir = (facing == 1) ? 1 : -1;
        BoxCollider2D box = this.GetComponent<BoxCollider2D>();
        Vector2 ledgeStartPoint = box.transform.position + (box.transform.right * dir);
        ledgeSpotter = Physics2D.Raycast(new Vector2(ledgeStartPoint.x + (box.size.x * dir), this.transform.position.y), -Vector2.up);
        obstacleSpotter = Physics2D.BoxCast(this.transform.position, box.size, Vector2.Angle(this.transform.position, this.transform.right * dir), this.transform.right * dir);
        if (obstacleSpotter)
        {
            if (obstacleSpotter.collider.gameObject.CompareTag("Player"))
            {
                obstacleSpotted = false;
            }
            else
            {
                obstacleSpotted = (obstacleSpotter.distance < box.bounds.size.x) ? true : false;
            }
        }
        else
        {
            obstacleSpotted = false;
        }
        if (ledgeSpotter)
        {
            RaycastHit2D ground = Physics2D.Raycast(new Vector2(box.transform.position.x, this.transform.position.y), -Vector2.up);
            ledgeSpotted = (ledgeSpotter.point.y < ground.point.y) ? true : false;
        }
        else
        {
            ledgeSpotted = true;
        }
    
    }
    
    void OnCollisionStay2D(Collision2D col)
    {
        RaycastHit2D ground = Physics2D.Raycast(this.transform.position, -this.transform.up);
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

    public bool checks()
    {
        return personalAlert;
    }

    public void playerIsHeard(Vector2 pPos)
    {
        lastDetectedPosition = new Vector2(pPos.x, this.transform.position.y);
        personalAlert = true;
    }

}
