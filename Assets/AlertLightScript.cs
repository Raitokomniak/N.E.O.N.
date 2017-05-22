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
    GameControllerScript gScript;
	void Awake () {
        GameObject[] alert = GameObject.FindGameObjectsWithTag("AlertLight");
        alertLights = new Light[alert.Length];
        for (int i = 0; i < alert.Length; i++)
        {
            alertLights[i] = alert[i].GetComponentInChildren<Light>();
            currentIntensity = alertLights[i].intensity;
        }
        nextIntensity = targetIntensity;
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
	}
	
	// Update is called once per frame
	void Update () {
        blinkLights();
	}

    void blinkLights()
    {
        float _speed = gScript.allGuardsAlerted() ? speed * 1.5f : speed;
        _speed = nextIntensity == 0 ? _speed * 3 : _speed;
        if (!wait)
        {
            foreach (Light lite in alertLights)
            {
                lite.intensity = gScript.allGuardsAlerted() ? currentIntensity *1.5f : currentIntensity;
                lite.range = gScript.allGuardsAlerted() ? currentIntensity / 1.2f : currentIntensity / 2;
            }
            currentIntensity = Mathf.Lerp(currentIntensity, nextIntensity, _speed * Time.deltaTime);
            if (Mathf.Approximately(currentIntensity, nextIntensity))
            {
              //  
            /* (   if (Mathf.Approximately(currentIntensity, targetIntensity))
                {
                    wait = true;
                    Invoke("waitForChange", waitTime);
                }        */
                nextIntensity = nextIntensity == 0 ? targetIntensity : 0;
            }
        }
    }

    void waitForChange()
    {
        wait = false;
    }
}
