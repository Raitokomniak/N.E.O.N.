﻿using System.Collections;
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
           shootSpot = setShootSpot(shootSpot);
        }
       
        
        if (connected && Input.GetButton("Jump"))
        {
            joint.enabled = false;
            line.enabled = false;
            connected = false;
            hookSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        if (ableToShoot && Input.GetButtonDown("FireGHook") && !connected)
        {
            Vector2 direction = shootSpot.transform.position - this.transform.position;
            RaycastHit2D ray = Physics2D.Raycast(this.transform.position, direction);
            Debug.DrawRay(this.transform.position, direction, Color.red);
            if (ray.collider.gameObject == shootSpot)
            {
                if (shootSpot)
                {
                    fireGHook();
                    hookSound.start();
                }
            }
        }

    }

    void FixedUpdate()
    {
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
        joint.connectedBody = shootSpot.GetComponent<Rigidbody2D>();
        distance = Vector2.Distance(this.transform.position, shootSpot.transform.position);
        connected = true;
        line.enabled = true;
        joint.enabled = true;
    }

    GameObject setShootSpot(GameObject shooter)
    {
        float closestDistance = Vector2.Distance(this.transform.position, targets[0].transform.position);
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
                if (targets[i].transform.position.y > this.transform.position.y)
                {
                    shooter = targets[i];
                    Debug.Log("Valittiin taman perusteella");
                    shooter.GetComponent<VantagePointScript>().setLight(3);
                }
                else if (closestDistance == distance)
                {
                    shooter = targets[i];
                    Debug.Log("Valittiin taman toisen perusteella");
                }
            }
            if (shooter != targets[i] && !connected)
            {
                targets[i].GetComponent<VantagePointScript>().setLight(1);
            }
        }
        if (!shooter)
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
                        shooter = targets[i];
                        
                    }
                }
                if (shooter != targets[i] && !connected)
                {
                    targets[i].GetComponent<VantagePointScript>().setLight(1);
                }
            }

        }
        return connected ? shootSpot : shooter;
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
