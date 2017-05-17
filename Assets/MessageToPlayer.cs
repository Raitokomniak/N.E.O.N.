using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageToPlayer : MonoBehaviour {

    // Use this for initialization
    BubbleScript bubble;
    public string message;
    bool flag;
	void Awake () {
        bubble = GameObject.FindGameObjectWithTag("GameController").GetComponent<BubbleScript>();
        flag = true;
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D col)
    {
	    if (col.gameObject.CompareTag("Player")&&flag)
        {
            bubble.setText(message, 5f);
            flag = false;
        }
	}
}
