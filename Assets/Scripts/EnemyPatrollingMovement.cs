using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrollingMovement : MonoBehaviour {

    [FMODUnity.EventRef]
    public string inputSound = "event:/Input_1";
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
    public int startFacingDirection = -1;
    float searchTimer;
    int facing;
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
        waypoint = (facing == 1) ? waypoints[1] : waypoints[0];
        facing = startFacingDirection;
        searchTimer = 0;
    }

    void Start()
    {
        grounded = false;
        startingSpeed = speed;
    }

    void Update()
    {
        behaviorHandler();
        flipHandler();
    }
    void stateHandler()
    {
        if (sensing.playerInSight())
        {
            state = states.alert;
        }
        else if (state == states.alert&& searchTimer < 2)
        {
            state = states.alert;
        }
        else if (gScript.allGuardsAlerted()&&!sensing.playerInSight() && timer < cautionTimer)
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
            if (!sensing.playerInSight())
            {
                turnAround();
            }
            if (state == states.normal)
            {
                reachedWaypoint();
            }
        }
    }


    void WaypointPatrol()
    {
        timer = 0;
        facing = (waypoint.position.x > this.transform.position.x) ? 1 : -1;
        Vector3 direction = (waypoint.position - transform.position).normalized;
        if (grounded)
        {
            enemyRig.MovePosition(transform.position + direction * speed * Time.deltaTime);
        }
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
        if (grounded)
        {
            enemyRig.MovePosition(transform.position + this.transform.right*facing * speed * Time.deltaTime);
        }
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
        Vector3 direction = (new Vector3(playerIsAt.x, this.transform.position.y) - transform.position).normalized;
        
        if (!sensing.playerInSight())
        {
            searchTimer += Time.deltaTime;

            enemyRig.MovePosition(transform.position + this.transform.right * facing * speed * Time.deltaTime);
        }
        else if (sensing.playerInSight())
        {
            Debug.Log(Vector2.Distance(this.transform.position, player.transform.position));
            searchTimer = 0;
            if (Vector2.Distance(this.transform.position, player.transform.position)<= 5)
            {
                direction = Vector2.zero;
            }
        }
        if (grounded)
        {
            enemyRig.MovePosition(transform.position + direction * speed * Time.deltaTime);
        }
       // Shoot();
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
            // gunAudio.Play();
                FMODUnity.RuntimeManager.PlayOneShot(inputSound);
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
}
