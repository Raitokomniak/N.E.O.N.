
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public AudioClip[] steps;
    public float speed = 20;
    public float jumpForce = 4;
    public float maxJumpPower = 10;
    public float maxVelocity_run = 20;
    public float maxVelocity_walk = 10;
    public float maxVelocity_sneak = 5;
    public float wallFriction = 6;
    public float timeBetweensteps = 0.5f;
    AudioSource stepAudio;
    Rigidbody2D playerRig;
    BoxCollider2D box;
    SpriteRenderer sr;
    Animator anim;
    GroundCheck_feet feet;
    Collider2D wall;
    float maxVelocity = 5;
    float stepTimer;
    float nroOfCollisions;
    float standingSize;
    float crouchingSize;
    float standingOffset;
    float crouchingOffset;
    int facing; // 1= RIGHT -1 = LEFT
    bool grounded;
    bool moving;
    bool wallJumpAble;
    bool crouched;
    bool ledgeHold;
    bool jumped;
    enum charStates
    {
        idle,
        sneak,
        walk,
        run,
        jump,
        midAir,
        wallSlide,
        wallJump,
        death,
        crouch,
        crouchWalk
    };
    charStates state;

    void Awake()
    {
        playerRig = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        stepAudio = GetComponent<AudioSource>();
        feet = GetComponentInChildren<GroundCheck_feet>();
        box = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        anim.Play("Idle");
        facing = 1;
        grounded = false;
        moving = false;
        wallJumpAble = false;
        crouched = false;
        ledgeHold = false;
        jumped = false;
        nroOfCollisions = 0;
        standingSize = box.size.y;
        crouchingSize = 1.887391f;
        standingOffset = box.offset.y;
        crouchingOffset = -1.010162f;
        RaycastHit2D ground = Physics2D.Raycast(this.transform.position, -this.transform.up);
        wall = null;
        if (ground)
        {
            this.transform.position = new Vector3(ground.point.x, ground.point.y, 0) + this.transform.up;
        }
    }

    void Update()
    {
        flipHandler();
        ledgeCheck();
        grounded = feet.isFeetOnGround();
       // Debug.Log(grounded);
       // Debug.Log("Grounded: "+grounded + " Ledge Hold: " + ledgeHold + " wallJumpAble: " + wallJumpAble);
    }
    void FixedUpdate()
    {
        movementHandler();    
    }

    void climbLedge(float y)
    {
        Debug.DrawRay(this.transform.position-this.transform.up, this.transform.right*facing, Color.red);
        if (ledgeHold&& y > 0.5f)
        {
            //BoxCollider2D box = GetComponent<BoxCollider2D>();
            // this.transform.position = new Vector2(this.transform.position.x + (box.size.x*facing), this.transform.position.y + box.size.y);

            // RaycastHit2D wall = Physics2D.Raycast(this.transform.position - this.transform.up, this.transform.right * facing);
            Collider2D col = wall;
            Vector2 upper = col.bounds.center + (col.bounds.size / 2);
            this.transform.position = new Vector2(this.transform.position.x+((box.size.x)*facing), upper.y+this.transform.up.y);
            crouched = true;
            ledgeHold = false;
            /*RaycastHit2D ground = Physics2D.Raycast(new Vector2(this.transform.position.x + ((box.size.x /2)* facing), box.transform.position.y + box.size.y/2), -box.transform.up);
            if (!(ground.point.y < this.transform.position.y))
            {
                crouched = true;
                this.transform.position = new Vector2(ground.point.x, ground.point.y + this.transform.up.y);
                ledgeHold = false;
            }*/

        }
    }

    void movementHandler()
    {
        float x = Input.GetAxisRaw("Horizontal");
        if (ledgeHold && !Input.GetButton("Crouch"))
        {
             playerRig.velocity = new Vector2(0, 0);
             playerRig.gravityScale = 0;
            //playerRig.isKinematic = true;
           // playerRig.constraints = RigidbodyConstraints2D.FreezePosition;
        }
        else {
           // playerRig.constraints = RigidbodyConstraints2D.FreezeRotation;
            playerRig.isKinematic = false;
            playerRig.gravityScale = 1;
            frictionHandler();
        }
        move(x);
        jump();
        wallJump();
        crouch();
        collisionChecker();
        animationHandler(x);
    }
    void move(float x)
    {
        if (grounded)
        {
            playerRig.AddForce(new Vector2((x * speed), 0));
            charSpeedDefiner(x);
        }
        else
        {
            playerRig.AddForce(new Vector2((x * (speed / 3)), 0));
        }
        if (!wallJumpAble)
        {
            if (x < 0)
            {
                facing = -1;
            }
            else if (x > 0)
            {
                facing = 1;
            }
        }
        float y = Input.GetAxisRaw("Vertical");
        climbLedge(y);
        if (x != 0|| y != 0)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }
        speedLimiter();
    }

    bool crouchChecker()
    {

        RaycastHit2D roof = Physics2D.Raycast(box.transform.position, box.transform.up);
        if (roof)
        {
            Debug.Log(roof.distance);
            return (roof && roof.distance < 1) ? true : false;

        }
        else
        {
            return false;
        }
        
    }

    void crouch()
    {
        if (Input.GetButton("Crouch") && grounded)
        {
            crouched = true;
        }
        
        else if (crouched)
        {
            crouched = crouchChecker();
        }
        else
        {
            crouched = false;
        }
        //this shit is for now
        if (crouched)
        {
            state = charStates.crouch;
            box.size = new Vector2(box.size.x, crouchingSize);
            box.offset = new Vector2(box.offset.x, crouchingOffset);
        }
        else
        {
            box.size = new Vector2(box.size.x, standingSize);
            box.offset = new Vector2(box.offset.x, standingOffset);
        }

    }

    void animationHandler(float x)
    {
        switch (state)
        {
            //To be changed when animations arrive
            case charStates.idle:
                anim.Play("Idle");
                break;
            case charStates.sneak:
                anim.Play("Run");
                break;
            case charStates.walk:
                anim.Play("Run");
                break;
            case charStates.run:
                anim.Play("Run");
                break;
            case charStates.midAir:
                anim.Play("MidAir");
                break;
            case charStates.wallSlide:
                anim.Play("WallJump");
                break;
            case charStates.crouch:
                anim.Play("Crouch");
                break;
            default:
                anim.Play("MidAir");
                break;
        }
    }

    void charSpeedHandler()
    {
        switch (state)
        {
            case charStates.walk:
                maxVelocity = maxVelocity_walk;
                break;
            case charStates.run:
                maxVelocity = maxVelocity_run;
                break;
            case charStates.sneak:
                maxVelocity = maxVelocity_sneak;
                break;
            default:
                maxVelocity = maxVelocity_walk;
                break;
        }
        if (crouched)
        {
            maxVelocity = maxVelocity_sneak;
        }
    }

    void speedLimiter()
    {
        if (Mathf.Abs(playerRig.velocity.x) > maxVelocity)
        {
            float maxSpeed = maxVelocity;
            if (playerRig.velocity.x < 0)
            {
                maxSpeed *= -1;
            }
            float _maxSpeed = Mathf.Lerp(playerRig.velocity.x, maxSpeed, 6 * Time.deltaTime);
            playerRig.velocity = new Vector2(_maxSpeed, playerRig.velocity.y);
        }
        if (playerRig.velocity.y > maxJumpPower)
        {
            playerRig.velocity = new Vector2(playerRig.velocity.x, maxJumpPower);
        }
    }

    void charSpeedDefiner(float x)
    {
        x = Mathf.Abs(x);
        if (x == 0)
        {
            state = charStates.idle;
        }
        else if (x <= 0.35f)
        {
            state = charStates.sneak;
        }
        else if (x <= 0.75f)
        {
            state = charStates.walk;
        }
        else if (x > 0.75f)
        {
            state = charStates.run;
        }
        charSpeedHandler();
    }

    void jump()
    {
        if (grounded && Input.GetButton("Jump"))
        {
            playerRig.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumped = true;
            state = charStates.jump;
        }
    }
    void wallJump()
    {
        if (wallJumpAble)
        {
            state = charStates.wallSlide;
            if (Input.GetButton("Jump"))
            {
                int dir = facing * -1;
                playerRig.AddForce(new Vector2(dir * jumpForce / 1.5f, jumpForce), ForceMode2D.Impulse);
                state = charStates.wallJump;
                wallJumpAble = false;
                facing *= -1;
            }
        }
    }

    void frictionHandler()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && grounded)
        {
            playerRig.velocity = new Vector2(playerRig.velocity.x * 0.9f, playerRig.velocity.y);
        }
        if (wallJumpAble && !Input.GetButton("Jump"))
        {
            playerRig.velocity = new Vector2(playerRig.velocity.x, -wallFriction * Time.deltaTime);
        }
    }

    void flipHandler()
    {
        if (facing == 1)
        {
            sr.flipX = false;
        }
        else
        {
            sr.flipX = true;
        }
    }

    void ledgeCheck()
    {
   
        if ((wallJumpAble||ledgeHold)&&!grounded)
        {
            RaycastHit2D spotter = Physics2D.Raycast(this.transform.position + this.transform.up, this.transform.right * facing);
            RaycastHit2D body = Physics2D.Raycast(this.transform.position, this.transform.right * facing);
            if (!spotter||spotter.distance > body.distance)
            {
                // bool rightPosFound = false;
                if (wall)
                {
                    ledgeHold = true;
                    wallJumpAble = false;
                    Collider2D col = wall;
                    Vector2 upper = col.bounds.center + (col.bounds.size / 2);
                    this.transform.position = new Vector2(this.transform.position.x, upper.y);
                }
                //RaycastHit2D aSpot = Physics2D.Raycast(this.transform.position + this.transform.up/2, this.transform.right * facing);
                //if (!Mathf.Approximately(aSpot.distance, body.distance))
                ///{
                   // this.transform.position = new Vector2(this.transform.position.x, this.transform.position.y - 0.5f); 
               // }
            }
            else
            {
                ledgeHold = false;
            }
        }
        else
        {
            ledgeHold = false;
        }
    }

    void wallCheck(Collider2D col)
    {
       
        if (col.gameObject.tag != "Enemy")
        {
            RaycastHit2D right = Physics2D.Raycast(this.transform.position, this.transform.right);
            RaycastHit2D left = Physics2D.Raycast(this.transform.position, -this.transform.right);
            if (left)
            {
                if (left.collider == col)
                {
                    facing = -1;
                    wallJumpAble = true;
                    wall = col;
                }
            }
            if (right)
            {
                if (right.collider == col)
                {
                    facing = 1;
                    wallJumpAble = true;
                    wall = col;
                }
            }
        }
              
    }

    void collisionChecker()
    {
        if (nroOfCollisions == 0)
        {
            grounded = false;
            wallJumpAble = false;
            ledgeHold = false;
            wall = null;
            state = charStates.midAir;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        nroOfCollisions++;
    }
    void OnCollisionStay2D(Collision2D col)
    { 
        if (!grounded&&!ledgeHold&&!col.collider.isTrigger)
        {
            wallCheck(col.collider);
        }
    }
    void OnCollisionExit2D(Collision2D col)
    {
        if (!grounded)
        {
            RaycastHit2D right = Physics2D.Raycast(this.transform.position, this.transform.right);
            RaycastHit2D left = Physics2D.Raycast(this.transform.position, -this.transform.right);
            if (left || right)
            {
                if (left.collider == col.collider)
                {
                    wallJumpAble = false;
                    wall = null;
                    state = charStates.midAir;
                }
                if (right.collider == col.collider)
                {
                    wallJumpAble = false;
                    wall = null;
                    state = charStates.midAir;
                }
            }
        }
        nroOfCollisions--;
        collisionChecker();
    }

    public bool isFacingRight()
    {
        if (facing == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool playerMoving()
    {
        return moving;
    }

    public bool isGrounded()
    {
        return grounded;
    }
}