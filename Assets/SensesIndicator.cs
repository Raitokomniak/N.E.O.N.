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
    bool gotHit = false;

    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        handleSprites();

       /* if (Time.timeScale < 0.2f&&!gotHit)
        {
            foreach (SpriteRenderer sense in senses)
            {
                sense.color = Vector4.Lerp(sense.color, new Vector4(1f, 1f, 1f, 0.5f - Time.timeScale), 0.4f * Time.fixedUnscaledDeltaTime);
            }

        }
        else
        {
            foreach (SpriteRenderer sense in senses)
            {
                sense.color = Vector4.Lerp(sense.color, new Vector4(1f, 1f, 1f, 0), 6 * Time.fixedUnscaledDeltaTime);
            }
        }
        oldTimeScale = Time.timeScale;*/
    }

    void handleSprites()
    {
        if (Time.timeScale < 0.2f && !gotHit)
        {
            outerCircle.color = Vector4.Lerp(outerCircle.color, new Vector4(1f, 1f, 1f, 0.5f - Time.timeScale), 0.6f * Time.fixedUnscaledDeltaTime);
            innerCircle.color = Vector4.Lerp(innerCircle.color, new Vector4(1f, 1f, 1f, 0.5f - Time.timeScale), 0.4f * Time.fixedUnscaledDeltaTime);
            coreCircle.color = Vector4.Lerp(coreCircle.color, new Vector4(1f, 1f, 1f, 0.5f - Time.timeScale), 0.2f * Time.fixedUnscaledDeltaTime);
            sight.color = Vector4.Lerp(sight.color, new Vector4(1f, 1f, 1f, 0.5f - Time.timeScale), 0.2f * Time.fixedUnscaledDeltaTime);
            rotateCircles();
        }
        else
        {
            outerCircle.color = Vector4.Lerp(outerCircle.color, new Vector4(1f, 1f, 1f, 0), 6 * Time.fixedUnscaledDeltaTime);
            innerCircle.color = Vector4.Lerp(innerCircle.color, new Vector4(1f, 1f, 1f, 0), 6 * Time.fixedUnscaledDeltaTime);
            coreCircle.color = Vector4.Lerp(coreCircle.color, new Vector4(1f, 1f, 1f, 0), 6 * Time.fixedUnscaledDeltaTime);
            sight.color = Vector4.Lerp(sight.color, new Vector4(1f, 1f, 1f, 0), 6 * Time.fixedUnscaledDeltaTime);
        }
    }

    void rotateCircles()
    {
        outerCircle.transform.Rotate(Vector3.forward * 2*Time.unscaledDeltaTime, Space.World);
        innerCircle.transform.Rotate(-Vector3.forward * 2*Time.unscaledDeltaTime, Space.World);
        coreCircle.transform.Rotate(-Vector3.forward *Time.unscaledDeltaTime, Space.World);
    }

    public void setHitStatus(bool option)
    {
        gotHit = option;
    }
}
