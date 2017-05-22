using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertLightScript : MonoBehaviour {

    // Use this for initialization
    Light[] alertLights;
    public float waitTime = 0.5f;
    public float speed = 20;
    public float targetIntensity = 20;
    float nextIntensity = 0;
    float currentIntensity = 0;
    bool wait = false;
	void Awake () {
        GameObject[] alert = GameObject.FindGameObjectsWithTag("AlertLight");
        alertLights = new Light[alert.Length];
        for (int i = 0; i < alert.Length; i++)
        {
            alertLights[i] = alert[i].GetComponentInChildren<Light>();
            currentIntensity = alertLights[i].intensity;
        }
        nextIntensity = targetIntensity;
        
	}
	
	// Update is called once per frame
	void Update () {
        blinkLights();
	}

    void blinkLights()
    {
        if (!wait)
        {
            foreach (Light lite in alertLights)
            {
                lite.intensity = currentIntensity;
            }
            currentIntensity = Mathf.Lerp(currentIntensity, nextIntensity, speed * Time.deltaTime);
            if (currentIntensity == nextIntensity)
            {
                wait = true;
                Invoke("waitForChange", waitTime);
                nextIntensity = nextIntensity == 0 ? targetIntensity : 0;
            }
        }
    }

    void waitForChange()
    {
        wait = false;
    }
}
