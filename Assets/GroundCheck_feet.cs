using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck_feet : MonoBehaviour {

    // Use this for initialization

    bool grounded = false;
    void OnTriggerStay2D(Collider2D col)
    {
        Debug.Log("GROUND");
        grounded = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log("NOT GROUND");
        grounded = false;
    }

    public bool isFeetOnGround()
    {
        return grounded;
    }
}
