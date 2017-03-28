﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class GameControllerScript : MonoBehaviour {

    // Use this for initialization
    public GameObject currentLevelPart;
    public float alertTime = 20;
    bool playerDead;
    public AudioClip[] musics;
    AudioSource gameAudio;
    bool guardsAlerted;
    float countdownTimer;
    Vector3 Checkpoint;
    GameObject player;
    //savefile stuff
    string saveFile = "saveFile.txt";
    int currentScene;
    Vector3 currentCheckpoint;
    public bool useSaveFile = true;
    void Awake()
    {
        if (!File.Exists(saveFile))
        {
            using(StreamWriter sw = File.CreateText(saveFile))
            {
                //0. line currentScene int
                //1. line currentCheckpointx float
                //2. line currentCheckpointy float
                //3. line currentCheckpointz float
                sw.WriteLine("1");
                sw.WriteLine("0.0");
                sw.WriteLine("0.0");
                sw.WriteLine("0.0");
            }
        }
        else if(File.Exists(saveFile) && useSaveFile)
        {
            //reaload from the savefile
            //set scene
            //set player spawnpoint
        }
        gameAudio = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        readSaveFile();
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
    void readSaveFile()
    {
        //set atributes from the save file into gamecontroller variables
        string [] rawRead = System.IO.File.ReadAllLines(saveFile);
        currentScene = Convert.ToInt32(rawRead[0]);
        float x = Convert.ToSingle(rawRead[1]);
        float y = Convert.ToSingle(rawRead[2]);
        float z = Convert.ToSingle(rawRead[3]);
        currentCheckpoint = new Vector3(x, y, z);
        Debug.Log(currentScene);
        Debug.Log(currentCheckpoint);
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
    public void loadNextScene(int scene)
    {
        SceneManager.LoadScene(scene);
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

    public void setCheckpoint(Vector3 checkpoint)
    {
        Checkpoint = checkpoint;
    }
}
