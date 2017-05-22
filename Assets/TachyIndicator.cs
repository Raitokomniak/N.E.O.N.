using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TachyIndicator : MonoBehaviour {

    // Use this for initialization
    public CanvasRenderer[] hexagons;
    void Awake()
    {
        for (int i = 0; i < hexagons.Length; i++)
        {
            
            // hexagons[i].SetAlpha(0);
        }
    }
	void Update () {
        
        handleHexagons();
	}

    void handleHexagons()
    {     
        float a = Time.timeScale < 0.2f ? 1 - Time.timeScale : 0f;
        for (int i = 0; i < hexagons.Length; i++)
        {
            if (i == 0)
            {
                hexagons[i].SetAlpha(Mathf.Lerp(hexagons[i].GetAlpha(), a, Time.fixedUnscaledDeltaTime));
            }
            else
            {
                hexagons[i].SetAlpha(Mathf.Lerp(hexagons[i].GetAlpha(), a,i*0.5f* Time.fixedUnscaledDeltaTime));
            }
        }
    }
}
