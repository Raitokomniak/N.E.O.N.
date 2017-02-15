using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour {

    // Use this for initialization
    bool playerDead;
    GameObject player;
    public AudioClip music;
    AudioSource gameAudio;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameAudio = GetComponent<AudioSource>();
    }
	void Start () {
        playerDead = false;
	}
	
	// Update is called once per frame
    void Update()
    {
        if (!gameAudio.isPlaying)
        {
            gameAudio.clip = music;
            gameAudio.volume = Mathf.Lerp(0f, 0.85f, 0.5f);
            gameAudio.Play();
        }
        if (Input.GetKey(KeyCode.Escape))
        {

            int scene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }
        if (Input.GetKey(KeyCode.Keypad1))
        {
           // int scene = SceneManager.GetSceneByName("testScene").buildIndex;
            SceneManager.LoadScene(0, LoadSceneMode.Single);

        }
        if (Input.GetKey(KeyCode.Keypad2))
        {
          //  int scene = SceneManager.GetSceneByName("testcity").buildIndex;
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }
    public void setPlayerDead()
    {
        playerDead = true;
        Destroy(player);
    }

    public void setPlayerAlive()
    {
        playerDead = false;
    }

    public bool isDead()
    {
        return playerDead;
    }
}
