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
	public Light spotLight;
    SpriteRenderer sr;
    GameObject player;
    GameControllerScript gScript;
    bool stunned;


    void Awake()
    {
        sensing = GetComponentInParent<EnemyAISensing>();
        movement = GetComponentInParent<EnemyPatrollingMovement>();
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        normalPos = this.transform.rotation;
        stunned = false;
    }

	
	// Update is called once per frame
   
	void FixedUpdate ()
    {
        turnHandler();
        if (!stunned&&!movement.controlledBySomeone())
        {
            aimTowardPlayer();
        }
        else
        {
            normalActivity();
        }

	}
    public void setHitStatus(bool status)
    {
        stunned = status;
    }
    void aimTowardPlayer()
    {
        if (light)
        {
            light.transform.localEulerAngles = new Vector3(0, 75, 0);
        }
        
            
        if ((!gScript.isDead()&&sensing.playerInSight())||movement.checks())
        {
            Vector2 playerPos = sensing.playerLastSeenPosition();
            Vector2 direction = playerPos - new Vector2(this.transform.parent.position.x, this.transform.parent.position.y);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (movement.facingRight()){
                angle = Mathf.Clamp(angle, -50, 50);
                sensing.checkIfPlayerIsBehind();
            }
            else
            {
                sensing.checkIfPlayerIsBehind();
            }
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward * Time.smoothDeltaTime);
            this.transform.rotation = rotation;
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
            normalActivity();
        }
 
    }

    void normalActivity()
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
