using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour {

    // Use this for initialization
    public Transform currentCheckPoint;
    public float alertTime = 20;
    bool playerDead;
    GameObject player;
    public AudioClip[] musics;
    AudioSource gameAudio;
    bool guardsAlerted;
    float countdownTimer;
    GameObject camera;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameAudio = GetComponent<AudioSource>();
        camera = GameObject.FindGameObjectWithTag("MainCamera");
    }
	void Start () {
        playerDead = false;
        guardsAlerted = false;
        countdownTimer = 0;
        gameAudio.clip = musics[0];
        gameAudio.Play();
	}
	
	// Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            reload();
        }
        if (guardsAlerted)
        {
            countdownTimer += Time.deltaTime;
            if (countdownTimer >= alertTime)
            {
                guardsAlerted = false;
               
            }
        }
        else
        {
            setMusic("Normal");
        }
        if (!gameAudio.isPlaying)
        {
            gameAudio.Play();
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
        /*int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);*/
        player.transform.position = currentCheckPoint.transform.position;
        camera.transform.position = currentCheckPoint.transform.position;
        setPlayerAlive();
        player.SetActive(true);
        guardsAlerted = false;
    }

    public void setAlertState(bool alert)
    {
        guardsAlerted = alert;
        if (alert)
        {
            countdownTimer = 0;
            setMusic("Alert");
        }
    }
    public bool allGuardsAlerted()
    {
        return guardsAlerted;
    }
    void setMusic(string music)
    {
        switch (music)
        {
            case "Normal":
                gameAudio.volume = 0.2f;
                gameAudio.clip = musics[0];
                break;
            case "Caution":
                gameAudio.clip = musics[1];
                break;
            case "Alert":
                gameAudio.volume = 0.6f;
                gameAudio.clip = musics[2];
                break;
            default:
                gameAudio.clip = musics[0];
                break;
        }
    }
}
