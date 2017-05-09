using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TachyIndicator : MonoBehaviour {

    // Use this for initialization
    public CanvasRenderer[] hexagons;
    void Awake()
    {
        /* hexagons = new SpriteRenderer[GameObject.FindGameObjectsWithTag("Hexagon").Length];
         for (int i = 0; i < GameObject.FindGameObjectsWithTag("Hexagon").Length; i++)
         {
             hexagons[i] = GameObject.FindGameObjectsWithTag("Hexagon")[i].GetComponent<SpriteRenderer>();
         }*/
        for (int i = 0; i < hexagons.Length; i++)
        {
            hexagons[i].SetAlpha(0);
            hexagons[i].SetColor(new Color(0.925f, 0.2f, 0.816f));
        }
    }
	// Update is called once per frame
	void Update () {
        handleHexagons();
	}

    void handleHexagons()
    {
       
        float a = Time.timeScale < 0.2f ? 0.8f - Time.timeScale : 0f;
        for (int i = 0; i < hexagons.Length; i++)
        {
            if (i == 0)
            {
                //hexagons[i].color = Vector4.Lerp(hexagons[i].color, new Vector4(1f, 1f, 1f, a), Time.fixedUnscaledDeltaTime);
               // hexagons[i].SetColor(Vector4.Lerp(hexagons[i].GetColor(), new Vector4(1f, 1f, 1f, a), Time.fixedUnscaledDeltaTime));
                hexagons[i].SetAlpha(Mathf.Lerp(hexagons[i].GetAlpha(), a, Time.fixedUnscaledDeltaTime));
            }
            else if (i == hexagons.Length)
            {
                //hexagons[i].color = Vector4.Lerp(hexagons[i].color, new Vector4(1f, 1f, 1f, a), 0.1f * Time.fixedUnscaledDeltaTime);
              //  hexagons[i].SetColor(Vector4.Lerp(hexagons[i].GetColor(), new Vector4(1f, 1f, 1f, a), 0.1f * Time.fixedUnscaledDeltaTime));
                hexagons[i].SetAlpha(Mathf.Lerp(hexagons[i].GetAlpha(), a, 0.5f * Time.fixedUnscaledDeltaTime));
            }
            else
            {
                // hexagons[i].color = Vector4.Lerp(hexagons[i].color, new Vector4(1f, 1f, 1f, a), i*0.1f * Time.fixedUnscaledDeltaTime);
              //  hexagons[i].SetColor(Vector4.Lerp(hexagons[i].GetColor(), new Vector4(1f, 1f, 1f, a), i *0.1f * Time.fixedUnscaledDeltaTime));
                hexagons[i].SetAlpha(Mathf.Lerp(hexagons[i].GetAlpha(), a,i*0.5f* Time.fixedUnscaledDeltaTime));
            }
        }
    }
}
