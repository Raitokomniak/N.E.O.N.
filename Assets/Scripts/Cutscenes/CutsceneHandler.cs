using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Page {
	public int panelCount;
	public Page (int _panelCount){
		panelCount = _panelCount;
	}
}
class CutScene {
	public Page[] pages;
	public CutScene (int _pages){
		pages = new Page[_pages];
	}
}

public class CutsceneHandler : MonoBehaviour
{
	public Canvas cutsceneCanvas;

	int onGoingCutscene;
	int onGoingPage;
	int onGoingPanel;
	int overlayCount;

	CutScene currentCutscene;
	public GameObject[] cutscenePages;
	bool cutSceneRunning;

	GameObject currentPage;
	public Image[] panels;
	Image[] overlays;

	int musicIndex;
	int ambientIndex;

	IEnumerator FadeInPanel;
	IEnumerator PanelTimer;

	void Awake ()
	{
		//Check progression (which cutscene to load)
		onGoingCutscene = 1;

		currentCutscene = CreateCutscene ();
		StartCutscene ();
	}


	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			if (cutSceneRunning)
				NextPanel ();
			else {
				NextScene ();
			}
		}
	}


	int[] LoadCutsceneProperties(){

		string path = "Cutscenes/Cutscene" + onGoingCutscene + "/Cutscene_Properties";
		TextAsset propertyFile = Resources.Load (path) as TextAsset;
		string toProcess = propertyFile.text;
		string[] toSplit = toProcess.Split('\n');
		string[] textProperties = toSplit [1].Split ('\t');

		int[] properties = new int[textProperties.Length];

		for(int i = 0; i < textProperties.Length; i++)
		{
			properties [i] = int.Parse(textProperties [i]);
		}

		musicIndex = properties [1];
		ambientIndex = properties [2];

		return properties;
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

	string GetText(int page, int panel){
		string path = "Cutscenes/Cutscene" + onGoingCutscene + "/Cutscene_Text";
		TextAsset propertyFile = Resources.Load (path) as TextAsset;
		string toProcess = propertyFile.text;
		string[] toSplit = toProcess.Split('\n');
		string[] textProperties = new string[toSplit.Length - 1];

		/*int[] properties = new int[textProperties.Length];

		foreach (string property in textProperties) {
			
		}

		for (int i = 0; i < textProperties.Length; i++) {
			string[] split = toSplit [i + 1].Split ('\t');
			properties[i] = int.Parse(split[1]);
		}*/
		Debug.Log (textProperties [0]);
		string text = "";
		return text;
	}

	CutScene CreateCutscene ()
	{
		int[] properties = LoadCutsceneProperties ();
		int[] pageProperties = LoadPageProperties ();
		CutScene cutscene = new CutScene (properties[0]);
		for (int i = 0; i < pageProperties.Length; i++) {
			cutscene.pages [i] = new Page (pageProperties[i]);
		}

		cutscenePages = new GameObject[cutscene.pages.Length];

		for (int i = 0; i < cutscene.pages.Length; i++) {
			string path = "Cutscenes/Cutscene" + onGoingCutscene + "/Cutscene" + onGoingCutscene + "_Page" + i;
			cutscenePages [i] = Resources.Load (path) as GameObject;
		}

		return cutscene;
	}

	void StartCutscene ()
	{
		cutSceneRunning = true;
		onGoingPage = -1;
		onGoingPanel = -1;
		overlayCount = -1;

		NextPage ();
	}


	void NextPage ()
	{
		if (currentPage != null) Destroy (currentPage);

		onGoingPage++;

		if (onGoingPage < currentCutscene.pages.Length) {
			onGoingPanel = -1;
			NewPage ();
			NextPanel ();
		} else {
			EndCutScene ();
		}

	}

	void NewPage ()
	{
		currentPage = Instantiate (cutscenePages [onGoingPage]);
		currentPage.transform.SetParent (cutsceneCanvas.transform);
		currentPage.transform.localScale = new Vector3 (1, 1, 1);
		currentPage.transform.position = new Vector3 (0, 0, 0);

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

	void NextPanel ()
	{
		overlayCount = -1;
		if (onGoingPanel != -1) {
			StopCoroutine (FadeInPanel);
			StopCoroutine (PanelTimer);
			Image previousPanel = panels [onGoingPanel];
			previousPanel.GetComponent<PanelHandler>().ForceFade (true);
		}	

		onGoingPanel++;

		if (onGoingPanel < panels.Length) {
			Image currentPanel = panels [onGoingPanel];
			PanelHandler panelHandler = currentPanel.GetComponent<PanelHandler> ();
			FadeInPanel = panelHandler.FadeInPanel (currentPanel, onGoingPanel, onGoingPage);
			PanelTimer = _PanelTimer ();
			StartCoroutine (FadeInPanel);
			StartCoroutine (PanelTimer);
		} else {
			NextPage ();
		}
	}


	IEnumerator _PanelTimer ()
	{
		yield return new WaitForSeconds (5f);
		if (onGoingPanel < panels.Length - 1) {
			NextPanel ();
		}
	}



	void EndCutScene ()
	{
		cutSceneRunning = false;
		Debug.Log ("End cutscene");
	}

	void NextScene(){
		//Progress to next gameplay scene
	}
}
