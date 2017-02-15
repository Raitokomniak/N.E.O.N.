using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    // Use this for initialization
    public float speed = 20;
    public float jumpForce = 10;
    public float maxVelocity = 20;
    public float wallFriction = 6;
    public float timeBetweensteps = 0.5f;
    float stepTimer = 0;
    public AudioClip[] steps;
    AudioSource stepAudio;
    Rigidbody2D playerRig;
    SpriteRenderer sr;
    Animator anim;
    bool grounded;
    bool moving;
    bool wallJumAble;
    bool wallJumped;
    enum facingDirection
    {
        right,
        left
    };
    facingDirection facing;
	void Awake () {
        playerRig = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        stepAudio = GetComponent<AudioSource>();
	}
	void Start()
    {
        grounded = false;
        moving = false;
        wallJumAble = false;
        facing = facingDirection.right;
        anim.Play("Idle");
        wallJumped = false;
        
    }
    void Update()
    {
        stepTimer += Time.deltaTime;
        flipHandler();
    }
	// Update is called once per frame
	void FixedUpdate () {
        movementHandler(); 
        if (grounded && moving&&playerRig.velocity.x !=0)
        {
            stepSounds();
        }
        else
        {
            stepTimer = 0;
        }
        speedHandler();
    }
    void speedHandler()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && grounded)
        {
            playerRig.velocity = new Vector2(playerRig.velocity.x * 0.95f, playerRig.velocity.y);
        }
        if (wallJumped)
        {
            if (playerRig.velocity.y < 0)
            {
                wallJumped = false;
   
            }
        }
    }
    void stepSounds()
    {
        if (stepTimer >= timeBetweensteps)
        {
            int step = Random.Range(0, steps.Length);
            stepAudio.clip = steps[step];
            stepAudio.pitch = Random.Range(0.8f, 1);
            stepAudio.Play();
            stepTimer = 0;
        }
    }
    public bool isFacingRight()
    {
        if (facing== facingDirection.right)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void movementHandler()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if (y < 0 || x != 0||Input.GetButton("Jump")&&grounded)
        {
            
            move(x);
            if (Input.GetButton("Jump")&&grounded)
            {
                jump();
            }
            moving = true;
        }
        else
        {
            moving = false;
        }
        if (playerRig.velocity.y > maxVelocity)
        {
            playerRig.velocity = new Vector2(playerRig.velocity.x, maxVelocity);
        }
        if (Mathf.Abs(playerRig.velocity.x) > maxVelocity)
        {
            float maxSpeed = maxVelocity;
            if (playerRig.velocity.x < 0)
            {
                maxSpeed *= -1;
            }
            playerRig.velocity = new Vector2(maxSpeed, playerRig.velocity.y);
        }
        if (x == 0 && grounded)
        {
            anim.Play("Idle");
        }
        else if (x != 0&&grounded)
        {
            anim.Play("Run");
        }
        if (!grounded&&!wallJumAble)
        {
            anim.Play("MidAir");
        }
        
        if (wallJumAble)
        {
            anim.Play("WallJump");
        }
    }

    void move(float x)
    {
        if (grounded)
        {
            playerRig.AddForce(new Vector2((x * speed), 0));
        }
        else
        {
            playerRig.AddForce(new Vector2((x * (speed/3)), 0));
        }
        if ( x < 0)
        {
            facing = facingDirection.left;
        }
        else if (x > 0)
        {
            facing = facingDirection.right;
        }
       
    }

    void flipHandler()
    {
        if (facing == facingDirection.left)
        {

            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
    }

    void jump()
    {
        playerRig.AddForce(new Vector2(0,jumpForce), ForceMode2D.Impulse);
        grounded = false;
    }

    void wallJump()
    {
        float x = Input.GetAxisRaw("Horizontal");
        //RaycastHit2D ground = Physics2D.Raycast(this.transform.position, -this.transform.up);
        if (Input.GetButton("Jump"))
        {

            int dir = 1;
            if (facing == facingDirection.right)
            {
                dir *= -1;
            }

            playerRig.AddForce(new Vector2(dir * jumpForce / 1.5f, jumpForce), ForceMode2D.Impulse);
            
            wallJumped = true;
        }

        
    }

    void OnCollisionStay2D(Collision2D col)
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        //RaycastHit2D ground = Physics2D.Raycast(this.transform.position, -this.transform.up);
        RaycastHit2D right = Physics2D.Raycast(this.transform.position, this.transform.right);
        RaycastHit2D left = Physics2D.Raycast(this.transform.position, -this.transform.right);
        RaycastHit2D ground = Physics2D.CircleCast(this.transform.position, box.size.x/2, -this.transform.up);
        
        if (ground)
        {
           // if (Vector2.Distance(this.transform.position - this.transform.up, ground.point) < 0.1f)
         //   {
                if (ground.collider == col.collider)
                {
                    grounded = true;
                }
         //   }
            Debug.Log(Vector2.Distance(this.transform.position - this.transform.up, ground.point));  
            
        }
        if (left)
        {
            if (left.collider.gameObject == col.gameObject)
            {
                if (!grounded)
                {
                    if (Input.GetAxisRaw("Horizontal") < 0){
                        wallJump();
                        wallJumAble = true;
                        //Anim walljump ready position
                    }
                    else
                    {
                        wallJumAble = false;
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
                    if (Input.GetAxisRaw("Horizontal") > 0){
                        wallJump();
                        wallJumAble = true;
                        //Anim walljump ready position
                    }
                    else
                    {
                        wallJumAble = false;
                    }
                }
            }
        }
        if (wallJumAble&&!Input.GetButton("Jump"))
        {
            playerRig.velocity = new Vector2(playerRig.velocity.x, -wallFriction * Time.deltaTime);
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
        wallJumAble = false;
    }
    public bool playerMoving()
    {
        return moving;
    }
}
