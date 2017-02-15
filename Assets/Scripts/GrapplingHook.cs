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
    bool ableToShoot;
    bool connected;
    void Start()
    {
        ableToShoot = false;
        connected = false;
        shootSpot = null;
    }
    void Awake () {
        joint = GetComponent<DistanceJoint2D>();
        line = GetComponent<LineRenderer>();
        line.material.color = Color.black;
        targets = new List<GameObject>();
        playMov = GetComponent<PlayerMovement>();
    }
	
	// Update is called once per frame
	void Update () {

        if (targets.Count == 0)
        {
            ableToShoot = false;
            shootSpot = null;
        }
       
        if (ableToShoot && Input.GetButton("FireGHook"))
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
                if (angle > 90)
                {
                    shootSpot = targets[i];
                }
            }
            if (targets.Count == 1)
            {
                shootSpot = targets[0];
            }
            if (shootSpot)
            {
                fireGHook();
            }
        }
        if (connected && Input.GetButton("Jump"))
        {
            joint.enabled = false;
            line.enabled = false;
            connected = false;
        }
        if (connected)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, shootSpot.transform.position);
        }
	}

    void fireGHook()
    {
        line.enabled = true;
        joint.enabled = true;
        joint.connectedBody = shootSpot.GetComponent<Rigidbody2D>();
       // joint.distance = Vector2.Distance(this.transform.position, shootSpot.transform.position);
        connected = true;
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
