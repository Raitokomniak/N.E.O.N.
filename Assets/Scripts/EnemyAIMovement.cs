using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIMovement : MonoBehaviour
{

    // Use this for initialization
    // public Transform gunBarrellRight;
    public Transform gunBarrell;
    public GameObject bullet;
    AudioSource gunAudio;
    public float velocity = 20f;
    public float timeBetweenBullets = 0.5f;
    public float speed = 5;
    public Transform[] wayPoints;
    float bulletTimer;
    float timeToShoot;
    bool grounded;
    bool ledgeSpotted;
    bool obstacleSpotted;
    EnemyAISensing sensing;
    Rigidbody2D enemyRig;
    Vector2 startPos;
    SpriteRenderer spriteRend;
    enum facingDir
    {
        right,
        left
    };
    facingDir facing;



    void Awake()
    {
        sensing = GetComponent<EnemyAISensing>();
        enemyRig = GetComponent<Rigidbody2D>();
        spriteRend = GetComponent<SpriteRenderer>();
        gunAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
        bulletTimer = 0;
        startPos = this.transform.position;
        facing = facingDir.left;
        grounded = false;
        ledgeSpotted = false;
        obstacleSpotted = false;
        //  timeToShoot = timeBetweenBullets;
    }
    // Update is called once per frame

    void patrol()
    {
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

    void Update()
    {
        //  Debug.Log(bulletTimer);
        if (sensing.playerInSight())
        {
            bulletTimer += Time.deltaTime;
            if (bulletTimer >= timeBetweenBullets)
            {

                GameObject projectile = (GameObject)Instantiate(bullet, gunBarrell.position, gunBarrell.rotation);
                Rigidbody2D rigidbody = projectile.GetComponent<Rigidbody2D>();
                gunAudio.Play();
                 
                rigidbody.velocity = projectile.transform.right * velocity;
                bulletTimer = 0;
            }
        }
        else
        {
            bulletTimer = 0;
        }
        gunAudio.pitch = Time.timeScale;
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
        //Debug.Log(Vector2.Distance(this.transform.position, obstacleSpotter.point) + " " + obstacleSpotter.distance);
        patrol();
        flipHandler();
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
}
