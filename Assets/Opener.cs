using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opener : MonoBehaviour {

    // Use this for initialization
    public GameObject checkPoint;
	void Start () {
		
	}

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<PlayerMovement>().gizmo())
            {
                checkPoint.SetActive(true);
                this.gameObject.SetActive(false);
            }
            else
            {
                checkPoint.SetActive(false);
            }
        }
    }
}
