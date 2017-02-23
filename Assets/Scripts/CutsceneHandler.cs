using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneHandler : MonoBehaviour {

	public GameObject[] chapter1_a_pages;
	public Canvas cutsceneCanvas;

	int chapter;
	int onGoingPage;
	int onGoingPanel;

	bool cutSceneRunning;

	GameObject currentPage;
	public Image[] panels;

	IEnumerator FadeIn;
	IEnumerator PanelTimer;

	void Awake () {
		//Check progression (which cutscene to load)
		StartCutscene ();
	}
	

	void Update () {
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			if (cutSceneRunning) {
				NextPanel ();
			}
		}
	}

	void StartCutscene(){
		cutSceneRunning = true;
		onGoingPage = -1;
		onGoingPanel = -1;
		NextPage ();
	}

	void NextPage(){
		if (currentPage != null) {
			Destroy (currentPage);
		}

		onGoingPage++;
		if (onGoingPage < chapter1_a_pages.Length) {
			onGoingPanel = -1;
			NewPage ();
			NextPanel ();
		} else {
			EndCutScene ();
		}

	}

	void NewPage(){
		currentPage = Instantiate (chapter1_a_pages [onGoingPage]);
		currentPage.transform.SetParent (cutsceneCanvas.transform);
		currentPage.transform.localScale = new Vector3 (1, 1, 1);
		currentPage.transform.position = new Vector3 (0, 0, 0);

		panels = currentPage.transform.GetComponentsInChildren<Image> ();

		foreach (Image panel in panels) {
			panel.color = new Color (1,1,1,0);
		}
	}

	void NextPanel(){
		//Force fade in prev panel
			if (onGoingPanel != -1) {
				StopCoroutine (FadeIn);
				StopCoroutine (PanelTimer);
				Image previousPanel = panels [onGoingPanel];
				ForceFade (true, previousPanel);
			}	

			onGoingPanel++;

		if (onGoingPanel < panels.Length) {
			Image currentPanel = panels [onGoingPanel]; 
			FadeIn = _FadeIn (currentPanel);
			PanelTimer = _PanelTimer ();
			StartCoroutine (FadeIn);
			StartCoroutine (PanelTimer);
		} else {
			NextPage ();
		}
	}

	IEnumerator _PanelTimer(){
		yield return new WaitForSeconds (3f);
		if (onGoingPanel < panels.Length - 1) {
			NextPanel ();
		}
	}

	IEnumerator _FadeIn(Image panel){
		for (float a = 0.0f; a <= 1.0f; a += 0.01f) {
			panel.color = new Color (1, 1, 1, a);
			yield return new WaitForSeconds (0.01f);
		}
		panel.color = new Color (1, 1, 1, 1);
	}

	void ForceFade(bool fadein, Image panel){
		if (fadein) {
			panel.color = new Color (1, 1, 1, 1);
		}
	}

	void EndCutScene(){
		cutSceneRunning = false;
		Debug.Log ("End cutscene");
	}
}
