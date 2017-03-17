using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck_feet : MonoBehaviour {

    // Use this for initialization

    PlayerMovement playMov;
    GameObject player;
    Collider2D groundCollider;
    BoxCollider2D box;
    bool grounded = false;
    void Awake()
    {
        player = this.transform.parent.gameObject;
        playMov = player.GetComponent<PlayerMovement>();
        box = player.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (!groundCollider)
        {
            grounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {

    }

    void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject != player&&!col.isTrigger)
        {
            int dir = (playMov.isFacingRight()) ? -1 : 1;
            Debug.DrawRay(box.transform.position + new Vector3(box.size.x/2 *dir, 0, 0), -box.transform.up, Color.red);
            RaycastHit2D ground = Physics2D.Raycast(box.transform.position + new Vector3(box.size.x / 2 * dir, 0, 0), -box.transform.up);
            RaycastHit2D ground2 = Physics2D.Raycast(box.transform.position, -box.transform.up);
            if (ground||ground2)
            {
                if (ground.collider == col||ground2.collider == col)
                {
                    grounded = true;
                    groundCollider = col;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col == groundCollider)
        {
            grounded = false;
            groundCollider = null;
        }
    }

    public bool isFeetOnGround()
    {
        return grounded;
    }
}
