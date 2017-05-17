using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleScript : MonoBehaviour {

    // Use this for initialization
    public GameObject bubble;
    CanvasRenderer speechBubble;
    Text speechText;
    GameControllerScript gScript;
    float time;

	void Awake () {
        speechBubble = bubble.GetComponent<CanvasRenderer>();
        speechText = bubble.GetComponentInChildren<Text>();
        bubble.SetActive(false);
        gScript = GetComponent<GameControllerScript>();
        time = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (time > 0 && bubble.activeSelf)
        {
            time -= Time.unscaledDeltaTime;
        }
        else if (time <= 0 && bubble.activeSelf)
        {
            setActive(false);
            gScript.setCharacterVisible(false);
        }
	}

    public void setText(string text, float showTime)
    {
        time = showTime;
        gScript.setCharacterVisible(true);
        speechText.text = text;
        setActive(true);
    }

    public void setChainedText(string[] text, float showTime)
    {
        StartCoroutine(writeText(text, showTime));
    }

    IEnumerator writeText(string[] messages, float textShowTime)
    {
        foreach (string message in messages)
        {
            setText(message, textShowTime);
            yield return new WaitForSecondsRealtime(textShowTime);
        }
    }
    public void setActive(bool option)
    {
        bubble.SetActive(option);
    }
}
