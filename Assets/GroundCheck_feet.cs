using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck_feet : MonoBehaviour {

    // Use this for initialization

    bool grounded = false;
    GameObject player;

    void Awake()
    {
        player = this.transform.parent.gameObject;
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject != player&&!col.isTrigger)
        {
            grounded = true;
        }
       // Debug.Log(col.gameObject);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject != player && !col.isTrigger)
        {
            grounded = false;
        }
    }

    public bool isFeetOnGround()
    {
        return grounded;
    }
}
