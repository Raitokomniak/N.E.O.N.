using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrollingMovement : MonoBehaviour {

    
    public Transform[] waypoints;
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
        gunAudio = GetComponent<AudioSource>();
        AlertZone = GetComponentInChildren<PlayerInsideAlertZone>();
        player = GameObject.FindGameObjectWithTag("Player");
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }

    void Start()
    {
        grounded = false;
        startingSpeed = speed;
    }

    void Update()
    {
        //Debug.Log(timer);
        if (!patrol)
        {
            speed = 5f;
        }
        else
        {
            speed = startingSpeed;
        }
        if (gScript.allGuardsAlerted() && !sensing.playerInSight() && !AlertZone.getPlayerInAlerZone() && timer < cautionTimer)
        {
            Caution();
            patrol = false;
        }
        else
        {
            if (!sensing.playerInSight() && patrol)
            {
                timer = 0f;
                WaypointPatrol();
            }
            else if (sensing.playerInSight())
            {
                Shoot();
                patrol = false;
                Alert();
            }
            else if (!sensing.playerInSight() && !patrol)
            {
                timer = timer + Time.deltaTime;
                if (timer >= cautionTimer)
                {
                    patrol = true;
                }
                else
                {
                    Alert();
                }
            }
        }
        flipHandler();
    }

    void WaypointPatrol()
    {
        //Debug.Log("Patrolling");
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
			headSpriteRend.flipX = false;
        }
        else if (facing == facingDir.left)
        {
            spriteRend.flipX = true;
			headSpriteRend.flipX = true;
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
        //moves outside of patrol routes seeking player
        //return to patrol if does not find player before cautionTimer runs out
        //Debug.Log("Caution");
        ObstacleCheck();
        if (!sensing.playerInSight())
        {
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
        }
        

    }
    void Alert()
    {
        if (AlertZone.getPlayerInAlerZone())
        {

            //target = new Vector2(player.transform.position.x, player.transform.position.y);
            target = sensing.playerLastSeenPosition();
            moveDirection = new Vector2(target.x - enemyRig.transform.position.x, 0f);
            ObstacleCheck();
            RaycastHit2D checker = Physics2D.Raycast(this.transform.position, target);
            if (checker&& checker.collider.gameObject != player)
            {
               Caution();
            }
            //Debug.Log(moveDirection.magnitude);
            if (ledgeSpotted || obstacleSpotted)
            {
                velocity = Vector2.zero;
            }
            
            else if (moveDirection.magnitude > 3)
            {
                velocity = moveDirection.normalized * speed;
            }

            else
            {
                velocity = Vector2.zero;
            }

            enemyRig.velocity = velocity;

            if (enemyRig.velocity.x > 0)
            {
                facing = facingDir.right;
            }
            else if (enemyRig.velocity.x < 0)
            {
                facing = facingDir.left;
            }
        }
        else if (!AlertZone.getPlayerInAlerZone())
        {
            Caution();
        }
    }

    void Shoot()
    {
        //RaycastHit2D shoot = Physics2D.Raycast(gunBarrell.position, player.transform.position);
        if (sensing.playerInSight())
        {
            
            bulletTimer += Time.deltaTime;
            
                if (bulletTimer >= timeBetweenBullets)
                {

                    GameObject projectile = (GameObject)Instantiate(bullet, gunBarrell.position, gunBarrell.rotation);
                    Rigidbody2D rigidbody = projectile.GetComponent<Rigidbody2D>();
                    gunAudio.Play();

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
