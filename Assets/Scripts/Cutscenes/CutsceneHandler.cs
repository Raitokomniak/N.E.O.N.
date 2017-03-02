﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Page {
	public int panelCount;
	public Page (int _panelCount){
		panelCount = _panelCount;
	}
}
public class CutScene {
	public Page[] pages;
	public string musicPath;
	public CutScene (int _pages){
		pages = new Page[_pages];
	}
}

public class CutsceneHandler : MonoBehaviour
{
	public Canvas cutsceneCanvas;
	public Text endText;
	public Image progressionBar;

	int onGoingCutscene;
	int onGoingPage;
	int onGoingPanel;

	public GameObject[] cutscenePages;
	public Image[] panels;
	Image[] overlays;

	bool cutSceneRunning;

	CutScene currentCutscene;
	GameObject currentPage;

	IEnumerator FadeInPanel;
	IEnumerator PanelTimer;
	IEnumerator ScalePanel;


	void Awake ()
	{
		
		endText.gameObject.SetActive (false);
		onGoingCutscene = CheckProgression();
		StartCutscene ();
		progressionBar.transform.SetAsLastSibling ();
	}

	int CheckProgression(){
		int chapter;
		chapter = 1;
		//Reference progression controller here, return chapter number
		return chapter;
	}

	void Update ()
	{
		if (Input.GetButtonDown("Jump")) {
			if (cutSceneRunning)
				NextPanel ();
			else {
				NextScene ();
			}
		}
	}


	void StartCutscene ()
	{
		currentCutscene = CreateCutscene ();
		PlayBGM (currentCutscene.musicPath);
		cutSceneRunning = true;
		onGoingPage = -1;
		onGoingPanel = -1;
		NextPage ();
	}

	void PlayBGM(string _bgm){
		AudioSource audioSource = GetComponent<AudioSource> ();
		AudioClip bgm = Resources.Load ("Cutscenes/BGM/" + _bgm) as AudioClip;
		audioSource.clip = bgm;
		audioSource.Play ();
	}

	void NextPage ()
	{
		if (currentPage != null) Destroy (currentPage);

		onGoingPage++;

		if (onGoingPage < currentCutscene.pages.Length) {
			NewPage ();
			NextPanel ();
		} else {
			EndCutScene ();
		}

	}

	void NextPanel ()
	{
		Image panel;

		bool previousPanelExists = false;

		if (onGoingPanel != -1) {
			previousPanelExists = true;
		}	

		if (previousPanelExists) {
			StopAllCoroutines ();
			panel = panels [onGoingPanel];
			panel.GetComponent<PanelHandler> ().ForceFadeAll (true);
			panel.GetComponent<PanelHandler> ().ForceScaleDown (panel);
		}

		onGoingPanel++;

		if (onGoingPanel < panels.Length) {
			panel = panels [onGoingPanel];
			PanelHandler panelHandler = panel.GetComponent<PanelHandler> ();
			FadeInPanel = panelHandler.FadeInPanel (panel, onGoingPanel, onGoingPage);
			ScalePanel = panelHandler.ScalePanel (panel);
			PanelTimer = _PanelTimer (panelHandler);
			StartCoroutine (FadeInPanel);
			StartCoroutine (ScalePanel);
			StartCoroutine (PanelTimer);

		} else {
			NextPage ();
		}
	}


	IEnumerator _PanelTimer (PanelHandler panelHandler){
		if (panelHandler.scaled) {
			yield return new WaitUntil (() => panelHandler.scalingDone == true);
		} else {
			yield return new WaitForSeconds (12f);
		}
		if (onGoingPanel < panels.Length - 1) {
			NextPanel ();
		}
	}

	void EndCutScene (){
		endText.gameObject.SetActive (true);
		cutSceneRunning = false;
		Debug.Log ("End cutscene");
	}

	void NextScene(){
		SceneManager.LoadScene("testLevel1");
	}


	////////////////////////////////////////////////////
	// BACKGROUND PROCESSES
	// DO NOT TOUCH
	///////////////////////////////////////////////////

	void NewPage ()
	{
		onGoingPanel = -1;
		currentPage = Instantiate (cutscenePages [onGoingPage]);
		currentPage.transform.SetParent (cutsceneCanvas.transform);
		currentPage.GetComponent<RectTransform> ().offsetMax = new Vector2 (0, 0);
		currentPage.GetComponent<RectTransform> ().offsetMin = new Vector2 (0, 0);
		currentPage.transform.localScale = new Vector3 (1f, 1f, 1);
		//currentPage.transform.position = new Vector3 (0, 0, 0);

		panels = new Image[currentCutscene.pages [onGoingPage].panelCount];

		for (int i = 0; i < currentCutscene.pages [onGoingPage].panelCount; i++) {
			panels [i] = currentPage.transform.GetChild (i).GetComponent<Image> ();
		}

		foreach (Image panel in panels) {
			overlays = panel.GetComponentsInChildren<Image> ();
			foreach (Image overlay in overlays) {
				overlay.color = new Color (1, 1, 1, 0);
			}
		}
	}

	string[] LoadCutsceneProperties(){

		string path = "Cutscenes/Cutscene" + onGoingCutscene + "/Cutscene_Properties";
		TextAsset propertyFile = Resources.Load (path) as TextAsset;
		string toProcess = propertyFile.text;
		string[] toSplit = toProcess.Split('\n');
		string[] textProperties = toSplit [1].Split ('\t');

		string musicPath = textProperties [1];
		string ambientPath = textProperties [2];



		return textProperties;
	}

	int[] LoadPageProperties(){
		string path = "Cutscenes/Cutscene" + onGoingCutscene + "/Page_Properties";
		TextAsset propertyFile = Resources.Load (path) as TextAsset;
		string toProcess = propertyFile.text;
		string[] toSplit = toProcess.Split('\n');
		string[] textProperties = new string[toSplit.Length - 1];
		int[] properties = new int[textProperties.Length];

		for (int i = 0; i < textProperties.Length; i++) {
			string[] split = toSplit [i + 1].Split ('\t');
			properties[i] = int.Parse(split[1]);
		}
		return properties;
	}

	CutScene CreateCutscene ()
	{
		string[] properties = LoadCutsceneProperties ();
		int[] pageProperties = LoadPageProperties ();
		CutScene cutscene = new CutScene (int.Parse(properties[0]));
		for (int i = 0; i < pageProperties.Length; i++) {
			cutscene.pages [i] = new Page (pageProperties[i]);
		}

		cutscene.musicPath = properties [1];
		cutscenePages = new GameObject[cutscene.pages.Length];

		for (int i = 0; i < cutscene.pages.Length; i++) {
			string path = "Cutscenes/Cutscene" + onGoingCutscene + "/Cutscene" + onGoingCutscene + "_Page" + i;
			cutscenePages [i] = Resources.Load (path) as GameObject;
		}

		return cutscene;
	}
}
