using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck_feet : MonoBehaviour {

    // Use this for initialization

    bool grounded = false;
    void OnTriggerStay2D(Collider2D col)
    {
        grounded = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        grounded = false;
    }

    public bool isFeetOnGround()
    {
        return grounded;
    }
}
