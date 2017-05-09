using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicController : MonoBehaviour {
    static musicController instance;
    string music = "event:/Music/Background 1";
    static FMOD.Studio.EventInstance Music;
    FMOD.Studio.PLAYBACK_STATE musicState;
    // Use this for initialization
    void Awake () {
		if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        if(Music == null)
        {
            Music = FMODUnity.RuntimeManager.CreateInstance(music);
        }

    }

    void Start()
    {
        Music.setParameterValue("Music speed", 0);
        Music.getPlaybackState(out musicState);
        Debug.Log(musicState);
        if (musicState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            Music.start();
        }
    }


    public void setSlowMusic()
    {
        Music.setParameterValue("Music speed", 0);
    }
    public void setFastMusic()
    {
        Music.setParameterValue("Music speed", 1);
    }
    public void stopMusic()
    {
        Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
