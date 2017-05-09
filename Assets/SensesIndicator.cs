using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensesIndicator : MonoBehaviour {

    // Use this for initialization
    public SpriteRenderer outerCircle;
    public SpriteRenderer innerCircle;
    public SpriteRenderer coreCircle;
    public SpriteRenderer sight;
    float oldTimeScale = 1;
    GameControllerScript gScript;
    bool gotHit = false;

    void Awake () {
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
	}

    // Update is called once per frame
    void Update()
    {
        handleSprites();
    }

    void handleSprites()
    {
        if (Time.timeScale < 0.2f && !gotHit)
        {
            Vector4 sightColor = gScript.allGuardsAlerted() ? new Vector4(1f, 1f, 1f, 0.5f - Time.timeScale) : new Vector4(0.196f, 1f, 0.196f, 0.5f - Time.timeScale);
            outerCircle.color = Vector4.Lerp(outerCircle.color, new Vector4(1f, 1f, 1f, 0.5f - Time.timeScale), 0.6f * Time.fixedUnscaledDeltaTime);
            innerCircle.color = Vector4.Lerp(innerCircle.color, new Vector4(1f, 1f, 1f, 0.5f - Time.timeScale), 0.4f * Time.fixedUnscaledDeltaTime);
            coreCircle.color = Vector4.Lerp(coreCircle.color, new Vector4(1f, 1f, 1f, 0.5f - Time.timeScale), 0.2f * Time.fixedUnscaledDeltaTime);
            sight.color = Vector4.Lerp(sight.color, sightColor, 0.1f * Time.fixedUnscaledDeltaTime);
            rotateCircles();
        }
        else
        {
            Vector4 sightColor = gScript.allGuardsAlerted() ? new Vector4(1f, 1f, 1f, 0f) : new Vector4(0.196f, 1f, 0.196f, 0f);
            outerCircle.color = Vector4.Lerp(outerCircle.color, new Vector4(1f, 1f, 1f, 0), 6 * Time.fixedUnscaledDeltaTime);
            innerCircle.color = Vector4.Lerp(innerCircle.color, new Vector4(1f, 1f, 1f, 0), 6 * Time.fixedUnscaledDeltaTime);
            coreCircle.color = Vector4.Lerp(coreCircle.color, new Vector4(1f, 1f, 1f, 0), 6 * Time.fixedUnscaledDeltaTime);
            sight.color = Vector4.Lerp(sight.color, sightColor, 6 * Time.fixedUnscaledDeltaTime);
        }
    }

    void rotateCircles()
    {
        int multiplier = gScript.allGuardsAlerted() ? 2 : 1;
        outerCircle.transform.Rotate(Vector3.forward * 2*Time.unscaledDeltaTime * multiplier, Space.World);
        innerCircle.transform.Rotate(-Vector3.forward * 2*Time.unscaledDeltaTime * multiplier, Space.World);
        coreCircle.transform.Rotate(-Vector3.forward *Time.unscaledDeltaTime * multiplier, Space.World);
    }

    public void setHitStatus(bool option)
    {
        gotHit = option;
    }
}
