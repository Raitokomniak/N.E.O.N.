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

	public Image[] panels;

	IEnumerator FadeIn;

	void Awake () {
		//Check progression (which cutscene to load)

		StartCutscene ();
	}
	

	void Update () {
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			if (onGoingPanel != panels.Length) {
				NextPanel ();
			} else {
				NextPage ();
			}
		}
	}

	void StartCutscene(){
		onGoingPage = -1;
		onGoingPanel = -1;
		NextPage ();
	}

	void NextPage(){
		//Delete previous page
		onGoingPage++;
		NewPage();
	}

	void NewPage(){
		//Create new page

		GameObject page = Instantiate (chapter1_a_pages [onGoingPage]);
		page.transform.SetParent (cutsceneCanvas.transform);
		page.transform.localScale = new Vector3 (1, 1, 1);
		page.transform.position = new Vector3 (0, 0, 0);

		panels = page.transform.GetComponentsInChildren<Image> ();

		Image panel1 = page.transform.GetChild (0).GetComponent<Image>();

		foreach (Image panel in panels) {
			Debug.Log (panel.name);
			panel.color = new Color (1,1,1,0);
		}
		NextPanel ();
	}

	void NextPanel(){
		if (onGoingPanel != -1) {
			StopCoroutine (FadeIn);
			Image previousPanel = panels [onGoingPanel];
			ForceFade (true, previousPanel);
		}

		onGoingPanel++;
		Image currentPanel = panels [onGoingPanel]; 
		currentPanel.color = new Color (1, 1, 1, 1);
		FadeIn = _FadeIn(currentPanel);
		StartCoroutine (FadeIn);
	}

	IEnumerator _FadeIn(Image panel){
		for (float a = 0.0f; a <= 1.0f; a += 0.05f) {
			panel.color = new Color (1, 1, 1, a);
			yield return new WaitForSeconds (0.05f);
		}
		panel.color = new Color (1, 1, 1, 1);
	}

	void ForceFade(bool fadein, Image panel)
	{
		if (fadein) {
			panel.color = new Color (1, 1, 1, 1);
		}
	}
}
