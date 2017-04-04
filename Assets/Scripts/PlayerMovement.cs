﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //[FMODUnity.EventRef]
   // public string inputSound = "event:/Input_1";
    public float acceleration = 20;
    public float jumpForce = 8;
    public float maxJumpPower = 12;
    public float maxVelocity_run = 20;
    public float maxVelocity_walk = 10;
    public float wallFriction = 6;
    public float timeBetweensteps = 0.5f;
    float _jumpForce;
    float _maxVelocity_run;
    float _maxVelocity_walk;
    public AudioClip[] stepSounds;
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
    float speed;
    float adrenalineMeter;
    int facing; // 1= RIGHT -1 = LEFT
    bool grounded;
    bool moving;
    bool wallJumpAble;
    bool crouched;
    bool ledgeHold;
    bool jumped;
    bool adrenalineRush;
    bool reducing;
    bool ableToRegenerate;
    bool adding;
    bool checker;

    enum charStates
    {
        idle,
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
        stepTimer = 0;
        initialize();

        
    }

    void steps()
    {
        if (playerRig.velocity.magnitude != 0)
        {
            stepAudio.clip = stepSounds[Random.Range(0, stepSounds.Length)];
            timeBetweensteps = 0.25f;
            stepTimer += Time.deltaTime;
            if (stepTimer >= timeBetweensteps&&state == charStates.run)
            {
                stepAudio.volume = 0.4f;
                //  FMODUnity.RuntimeManager.PlayOneShot(inputSound);
                stepAudio.pitch = Random.Range(0.90f, 1f);
                stepAudio.Play();
                stepTimer = 0;
            }
        }
        speed = 0;
    }
  
    void Start()
    {
        RaycastHit2D ground = Physics2D.Raycast(this.transform.position, -this.transform.up);
        if (ground)
        {
            this.transform.position = new Vector3(ground.point.x, ground.point.y, 0) + this.transform.up;
        }
    }

    void initialize()
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
        wall = null;
        reducing = false;
        adrenalineRush = false;
        ableToRegenerate = true;
        adding = false;
        checker = false;
        adrenalineMeter = 10;
    }

    void Update()
    {
        handleAbilities();
        setPowers();
        flipHandler();
        ledgeCheck();
        grounded = feet.isFeetOnGround();
        jump();
        wallJump();
        speedLimiter();
        Debug.Log(playerRig.velocity.x);
    }

    void handleAbilities()
    {
        if (Input.GetButtonDown("LeftBumber"))
        {
            if (adrenalineRush)
            {
                setAdrenalineRush(false);
            }
            else if (!checker)
            {
                setAdrenalineRush(true);
            }
        }
        if (adrenalineRush&&!reducing)
        {
            reducing = true;
            StartCoroutine(drainMeter());
        }

        if (!adrenalineRush && ableToRegenerate && !adding)
        {
            adding = true;
            StartCoroutine(regenerateAdrenaline());
        }
    }

    void setAdrenalineRush(bool option)
    {
        if ((option == true&&adrenalineMeter !=0)||option == false)
        {
            adrenalineRush = option;
        }
    }

    IEnumerator drainMeter()
    {
        yield return new WaitForSeconds(0.5f);
        reducing = false;
        adrenalineMeter--;
        if (adrenalineMeter <= 0)
        {
            //ableToRegenerate = false;
            checker = true;
            StartCoroutine(adrenalineBack());
            setAdrenalineRush(false);
        }
    }

    IEnumerator adrenalineBack()
    {
        yield return new WaitForSeconds(5f);
        ableToRegenerate = true;
    }

    IEnumerator regenerateAdrenaline()
    {
        yield return new WaitForSeconds(0.5f);
        
        adrenalineMeter++;
        if (adrenalineMeter >= 10)
        {
            adrenalineMeter = 10;
            checker = false;
            ableToRegenerate = false;
        }
        adding = false;
    }

    void setPowers()
    {
        int power = (adrenalineRush) ? 2 : 1;
        _maxVelocity_run = maxVelocity_run * power;
        _maxVelocity_walk = maxVelocity_walk;
        _jumpForce = jumpForce * power;
        speed = acceleration * power;
    }
    void FixedUpdate()
    {
        movementHandler();    
    }

    void climbLedge(float y)
    {
        if (ledgeHold&& y > 0.5f)
        {
            Collider2D col = wall;
            Vector2 upper = col.bounds.center + (col.bounds.size / 2);
            int dir = (this.transform.position.x > wall.transform.position.x) ? -1 : 1;
            this.transform.position = new Vector2(this.transform.position.x+((box.size.x)*dir), upper.y+this.transform.up.y);
            crouched = true;
            ledgeHold = false;
        }
    }

    void movementHandler()
    {
        float x = Input.GetAxisRaw("Horizontal");
        if (ledgeHold && !Input.GetButton("Crouch"))
        {
             playerRig.velocity = new Vector2(0, 0);
             playerRig.gravityScale = 0;
        }
        else {
            playerRig.isKinematic = false;
            playerRig.gravityScale = 1;
            frictionHandler();
        }
        move(x);
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
            steps();
        }
        else
        {
            float divider = GetComponent<DistanceJoint2D>().enabled ? 3 : 4.5f;
            float something = GetComponent<DistanceJoint2D>().enabled ? acceleration : 20;
            playerRig.AddForce(new Vector2((x * (something / 3)), 0));
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
        
    }

    bool crouchChecker()
    {
        RaycastHit2D roof = Physics2D.Raycast(box.bounds.center, box.transform.up);
        if (roof)
        {
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
            case charStates.walk:
                anim.Play("Run");
                break;
            case charStates.run:
                anim.Play("Run");
                break;
            case charStates.midAir:
                anim.Play("MidAir");
                break;
            case charStates.jump:
                anim.Play("MidAir");
                break;
            case charStates.wallJump:
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
        if (grounded)
        {
            switch (state)
            {
                case charStates.walk:
                    maxVelocity = _maxVelocity_walk;
                    break;
                case charStates.run:
                    maxVelocity = _maxVelocity_run;
                    break;
                default:
                    maxVelocity = _maxVelocity_walk;
                    break;
            }
            if (crouched)
            {
                maxVelocity = _maxVelocity_walk / 2;
            }
        }
        else
        {
            maxVelocity = maxVelocity_run * 2;
        }
    }

    void speedLimiter()
    {
        if (Mathf.Abs(playerRig.velocity.x) >= maxVelocity)
        {
            int dir = (playerRig.velocity.x < 0) ? -1 : 1;
            playerRig.velocity = new Vector2(maxVelocity * dir, playerRig.velocity.y);
        }
        if (playerRig.velocity.y > maxJumpPower)
        {
            playerRig.velocity = new Vector2(playerRig.velocity.x, maxJumpPower);
        }

    }

    void charSpeedDefiner(float x)
    {
        x = Mathf.Abs(x);
        if (!crouched)
        {
            if (x <= 0.1f)
            {
                state = charStates.idle;
            }
            else if (x <= 0.65f)
            {
                state = charStates.walk;
            }
            else if (x > 0.75f)
            {
                state = charStates.run;
            }
        }
        charSpeedHandler();
    }

    void jump()
    {
        if (grounded && Input.GetButtonDown("Jump"))
        {
            playerRig.AddForce(new Vector2(0, _jumpForce), ForceMode2D.Impulse);
            state = charStates.jump;
            jumped = false;
        }
    }
    void wallJump()
    {
        if (wallJumpAble)
        {
            state = charStates.wallSlide;
            if (Input.GetButtonDown("Jump"))
            {
                int dir = facing * -1;
                playerRig.AddForce(new Vector2(dir * _jumpForce / 1.5f, _jumpForce), ForceMode2D.Impulse);
                state = charStates.wallJump;
                wallJumpAble = false;
                jumped = false;
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
        sr.flipX = (facing == 1) ? false : true;
    }

    void ledgeCheck()
    {
   
        if ((wallJumpAble||ledgeHold)&&!grounded)
        {
            RaycastHit2D spotter = Physics2D.Raycast(this.transform.position + this.transform.up, this.transform.right * facing);
            RaycastHit2D body = Physics2D.Raycast(this.transform.position, this.transform.right * facing);
            if (!spotter||spotter.distance > body.distance)
            {
                if (wall)
                {
                    ledgeHold = true;
                    wallJumpAble = false;
                    Collider2D col = wall;
                    Vector2 upper = col.bounds.center + (col.bounds.size / 2);
                    this.transform.position = new Vector2(this.transform.position.x, upper.y);
                }
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
        if (nroOfCollisions == 0|| (nroOfCollisions != 0 && !grounded && !ledgeHold && !wallJumpAble))
        {
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
    public void reset()
    {
        initialize();
    }

    public bool isFacingRight()
    {
        return (facing == 1) ? true : false;
    }

    public bool playerMoving()
    {
        return moving;
    }

    public bool isGrounded()
    {
        return grounded;
    }
    public string getState()
    {
        return state.ToString();
    }
}