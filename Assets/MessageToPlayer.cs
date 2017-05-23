using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageToPlayer : MonoBehaviour {

    // Use this for initialization
    BubbleScript bubble;
    public string[] messages;
    public float textShowTime = 5;
    public bool showChar = true;
    bool flag;
	void Awake () {
        bubble = GameObject.FindGameObjectWithTag("GameController").GetComponent<BubbleScript>();
        flag = true;
        
	}
    void Start()
    {
        bubble.showChar = showChar;
    }
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D col)
    {
	    if (col.gameObject.CompareTag("Player")&&flag)
        {
            //StartCoroutine(writeText());
            bubble.setChainedText(messages, textShowTime);
            flag = false;
        }
	}
}
