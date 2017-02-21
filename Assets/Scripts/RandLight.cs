using UnityEngine;
using System.Collections;

public class RandLight : MonoBehaviour {
    public Light source;
    public float intensity;
    public float start = 3.5f;
    public float end = 6f;
    public float rate = 0.01f;
	// Use this for initialization
	void Start () {
        intensity = start;
        StartCoroutine(RandomizingLight());
    }
	
	// Update is called once per frame
	void Update () {
        
        source.intensity = intensity;
        if(intensity <= start)
        {
            StartCoroutine(RandomizingLight());
        }
    }

    private IEnumerator RandomizingLight()
    {
        //while loop for light
        if (intensity <= start)
        {
            while (intensity < end)
            {
                intensity = intensity + rate;
                yield return intensity;
            }
        }
        if (intensity >= end)
        {
            while (intensity >= start)
            {
                intensity = intensity - rate;
                yield return intensity;
            }
        }
     }
    }
