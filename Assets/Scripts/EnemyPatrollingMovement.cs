using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrollingMovement : MonoBehaviour {

    // [FMODUnity.EventRef]
    // public string inputSound = "event:/Input_1";
    public Light gunLite;
    public Transform[] waypoints;
    public Transform gunBarrell;
    public AudioClip[] guardSteps;
    public Vector2 target;
    public Vector2 moveDirection;
    public Vector2 velocity;
    public SpriteRenderer headSpriteRend;
    public SpriteRenderer sightRend;
    public GameObject bullet;
    public GameObject player;
    public float patrollingSpeed = 4;
    public float cautionSpeed = 6;
    public float alertSpeed = 9;
    public float bulletVelocity = 20f;
    public float timeBetweenBullets = 0.5f;
    public float cautionTimer = 5f;
    public float timeToIdleInWayPoint = 2f;
    public int currentWayPoint;
    public bool inUse;
    public bool startPointReached;
    Transform waypoint;
    Rigidbody2D enemyRig;
    public SpriteRenderer spriteRend;
    public SpriteRenderer arms;
    EnemyAISensing sensing;
    GameControllerScript gScript;
    AudioSource gunAudio;
    AudioSource stepAudio;
    Vector3 startPosition;
    Vector3 lastDetectedPosition;
    Animator anim;
    //PlayerInsideAlertZone AlertZone;
    float speed = 8;
    float maxSpeed = 4;
    float timer;
    float bulletTimer;
    float timeToShoot;
    float oldpoint;
    float startingSpeed;
    float searchTimer;
    float timeBetweenSteps = 0;
    float stepTimer;
    float waitTimer = 0;
    int facing;
    bool personalAlert;
    bool playerHeard;
    bool firstTime; 
    bool controlledByGameController;
    bool patrol = true;
    bool grounded;
    bool ledgeSpotted;
    bool obstacleSpotted;
    bool getSilentlyKilled;
    bool gotHit;
    string guardStepSound1 = "event:/Enemy sounds/Guard (cyborg grunt)/Footsteps/Walking";
    string guardStepSound2 = "event:/Enemy sounds/Guard (cyborg grunt)/Footsteps/Brisk walking";
    FMOD.Studio.EventInstance gunSound;
    enum states
    {
        normal,
        caution,
        alert
    };
    states state;
    void Awake()
    {
        AudioSource[] audios = GetComponents<AudioSource>();
        gunAudio = audios[0];
        stepAudio = audios[1];
        enemyRig = GetComponent<Rigidbody2D>();
        //spriteRend = GetComponentInChil<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        sensing = GetComponent<EnemyAISensing>();
        timer = 0f;
       // AlertZone = GetComponentInChildren<PlayerInsideAlertZone>();
        player = GameObject.FindGameObjectWithTag("Player");
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        facing = 1;
        waypoint = (facing == 1) ? waypoints[0] : waypoints[1];
        searchTimer = 0;
        grounded = false;
        firstTime = true;
        startingSpeed = speed;
        oldpoint = 0;
        stepTimer = 0;
        startPosition = this.transform.position;
        controlledByGameController = false;
        inUse = true;
        startPointReached = true;
        getSilentlyKilled = false;
        gotHit = false;
        gunSound = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy sounds/Guard (cyborg grunt)/Gunshots/Single");
        
    }

    void Update()
    {
        float distance = Vector2.Distance(this.transform.position, player.transform.position);
        if (!gotHit)
        {
            if (distance <= 40)
            {
                if (!spriteRend.enabled)
                {
                    toggleObjectOnorOff(true);
                }
                if (!controlledByGameController && !getSilentlyKilled)
                {
                    behaviorHandler();
                }
                flipHandler();
                inUse = true;
                if (Mathf.Abs(enemyRig.velocity.x) < 0.2f)
                {
                    anim.Play("idle");
                }

            }
            else
            {
                if (spriteRend.enabled)
                {
                    toggleObjectOnorOff(false);
                }
                inUse = false;
            }
        }
        if (!sensing.playerInSight() && gunLite.enabled)
        {
            gunLite.enabled = false;
        }

    }

    public void getDaggerHit()
    {
        StartCoroutine(getStunned());
    }

    IEnumerator getStunned()
    {
        //FMODUnity.RuntimeManager.PlayOneShot("event:/Character sounds/GIZMO/Stun pulse", transform.position);
        sensing.setHitStatus(true);
        gotHit = true;
        anim.Play("idle");
        yield return new WaitForSeconds(4f);
        sensing.setHitStatus(false);
        gScript.setAlertState(true);
        gotHit = false;
    }

    public void silentKill(bool option)
    {
        getSilentlyKilled = option;
    }

    public bool playerInSight()
    {
        return sensing.playerInSight();
    }

    public void controlOnGameController(bool control)
    {
        controlledByGameController = control;
    }

    public bool controlledBySomeone()
    {
        return controlledByGameController;
    }

    public void returnToStartPosition()
    {
        waypoint = waypoints[0];
        startPosition = new Vector3(startPosition.x, this.transform.position.y);
        if (!startPointReached)
        {
            moveToDirection(startPosition);
            if (Vector2.Distance(this.transform.position, startPosition) < 1)
            {
                startPointReached = true;
            }
        }

    }

    public bool guardInStartPosition()
    {
        return startPointReached;
    }

    void toggleObjectOnorOff(bool option)
    {
        spriteRend.enabled = option;
       // if (!option)
       // {
            GetComponentInChildren<TakeDamage>().destructLight.enabled = false;
        //}
        if (option == false)
        {
            enemyRig.Sleep();
        }
        else
        {
            enemyRig.WakeUp();
        }
        Light[] lights = GetComponentsInChildren<Light>();
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].enabled = option;
        }
         SpriteRenderer[] childSprites = GetComponentsInChildren<SpriteRenderer>();
         for (int i = 0; i < childSprites.Length; i++)
         {
            if (childSprites[i].name != "exclamation_mark")
            {
                childSprites[i].enabled = option;
            }
         }
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

    void withoutObstacles()
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
        else if (gScript.allGuardsAlerted() && !sensing.playerInSight() && !personalAlert)
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

    void withObstacles()
    {
        if (sensing.playerInSight())
        {
            stop();
        }
        else
        {
            if (state == states.caution && !personalAlert)
            {
                turnAround();
            }
            else if (personalAlert && !sensing.playerInSight())
            {
                stop();
                if (firstTime)
                {
                    firstTime = false;
                    float waitTime = Random.Range(1, 3);
                    Invoke("checkPos", waitTime);
                }
                //stop();
                //personalAlert = false;
            }
            else
            {
                reachedWaypoint();
            }
        }
    }

    void behaviorHandler()
    {
        
        ObstacleCheck();
        startPointReached = false;
        if (!obstacleSpotted && !ledgeSpotted)
        {
            withoutObstacles();
        }
        else
        {
            withObstacles();
        }

    }

    void handleSteps()
    {
        if (enemyRig.velocity.magnitude != 0)
        {
            timeBetweenSteps = 0.75f;
            stepTimer += Time.deltaTime;
            if (stepTimer >= timeBetweenSteps)
            {
                if (state == states.normal)
                {
                    FMODUnity.RuntimeManager.PlayOneShot(guardStepSound1, transform.position);
                }
                else if (state == states.caution || state == states.alert)
                {
                    FMODUnity.RuntimeManager.PlayOneShot(guardStepSound2, transform.position);
                }
                stepTimer = 0;
            }
        }
        
    }

    public void moveToDirection(Vector3 point)
    {
        if (grounded)
        {
            if (!controlledByGameController || (controlledByGameController && !(Mathf.Approximately(this.transform.position.x, startPosition.x))))
            {
                if (Mathf.Abs(enemyRig.velocity.x) < 0.2f)
                {
                    anim.Play("idle");
                }
                else
                {
                    anim.Play("walk");
                }
                anim.speed = state == states.normal ? 0.4f : 0.8f;
                targetOnRightOrLeft(point);
                Vector3 direction = (point - transform.position).normalized;
                //  enemyRig.MovePosition(transform.position + direction * speed * Time.deltaTime);
                enemyRig.AddForce(direction * speed);
                if (Mathf.Abs(enemyRig.velocity.x) > maxSpeed)
                {
                    enemyRig.velocity = new Vector2(maxSpeed * facing, enemyRig.velocity.y);
                }
                handleSteps();
            }
        }

    }

    void WaypointPatrol()
    {
        timer = 0;  
        if (Vector2.Distance (this.transform.position, waypoint.position) < 1)
        {
            reachedWaypoint();
        }
        else
        {
            moveToDirection(waypoint.position);
        }
    }

    void reachedWaypoint()
    {
        waitTimer += Time.deltaTime;
        if (waitTimer > timeToIdleInWayPoint)
        {
            if (waypoint.Equals(waypoints[0]))
            {
                waypoint = waypoints[1];
            }
            else
            {
                waypoint = waypoints[0];
            }
            waitTimer = 0;
        }
    }

    void flipHandler()
    {
        if (facing == 1)
        {
            arms.sortingOrder = headSpriteRend.sortingOrder + 1;
            spriteRend.flipX = false;
			headSpriteRend.flipX = false;
            sightRend.flipX = false;
        }
        else
        {
            arms.sortingOrder = spriteRend.sortingOrder - 1;
            spriteRend.flipX = true;
			headSpriteRend.flipX = true;
            sightRend.flipX = true;
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

    public void stop()
    {
        enemyRig.velocity = new Vector2(0, enemyRig.velocity.y);
    }

    void checkPos()
    {
        stop();
        //yield return new WaitForSeconds(waitTime);
        if (!sensing.playerInSight())
        {
           // turnAround();
            personalAlert = false;
            state = states.caution;
        }
        firstTime = true;
    }

    void checkLastPosition()
    {
        if (lastDetectedPosition != null)
        {
            Vector3 dir = lastDetectedPosition;
            if (Vector3.Distance(this.transform.position, lastDetectedPosition) <= 3)
            {
                stop();
                if (firstTime)
                {
                    firstTime = false;
                    float waitTime = Random.Range(1, 4);
                    Invoke("checkPos", waitTime);
                    //StartCoroutine(checkPos(waitTime));
                }
            }
            else
            {
                moveToDirection(dir);
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
        if (sensing.checkIfPlayerIsBehind())
        {
            //turnAround();
    
        }
        else if (!playerInShootingRange)
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

    void shutDownLight()
    {
        gunLite.enabled = false;
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
                gunLite.enabled = true;
                Invoke("shutDownLight", 0.05f);
                //gunAudio.Play();
                // FMODUnity.RuntimeManager.PlayOneShot(inputSound);
                gunSound.start();
                rigidbody.velocity = projectile.transform.right * bulletVelocity;
                bulletTimer = 0;
            }  
        }
        else
        {
            bulletTimer = 0;
        }
        //gunAudio.pitch = Time.timeScale;
        gunSound.setPitch(Time.timeScale);
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
        float hearingTime = 1.4f;
        float newTimer = 0;
        
        while (newTimer < hearingTime)
        {
            newTimer += Time.deltaTime;
            facing = this.transform.position.x < lastDetectedPosition.x ? 1 : -1;
        }
        personalAlert = true;
    }

    public bool returnStunned()
    {
        return gotHit;
    }

}
