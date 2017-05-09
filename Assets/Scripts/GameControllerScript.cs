using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameControllerScript : MonoBehaviour {

    // Use this for initialization
    public GameObject currentLevelPart;
    public float alertTime = 20;
    bool playerDead;
    public AudioClip[] musics;
    AudioSource gameAudio;
    bool guardsAlerted;
    float countdownTimer;
    GameObject player;
    GameObject camera;
   // GameObject[] guards;
    //savefile stuff
    string saveFile = "saveFile.txt";
    int currentScene;
    Vector3 currentCheckpoint;
    public bool useSaveFile = true;
    int deaths;
    int alarms;
    float time;
    public GameObject checkpointText;
    //menustuff
    public GameObject pauseMenuCanvas;
    public bool pauseOn = false;
    public EventSystem eventSystem;
    public GameObject restartButton;
    public GameObject exitMainMenuButton;
    public GameObject alertIndicator;
    List<GameObject> guards;
    //string music = "event:/Music/Background 1";
    //FMOD.Studio.EventInstance Music;
    public int crushing;
    public PlayerHealth playerHealth;
    musicController music;
    //FMOD.Studio.PLAYBACK_STATE musicState;
    void Awake()
    {
        // guards = new ArrayList();
        guards = new List<GameObject>();
        guards.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        gameAudio = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        music = gameObject.GetComponent<musicController>();
        //Music = FMODUnity.RuntimeManager.CreateInstance(music);
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        crushing = 0;
        if (!File.Exists(saveFile) && useSaveFile)
        {
            using(StreamWriter sw = File.CreateText(saveFile))
            {
                //0. line currentScene int
                //1. line currentCheckpointx float
                //2. line currentCheckpointy float
                //3. line currentCheckpointz float
                //4. line deaths int
                //5. line alarms int
                //6. line time float
                sw.WriteLine("2");
                sw.WriteLine("0.0");
                sw.WriteLine("0.0");
                sw.WriteLine("0.0");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("0.0");
            }
            readSaveFile();
        }
        else if(File.Exists(saveFile) && useSaveFile)
        {
            //reaload from the savefile
            //set scene
            //set player spawnpoint
            readSaveFile();
            loadSaveFile();
        }

        pauseMenuCanvas.SetActive(false);
        pauseOn = false;
        alertIndicator.SetActive(false);
    }

	void Start () {
        playerDead = false;
        guardsAlerted = false;
        countdownTimer = 0;
        /*Music.setParameterValue("Music speed", 0);
        Music.getPlaybackState(out musicState);
        if (musicState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            Music.start();
        }*/      
        //gameAudio.clip = musics[0];
        //gameAudio.Play();
	}
	
	// Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            
            if (pauseMenuCanvas.activeInHierarchy)
            {
                unPause();
            }
            else
            {
                pause();
            }
        }
        if (guardsAlerted)
        {
            alertIndicator.SetActive(true);
            countdownTimer += Time.deltaTime;
            alertIndicator.GetComponent<Slider>().value = alertTime - countdownTimer;
            if (countdownTimer >= alertTime)
            {
                // guardsAlerted = false;
                setGuardsToStartingPosition();
            }
        }
        else
        {
            //setMusic("Normal");
            //Music.setParameterValue("Music speed", 0);
            //Music.start();
            music.setSlowMusic();
            alertIndicator.SetActive(false);
        }
        if (crushing > 1)
        {
            //setPlayerDead();
            playerHealth.takeDamage(100);
        }
        /*
        if (!gameAudio.isPlaying)
        {
            gameAudio.Play();
        }
        */

    }

    public void killGuard(GameObject guard)
    {
        guards.Remove(guard);
        Destroy(guard);
    }

    void pause()
    {
        pauseOn = true;
        pauseMenuCanvas.SetActive(true);
        if (eventSystem.currentSelectedGameObject != restartButton)
        {
            eventSystem.SetSelectedGameObject(restartButton);
        }
        
    }

    void setGuardsToStartingPosition()
    {
        bool allGuardsInStartPosition = false;
        bool atStartPos = true;
        foreach (GameObject guard in guards)
        {
            EnemyPatrollingMovement enemyMov = guard.GetComponent<EnemyPatrollingMovement>();
            if (!enemyMov.playerInSight() && enemyMov.inUse && !enemyMov.checks())
            {
                if (!enemyMov.guardInStartPosition())
                {
                    enemyMov.controlOnGameController(true);
                    enemyMov.returnToStartPosition();
                    atStartPos = false;
                }

            }
            else
            {
                atStartPos = true;
            }
            allGuardsInStartPosition = atStartPos;
        }
            
        
        if (allGuardsInStartPosition)
        {
            guardsAlerted = false;
            foreach (GameObject guard in guards)
            {
                EnemyPatrollingMovement enemyMov = guard.GetComponent<EnemyPatrollingMovement>();
                enemyMov.controlOnGameController(false);
            }
        }
    }

    void unPause()
    {
        pauseOn = false;
        eventSystem.SetSelectedGameObject(exitMainMenuButton);
        pauseMenuCanvas.SetActive(false);
    }

    public void restartButtonPressed()
    {
        readSaveFile();
        reload();
    }

    public void exitMainMenuButtonPressed()
    {
        //Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        music.stopMusic();
        SceneManager.LoadScene(0);
    }

    public void exitButtonPressed()
    {
        Application.Quit();
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
        deaths = Convert.ToInt32(rawRead[4]);
        alarms = Convert.ToInt32(rawRead[5]);
        time = Convert.ToSingle(rawRead[6]);
        Debug.Log("savefile read: scene = " + currentScene + ", checkpoint = " + currentCheckpoint + ", deaths = " + deaths + ", alarms = " + alarms + ", time = " + time);
    }

    void loadSaveFile()
    {
        if(currentScene != SceneManager.GetActiveScene().buildIndex)
        {
            reload();
        }
        player.transform.position = currentCheckpoint;
        camera.transform.position = new Vector3(currentCheckpoint.x,currentCheckpoint.y, -12f);
    }

    public void setPlayerDead()
    {
        deaths++;
        playerDead = true;
        player.SetActive(false);
        //Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        music.stopMusic();
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
        //Music.setParameterValue("Music speed", 0);
        music.setSlowMusic();
        if (useSaveFile)
        {
            time = time + Time.timeSinceLevelLoad;
            writeSavefile();
            SceneManager.LoadScene(currentScene, LoadSceneMode.Single);
        }
        else
        {
            int scene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }

        
    }
    public void loadNextScene()
    {
        //called from player entering door to next levelpart
        if (useSaveFile)
        {
            currentScene = SceneManager.GetActiveScene().buildIndex + 1;
            currentCheckpoint = new Vector3(0f, 0f, 0f);
            writeSavefile();
            //Music.setParameterValue("Music speed", 0);
            music.setSlowMusic();
            SceneManager.LoadScene(currentScene);
        }
        else
        {
            SceneManager.LoadScene(currentScene);
        }
        
    }

    public void setAlertState(bool alert)
    {
        if (!guardsAlerted)
        {
            alarms++;
        }
        guardsAlerted = alert;
        if (alert)
        {
            countdownTimer = 0;
            //setMusic("Alert");
            //Music.setParameterValue("Music speed", 1);
            music.setFastMusic();
            //Music.start();
        }
    }

    public bool allGuardsAlerted()
    {
        return guardsAlerted;
    }

    /*void setMusic(string music)
    {
        switch (music)
        {
            case "Normal":
                gameAudio.volume = 0.1f;
                gameAudio.clip = musics[0];
                break;
            case "Caution":
                gameAudio.clip = musics[1];
                break;
            case "Alert":
                gameAudio.volume = 0.3f;
                gameAudio.clip = musics[2];
                break;
            default:
                gameAudio.clip = musics[0];
                break;
        }
    }*/

    public void setCheckpoint(Vector3 checkpoint)
    {
        //caled from player triggering checkpoint
        if (useSaveFile)
        {
            Debug.Log("Checkpoint reached");
            currentCheckpoint = checkpoint;
            currentScene = SceneManager.GetActiveScene().buildIndex;
            writeSavefile();
            checkpointText.SetActive(true);
            //StartCoroutine(disableCheckpointText());
        }
    }
    IEnumerator disableCheckpointText()
    {
        yield return new WaitForSeconds(2);
        checkpointText.SetActive(false);
    }
    public void writeSavefile()
    {
        using (StreamWriter sw = File.CreateText(saveFile))
        {
            sw.Flush();
            sw.WriteLine(currentScene);
            sw.WriteLine(currentCheckpoint.x);
            sw.WriteLine(currentCheckpoint.y);
            sw.WriteLine(currentCheckpoint.z);
            sw.WriteLine(deaths);
            sw.WriteLine(alarms);
            sw.WriteLine(time);
        }
    }

  /*  public void stopMusic()
    {
        Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    ~GameControllerScript()
    {
        Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }*/
}
