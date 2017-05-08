using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineHum : MonoBehaviour {
    string engineHum = "event:/Environmental sounds/Engine hum";
    FMOD.Studio.EventInstance engineSound;
    FMOD.ATTRIBUTES_3D Attributes;
    // Use this for initialization
    void Awake ()
    {
        engineSound = FMODUnity.RuntimeManager.CreateInstance(engineHum);
        Vector3 position = transform.position;
        Attributes = FMODUnity.RuntimeUtils.To3DAttributes(position);
        engineSound.set3DAttributes(Attributes);
        engineSound.start();
    }

    ~EngineHum()
    {
        engineSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
