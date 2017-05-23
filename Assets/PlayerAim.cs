using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour {

    // Use this for initialization
    public GameObject dagger;
    public float daggerThrowVelocity = 15f;
    public float timeBetweedThrows = 2f;
    PlayerMovement playMov;
    GameControllerScript gScript;
    SpriteRenderer sr;
    List<GameObject> daggers;
    Light lite;
    float timer;
    FMOD.Studio.EventInstance buildUp;
    FMOD.ATTRIBUTES_3D Attributes;
    FMOD.Studio.PLAYBACK_STATE buildUpState;

    void Awake()
    {
        daggers = new List<GameObject>();
        lite = GetComponentInChildren<Light>();
        playMov = GetComponentInParent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        timer = timeBetweedThrows;
        sr.enabled = false;
        sr.color = new Vector4(0, 1, 0, 0.5f);
        lite.enabled = false;
        buildUp = FMODUnity.RuntimeManager.CreateInstance("event:/Character sounds/GIZMO/Buildup");

    }


	// Update is called once per frame
	void Update () {
        if (playMov.gizmo())
        {
            timer += Time.deltaTime;
            if (Input.GetAxis("Aim") != 0 && !gScript.pauseOn && timer > timeBetweedThrows)
            {
                Vector3 position = transform.position;
                Attributes = FMODUnity.RuntimeUtils.To3DAttributes(position);
                buildUp.set3DAttributes(Attributes);
                buildUp.getPlaybackState(out buildUpState);
                if(buildUpState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
                {
                    buildUp.start();
                }
                

                lite.enabled = true;
                float maxValue = Random.Range(1.6f, 2.2f);
                lite.range = Mathf.Clamp(lite.range, 0, maxValue);
                lite.intensity = Mathf.Clamp(lite.intensity, 0, maxValue);
                lite.intensity += 2*Time.unscaledDeltaTime;
                lite.range += 2*Time.unscaledDeltaTime;
                aim();
                sr.enabled = true;
                Time.timeScale = Mathf.Lerp(Time.timeScale, 0.001f, 30 * Time.deltaTime);
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
                if (Input.GetAxisRaw("Throw") == 1 && timer > timeBetweedThrows)
                {
                    throwDagger();
                }
            }
            else
            {
                buildUp.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                lite.range = 0;
                lite.intensity = 0;
                lite.enabled = false;
                sr.enabled = false;
            }
        }
    }

    void aim()
    {
        float x = Input.GetAxisRaw("RHorizontal");
        float y = Input.GetAxisRaw("RVertical") * -1;
        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        if (playMov.isFacingRight())
         {
            angle = Mathf.Clamp(angle, -80, 80);
         }
        
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void throwDagger()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Character sounds/GIZMO/Stun pulse", transform.position);
        timer = 0;
        playMov.playThrowAnimation();
        GameObject projectile = getDagger();
        projectile.SetActive(true);
        projectile.transform.position = this.transform.position + this.transform.right;
        projectile.transform.rotation = this.transform.rotation;
        Rigidbody2D rigidbody = projectile.GetComponent<Rigidbody2D>();
        rigidbody.velocity = projectile.transform.right * daggerThrowVelocity;
    }

    GameObject getDagger()
    {
        GameObject projectile = null;
        foreach (GameObject dag in daggers)
        {
            if (!dag.activeSelf)
            {
                projectile = dag;
            }
                
        }
        if (projectile == null)
        {
            createDaggerToList();
            projectile = getDagger();
        }
        return projectile;

    }

    void createDaggerToList()
    {
        GameObject projectile = (GameObject)Instantiate(dagger, this.transform.position, this.transform.rotation);
        projectile.GetComponent<SpriteRenderer>().color = new Vector4(0.1f, 0.85f, 0.2f, 0.5f);
        projectile.SetActive(false);
        daggers.Add(projectile);
    }


}
