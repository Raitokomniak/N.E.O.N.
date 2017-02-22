using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public AudioClip[] steps;
    public float speed = 20;
    public float jumpForce = 4;
    public float maxVelocity_run = 20;
    public float maxVelocity_walk = 10;
    public float maxVelocity_sneak = 5;
    public float wallFriction = 6;
    public float timeBetweensteps = 0.5f;
    AudioSource stepAudio;
    Rigidbody2D playerRig;
    SpriteRenderer sr;
    Animator anim;
    float maxVelocity = 5;
    float stepTimer;
    int facing; // 1= RIGHT -1 = LEFT
    bool grounded;
    bool moving;
    bool wallJumpAble;
    bool wallJumped;
    enum charStates
    {
        idle,
        sneak,
        walk,
        run,
        jump,
        midAir,
        wallSlide,
        wallJump
    };
    charStates state;

    void Awake()
    {
        playerRig = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        stepAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
        anim.Play("Idle");
        facing = 1;
        grounded = false;
        moving = false;
        wallJumpAble = false;
        wallJumped = false;
    }

    void Update()
    {
        flipHandler();
        Debug.Log(state);
    }
    void FixedUpdate()
    {
        movementHandler();
    }

    void movementHandler()
    {
        float x = Input.GetAxisRaw("Horizontal");
        move(x);
        frictionHandler();
        jump();
        wallJump();
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
        if (x < 0)
        {
            facing = -1;
        }
        else if (x > 0)
        {
            facing = 1;
        }
        if(x != 0)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }
    }

    void animationHandler(float x)
    {
      /*  if (x == 0 && grounded)
        {
            anim.Play("Idle");
        }
        else if (x != 0 && grounded)
        {
            anim.Play("Run");
        }
        if (!grounded && !wallJumpAble)
        {
            anim.Play("MidAir");
        }

        if (wallJumpAble)
        {
            anim.Play("WallJump");
        }*/
        switch (state)
        {
            case charStates.idle:
                anim.Play("Idle");
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
            grounded = false;
            state = charStates.jump;
        }
    }
    void wallJump()
    {
        if (wallJumpAble)
        {
            if (Input.GetButton("Jump"))
            {
                int dir = facing * -1;
                playerRig.AddForce(new Vector2(dir * jumpForce / 1.5f, jumpForce), ForceMode2D.Impulse);
                state = charStates.wallJump;
                wallJumped = true;
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
    void OnCollisionStay2D(Collision2D col)
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        RaycastHit2D right = Physics2D.Raycast(this.transform.position, this.transform.right);
        RaycastHit2D left = Physics2D.Raycast(this.transform.position, -this.transform.right);
        RaycastHit2D ground = Physics2D.CircleCast(this.transform.position, box.size.x / 2, -this.transform.up);

        if (ground)
        {
            if (ground.collider == col.collider)
            {
                grounded = true;
            }
        }
        if (left)
        {
            if (left.collider.gameObject == col.gameObject)
            {
                if (!grounded)
                {
                    if (Input.GetAxisRaw("Horizontal") < 0)
                    {
                        wallJumpAble = true;
                        state = charStates.wallSlide;
                    }
                    else
                    {
                        wallJumpAble = false;
                        state = charStates.midAir;
                    }
                }
            }

        }
        if (right)
        {
            if (right.collider.gameObject == col.gameObject && !grounded)
            {
                if (!grounded)
                {
                    if (Input.GetAxisRaw("Horizontal") > 0)
                    {
                        state = charStates.wallSlide;
                        wallJumpAble = true;
                    }
                    else
                    {
                        wallJumpAble = false;
                        state = charStates.midAir;
                    }
                }
            }

        }
    }
    void OnCollisionExit2D(Collision2D col)
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        RaycastHit2D ground = Physics2D.CircleCast(this.transform.position, box.size.x / 2, -this.transform.up);
        if (ground)
        {
            if (ground.collider.gameObject == col.gameObject)
            {
                grounded = false;
            }
        }
        wallJumpAble = false;
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
}
