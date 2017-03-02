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
            reload();
        }
    }
    public void setPlayerDead()
    {
        playerDead = true;
        player.SetActive(false);
        StartCoroutine(reloadScene());
    }

    public void setPlayerAlive()
    {
        playerDead = false;
    }

    public bool isDead()
    {
        return playerDead;
    }

    IEnumerator reloadScene()
    {
        yield return new WaitForSeconds(2);
        reload();
    }

    void reload()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
