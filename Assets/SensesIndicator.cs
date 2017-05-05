using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensesIndicator : MonoBehaviour {

    // Use this for initialization
    public SpriteRenderer[] senses;
    float oldTimeScale = 1;
    bool gotHit = false;
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {

        if (Time.timeScale < 0.2f&&!gotHit)
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
        oldTimeScale = Time.timeScale;
    }

    public void setHitStatus(bool option)
    {
        gotHit = option;
    }
}
