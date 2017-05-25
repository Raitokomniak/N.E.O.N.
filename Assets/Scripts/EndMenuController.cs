using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndMenuController : MonoBehaviour {

    string saveFile = "saveFile.txt";
    int sceneToLoad = 0;
    public Text deaths;
    public Text alarms;
    public Text time;

    void Awake()
    {
       /* string[] rawRead = System.IO.File.ReadAllLines(saveFile);
        string Sdeaths = rawRead[4];
        string Salarms = rawRead[5];
        string Stime = rawRead[6];

        deaths.text = "You died " + Sdeaths + " times";
        alarms.text = "You triggered alarm " + Salarms + " times";
        time.text = "And it took you " + Stime + " seconds in NEON time";*/
    }

    public void mainMenuPressed()
    {
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }
}
