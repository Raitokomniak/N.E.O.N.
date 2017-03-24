using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrollingMovement : MonoBehaviour {

   // [FMODUnity.EventRef]
   // public string inputSound = "event:/Input_1";
    public Transform[] waypoints;
    Transform waypoint;
    public float speed = 5;
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
        if (sensing.playerInSight()&&!personalAlert)
        {
            personalAlert = sensing.playerInSight();
        }
        Debug.Log(state);
    }

    int enemyDirection(int dir)
    {
        float velocity = (this.transform.position.x - oldpoint) / Time.deltaTime;
        oldpoint = this.transform.position.x;
        if (velocity > 0)
        {
            dir = 1;
        }
        else if (velocity < 0)
        {
            dir = -1;
        }
        return dir;
    }

    void stateHandler()
    {
        if (sensing.playerInSight())
        {
            state = states.alert;
        }
        if (gScript.allGuardsAlerted()&&!sensing.playerInSight() && timer < cautionTimer)
        {
            state = states.caution;
        }
        else if (!sensing.playerInSight() && !gScript.allGuardsAlerted())
        {
            state = states.normal;
        }
    }
    void behaviorHandler()
    {
        stateHandler();
        ObstacleCheck();
        if (!obstacleSpotted && !ledgeSpotted)
        {
            switch (state)
            {
                case states.normal:
                    WaypointPatrol();
                    break;
                case states.caution:
                    Caution();
                    break;
                case states.alert:
                    Alert();
                    break;
                default:
                    WaypointPatrol();
                    break;
            }
        }
        else
        {
            if (state == states.caution || state == states.alert)
            {
                turnAround();
            }
            else
            {
                reachedWaypoint();
            }
        }
    }


    void moveToDirection(Vector3 point)
    {
        if (grounded)
        {
            Vector3 direction = (point - transform.position).normalized;
            enemyRig.MovePosition(transform.position + direction * speed * Time.deltaTime);
        }
    }

    void WaypointPatrol()
    {
        timer = 0;
        moveToDirection(waypoint.position);
        if (waypoint.position.x < this.transform.position.x)
        {
            facing = -1;
        }
        else if (waypoint.position.x > this.transform.position.x)
        {
            facing = 1;
        }
        Debug.Log(waypoint.position.x + " " + this.transform.position.x);
        //enemyRig.velocity = direction * speed*facing;
        
        if (Vector2.Distance (this.transform.position, waypoint.position) < 1)
        {
            reachedWaypoint();
        }
    }

    void reachedWaypoint()
    {
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


    void Alert()
    {
        Vector3 playerIsAt = sensing.playerLastSeenPosition();
        Vector3 direction = playerIsAt;
        bool playerInShootingRange = false;
        if (sensing.playerInSight())
        {
            searchTimer = 0;
            if (Vector2.Distance(this.transform.position, playerIsAt)<= 5)
            {
                playerInShootingRange = true;
            }
 
        }
        facing = enemyDirection(facing);
        if (!playerInShootingRange)
        {
            moveToDirection(direction);
        }
         Shoot();
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
        Debug.DrawRay(new Vector2(ledgeStartPoint.x + (box.size.x * dir), this.transform.position.y), -Vector2.up, Color.red);
        obstacleSpotter = Physics2D.BoxCast(this.transform.position, box.size, Vector2.Angle(this.transform.position, this.transform.right * dir), this.transform.right * dir);
        if (obstacleSpotter)
        {
            obstacleSpotted = (obstacleSpotter.distance < box.bounds.size.x) ? true : false;
           
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

}
