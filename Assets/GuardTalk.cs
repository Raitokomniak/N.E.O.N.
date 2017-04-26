using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardTalk : MonoBehaviour {

    public void say(string clip)
    {
        FMODUnity.RuntimeManager.PlayOneShot(clip, this.transform.position);
    }
}
