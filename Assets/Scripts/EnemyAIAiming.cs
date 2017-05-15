using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIAiming : MonoBehaviour {

    // Use this for initialization
    public float aimSpeed = 1;
    public Transform gunBarrell;
    public GameObject head;
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
        if (head == null)
        {
            head = this.transform.parent.transform.GetChild(0).gameObject;
        }
    }

    void lightTurner(Light lite)
    {
        if (lite)
        {
            if (movement.facingRight())
            {
                lite.transform.localEulerAngles = new Vector3(0, 75, 0);
            }
            else
            {
                lite.transform.localEulerAngles = new Vector3(0, -75, 0);
            }
            if (stunned && lite.enabled)
            {
                lite.enabled = false;
                foreach (Light li in this.transform.parent.gameObject.GetComponents<Light>())
                {
                    li.enabled = false;
                }
                foreach (Light li in this.transform.parent.gameObject.GetComponentsInChildren<Light>())
                {
                    li.enabled = false;
                }
            }
            else if (!stunned && !lite.enabled)
            {
                lite.enabled = true;
                foreach (Light li in this.transform.parent.gameObject.GetComponents<Light>())
                {
                    li.enabled = true;
                }
                foreach (Light li in this.transform.parent.gameObject.GetComponentsInChildren<Light>())
                {
                    li.enabled = true;
                }
            }
        }
    }
	void Update ()
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

    void turnHead(Vector3 target)
    {
        float angle = 0;
        if (target.y > head.transform.position.y)
        {
            angle = movement.facingRight() ? 9 : -9;
        }
        else if (target.y < head.transform.position.y)
        {
            angle = movement.facingRight() ? -9 : 9;
        }
        GetComponentInParent<EnemyPatrollingMovement>().sightRend.transform.localEulerAngles = new Vector3(0, 0, angle);
        head.transform.localEulerAngles = new Vector3(0, 0, angle);
    }

    void aimTowardPlayer()
    {
        if (light)
        {
            light.transform.localEulerAngles = new Vector3(0, 75, 0);
        }
        
            
        if ((sensing.playerInSight())||movement.checks())
        {
            Vector2 playerPos = sensing.playerLastSeenPosition();
            turnHead(playerPos);
            Vector2 direction = playerPos - new Vector2(this.transform.parent.position.x, this.transform.parent.position.y);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bool turn = true;
            if (movement.facingRight()){
                angle = Mathf.Clamp(angle, -50, 50);
                sensing.checkIfPlayerIsBehind();
            }
            else
            {
                if (angle >-50&&angle < 50)
                {
                    turn = false;
                }
                sensing.checkIfPlayerIsBehind();
            }
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward * Time.smoothDeltaTime);
            if (turn)
            {
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
        }
        else
        {
            normalActivity();
        }
 
    }

    void normalActivity()
    {
        this.transform.rotation = normalPos;
        head.transform.rotation = new Quaternion(0, 0, 0, 0);
        GetComponentInParent<EnemyPatrollingMovement>().sightRend.transform.rotation = new Quaternion(0, 0, 0, 0);
        lightTurner(light);
        lightTurner(spotLight);
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
