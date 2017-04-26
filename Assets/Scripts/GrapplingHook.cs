using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour {

    // Use this for initialization

    DistanceJoint2D joint;
    GameObject shootSpot;
    LineRenderer line;
    List<GameObject> targets;
    PlayerMovement playMov;
    float distance;
    bool ableToShoot;
    bool connected;
    string graplinghookSound = "event:/Character sounds/GIZMO/Attach (energy)";
    
    FMOD.Studio.EventInstance hookSound;

    void Start()
    {
        ableToShoot = false;
        connected = false;
        shootSpot = null;
        distance = 0;
    }

    void Awake () {
        joint = GetComponent<DistanceJoint2D>();
        line = GetComponent<LineRenderer>();
        line.material.color = Color.black;
        targets = new List<GameObject>();
        playMov = GetComponent<PlayerMovement>();
        hookSound = FMODUnity.RuntimeManager.CreateInstance(graplinghookSound);
        
    }

    ~GrapplingHook()
    {
        hookSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    // Update is called once per frame
    void Update () {
        if (targets.Count == 0)
        {
            ableToShoot = false;
            shootSpot = null;
        }
        else
        {
            setShootSpot();
        }
       
        
        if (connected && Input.GetButton("Jump"))
        {
            joint.enabled = false;
            line.enabled = false;
            connected = false;
            hookSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        
       
    }

    void FixedUpdate()
    {
        if (ableToShoot && Input.GetButton("FireGHook") && !connected)
        {
            
            if (shootSpot)
            {
                fireGHook();
                FMODUnity.RuntimeManager.PlayOneShot(graplinghookSound);
                hookSound.start();
            }
        }

        if (connected)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, shootSpot.transform.position);
            joint.distance = distance;
        }

        if (connected && Input.GetButton("FireGHook"))
        {
            distance -= 10 * Time.smoothDeltaTime;
        }
    }

    void fireGHook()
    {
        line.enabled = true;
        joint.enabled = true;
        joint.connectedBody = shootSpot.GetComponent<Rigidbody2D>();
        distance = Vector2.Distance(this.transform.position, shootSpot.transform.position);
        connected = true;
    }

    void setShootSpot()
    {
        float closestDistance = 0;

        for (int i = 0; i < targets.Count; i++)
        {
            //Very much in the works
            int dir = 1;
            if (!playMov.isFacingRight())
            {
                dir *= -1;
            }
            Vector2 directionToTarget = transform.position - targets[i].transform.position;
            float angle = Vector2.Angle(transform.right * dir, directionToTarget);
            float distance = directionToTarget.magnitude;
            if (closestDistance == 0)
            {
                closestDistance = distance;
            }
            if (distance < closestDistance && angle > 90)
            {
                closestDistance = distance;
            }
            if (angle > 90)
            {
                if (closestDistance == distance && targets[i].transform.position.y > this.transform.position.y)
                {
                    shootSpot = targets[i];
                    shootSpot.GetComponent<VantagePointScript>().setLight(3);
                }
            }
            if (shootSpot != targets[i] && !connected)
            {
                targets[i].GetComponent<VantagePointScript>().setLight(1);
            }
        }
        if (!shootSpot)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                int dir = 1;
                if (!playMov.isFacingRight())
                {
                    dir *= -1;
                }
                Vector2 directionToTarget = transform.position - targets[i].transform.position;
                float angle = Vector2.Angle(transform.right * dir, directionToTarget);
                float distance = directionToTarget.magnitude;
                if (distance < closestDistance && angle > 90)
                {
                    closestDistance = distance;
                }
                if (angle > 90)
                {

                    if (closestDistance == distance)
                    {
                        shootSpot = targets[i];
                    }
                }
                if (shootSpot != targets[i] && !connected)
                {
                    targets[i].GetComponent<VantagePointScript>().setLight(1);
                }
            }

        }
    }
    public void setGHookable(GameObject vantagePos)
    {
       if (!targets.Contains(vantagePos))
       {
           targets.Add(vantagePos);
        } 
        ableToShoot = true;
    }

    public void unSetGHookable(GameObject vantagePos)
    {
        targets.Remove(vantagePos);
    }
}
