using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensesIndicator : MonoBehaviour {

    // Use this for initialization
    public SpriteRenderer[] senses;
    float oldTimeScale = 1;
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale < oldTimeScale)
        {
            foreach (SpriteRenderer sense in senses)
            {
                sense.color = Vector4.Lerp(sense.color, new Vector4(1f, 1f, 1f, 1 - Time.timeScale*0.5f), 0.4f * Time.fixedUnscaledDeltaTime);
            }
        }
        else
        {
            foreach (SpriteRenderer sense in senses)
            {
                sense.color = Vector4.Lerp(sense.color, new Vector4(1f, 1f, 1f, 1 - Time.timeScale), 6 * Time.fixedUnscaledDeltaTime);
            }
        }
        oldTimeScale = Time.timeScale;
    }
}
