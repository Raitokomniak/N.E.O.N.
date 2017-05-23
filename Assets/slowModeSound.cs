using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slowModeSound : MonoBehaviour {
    float currentTimeScale;
    float slowTimeScale = 0.1f;
    float normalTimeScale = 1f;
    FMOD.Studio.EventInstance SlowDown;
    FMOD.Studio.EventInstance SpeedUp;
    FMOD.Studio.PLAYBACK_STATE SlowDownState;
    FMOD.Studio.PLAYBACK_STATE SpeedUpState;
    bool flag;
    // Use this for initialization
    void Awake ()
    {
        SlowDown = FMODUnity.RuntimeManager.CreateInstance("event:/Character sounds/Tachy/Slowdown");
        SpeedUp = FMODUnity.RuntimeManager.CreateInstance("event:/Character sounds/Tachy/Speedup");
	}
	
	// Update is called once per frame
	void Update ()
    {
        currentTimeScale = Time.timeScale;
        SlowDown.getPlaybackState(out SlowDownState);
        SpeedUp.getPlaybackState(out SpeedUpState);
        if (currentTimeScale < slowTimeScale && SlowDownState != FMOD.Studio.PLAYBACK_STATE.PLAYING && SpeedUpState != FMOD.Studio.PLAYBACK_STATE.PLAYING && flag)
        {
            Debug.Log("slow");
            SlowDown.start();
            flag = false;
        }
        else if(currentTimeScale == normalTimeScale  && SlowDownState != FMOD.Studio.PLAYBACK_STATE.PLAYING && SpeedUpState != FMOD.Studio.PLAYBACK_STATE.PLAYING && flag)
        {
            Debug.Log("speed");
            SpeedUp.start();
            flag = false;
        }
        else if(currentTimeScale > 0.4f && currentTimeScale < 0.6f)
        {
            flag = true;
        }
	}
}
