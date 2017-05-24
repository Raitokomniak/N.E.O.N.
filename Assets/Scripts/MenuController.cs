using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour {

    int sceneToLoad;
    string saveFile = "saveFile.txt";
    public GameObject basePanel;
    public GameObject confirmationPanel;
    public GameObject continueButton;
    public EventSystem eventSystem;
    public GameObject cancelButton;
	public GameObject newGameButton;

	void Start(){
		
	}

    void Awake()
    {
        basePanel.SetActive(true);
        confirmationPanel.SetActive(false);
        
        if (!File.Exists(saveFile))
        {
            continueButton.GetComponent<Button>().interactable = false;
			continueButton.GetComponentInChildren<Text> ().color = Color.gray;
			eventSystem.SetSelectedGameObject(newGameButton);
			//eventSystem.
        }
        else
        {
            string[] rawRead = System.IO.File.ReadAllLines(saveFile);
            sceneToLoad = Convert.ToInt32(rawRead[0]);
        }
    }
		
	public void ContinuePressed()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void NewGamePressed()
    {
        if (File.Exists(saveFile))
        {
           // basePanel.SetActive(false);
            confirmationPanel.SetActive(true);
            eventSystem.SetSelectedGameObject(cancelButton);
        }
        else
        {
            ProceedPressed();
        }
    }

    public void ProceedPressed()
    {
        if (File.Exists(saveFile))
        {
            File.Delete(saveFile);
        }
        sceneToLoad = 1;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void CancelPressed()
    {
        //basePanel.SetActive(true);
        confirmationPanel.SetActive(false);
        eventSystem.SetSelectedGameObject(continueButton);
    }

    public void exitButtonPressed()
    {
        Application.Quit();
    }
}
