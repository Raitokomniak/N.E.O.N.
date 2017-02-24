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
		}
	}

	CutScene CreateCutscene ()
	{
		CutScene cutscene = new CutScene (6);
		cutscene.pages [0] = new Page (4);
		cutscene.pages [1] = new Page (4);
		cutscene.pages [2] = new Page (5);
		cutscene.pages [3] = new Page (4);
		cutscene.pages [4] = new Page (4);
		cutscene.pages [5] = new Page (4);	

		cutscenePages = new GameObject[cutscene.pages.Length];

		for (int i = 0; i < cutscene.pages.Length; i++) {
			string path = "Cutscenes/Cutscene" + onGoingCutscene + "/Cutscene" + onGoingCutscene + "_Page" + i;
			Debug.Log (path);
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

		//NextPage ();
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
			ForceFade (true, previousPanel);
		}	

		onGoingPanel++;

		if (onGoingPanel < panels.Length) {
			Image currentPanel = panels [onGoingPanel]; 
			FadeInPanel = _FadeInPanel (currentPanel);
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
		
	IEnumerator _FadeInPanel (Image panel)
	{
		overlays = panel.GetComponentsInChildren<Image> ();
		foreach (Image overlay in overlays) {
			for (float a = 0.0f; a <= 1.0f; a += 0.01f) {
				overlay.color = new Color (1, 1, 1, a);
				yield return new WaitForSeconds (0.01f);
			}
			overlay.color = new Color (1, 1, 1, 1);
		}
	}

	void ForceFade (bool fadein, Image panel)
	{
		overlays = panel.GetComponentsInChildren<Image> ();
		foreach (Image overlay in overlays) {
			if (fadein) {
				overlay.color = new Color (1, 1, 1, 1);
			}
		}
	}

	void EndCutScene ()
	{
		cutSceneRunning = false;
		Debug.Log ("End cutscene");
	}
}
