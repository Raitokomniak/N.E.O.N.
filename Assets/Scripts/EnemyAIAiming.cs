using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIAiming : MonoBehaviour {

    // Use this for initialization
    public float aimSpeed = 1;
    public Transform gunBarrell;
    EnemyAISensing sensing;
    EnemyPatrollingMovement movement;
    Quaternion normalPos;
    public Light light;
    SpriteRenderer sr;
    GameObject player;
    GameControllerScript gScript;

    void Awake()
    {
        sensing = GetComponentInParent<EnemyAISensing>();
        movement = GetComponentInParent<EnemyPatrollingMovement>();
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }
    void Start () {
        normalPos = this.transform.rotation;
	}
	
	// Update is called once per frame
   
	void FixedUpdate () {
        turnHandler();
        aimTowardPlayer();
	}

    void aimTowardPlayer()
    {
        if (light)
        {
            light.transform.localEulerAngles = new Vector3(0, 75, 0);
        }
        
            
            if (!gScript.isDead()&&sensing.playerInSight())
            {
            
                Vector2 playerPos = player.transform.position;
                Vector2 direction = playerPos - new Vector2(this.transform.parent.position.x, this.transform.parent.position.y);
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward*Time.smoothDeltaTime);
                this.transform.rotation = rotation;
               // this.transform.rotation = Quaternion.FromToRotation(this.transform.position, direction * Time.deltaTime);
                if (!movement.facingRight())
                {
                    sr.flipY = true;
                    sr.flipX = false;
                }
                else
                {
                    sr.flipY = false;
                }
            }
            else
            {
                this.transform.rotation = normalPos;
                if (light)
                {
                    if (movement.facingRight())
                    {
                        light.transform.localEulerAngles = new Vector3(0, 75, 0);
                    }
                    else
                    {
                        light.transform.localEulerAngles = new Vector3(0, -75, 0);
                    }
                }
                if (movement.facingRight())
                {
                    sr.flipY = false;
                    sr.flipX = false;
                }
                else
                {
                    sr.flipY = false;
                }
            }
 
    }

    void turnHandler()
    {
        if (movement.facingRight())
        {
            sr.flipX = false;

        }
        else
        {
            sr.flipX = true;
            sr.flipY = false;
        }
    }
}
