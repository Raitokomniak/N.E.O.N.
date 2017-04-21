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
    float timer;
	
    void Awake()
    {
        playMov = GetComponentInParent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        timer = 0;
        sr.enabled = false;
    }


	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (Input.GetAxis("Aim") !=0 && !gScript.pauseOn)
        {
            aim();
            sr.enabled = true;
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0.001f, 30 * Time.deltaTime);
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }
        else
        {
            sr.enabled = false;
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
        if (Input.GetAxisRaw("Throw") == 1 && timer > timeBetweedThrows)
        {
            throwDagger();
        }
        Debug.Log(angle);
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void throwDagger()
    {
        timer = 0;
        int dir = playMov.isFacingRight() ? 1 : -1;
        Vector2 startPoint = this.transform.position + this.transform.right;
        GameObject projectile = (GameObject)Instantiate(dagger, startPoint, this.transform.rotation);
        Rigidbody2D rigidbody = projectile.GetComponent<Rigidbody2D>();
        rigidbody.velocity = projectile.transform.right * daggerThrowVelocity;
    }


}
