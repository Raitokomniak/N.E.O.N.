using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelHandler : MonoBehaviour
{
	public ArrayList components;
	ArrayList textArray;
	int textIndex;

	IEnumerator ProcessText;

	CutsceneHandler cutsceneHandler;
	AudioSource audioSource;
	AudioClip SFXClip;

	Image previousComponent;
	Vector2 previousPanelScale;

	Outline panelOutline;

	bool textProcessed;
	public bool scalingDone;
	int sfxIndex = 0;
	//////////////////////////////////
	//PUBLIC ADJUSTABLE VARIABLES
	//////////////////////////////////
	public bool switchingOverlay;
	public bool scaled;
	public bool monochromeIn;
	public bool monochromeOut;
	public string[] speechLines;
	public string[] SFXPaths;


	void Awake (){
		cutsceneHandler = GameObject.Find ("CutsceneHandler").GetComponent<CutsceneHandler> ();
		audioSource = cutsceneHandler.GetComponent<AudioSource> ();
		panelOutline = GetComponent<Outline> ();
		panelOutline.enabled = false;
		FetchComponents ();
	}

	void FetchComponents ()
	{
		components = new ArrayList ();
		components.AddRange (GetComponentsInChildren<Image> ());
	}
		
	void ForceScaleUp(Image panel)
	{
		RectTransform panelRect = panel.GetComponent<RectTransform> ();
		panelRect.sizeDelta = new Vector2 (2560, 1440);
	}

	public void ForceScaleDown(Image panel)
	{
		RectTransform panelRect = panel.GetComponent<RectTransform> ();
		if (panelRect.sizeDelta != previousPanelScale) {
			panelRect.sizeDelta = previousPanelScale;
		}
	}
	public IEnumerator ScalePanel(Image panel)
	{
		
		scalingDone = false;
		RectTransform panelRect = panel.GetComponent<RectTransform> ();
		previousPanelScale = panelRect.sizeDelta;
		if (scaled) {
			
			ForceScaleUp (panel);
			/*/////////////
			/// SCALE UP
			/////////////
	
			
			previousPanelScale = panelRect.sizeDelta;
			for (int i = 0; i <= 2560; i += 40) {
				panelRect.sizeDelta = new Vector2 (i, i * 0.5625f);
				yield return new WaitForSeconds (0.0001f);
			}

			Debug.Log (panelRect.sizeDelta.x + " x " + panelRect.sizeDelta.y);

*/
			//yield return new WaitForSeconds (4f);

			yield return new WaitUntil (() => textProcessed == true);
			yield return new WaitForSeconds (5f);
			//Debug.Log ("textprocessed");
			/////////////
			/// SCALE DOWN
			/////////////
			yield return new WaitForSeconds (1f);
			float width = 0;
			float height = panelRect.sizeDelta.y;

			for (int i = 2560; i > previousPanelScale.x; i -= 40) {
				width = i;
				if (previousPanelScale.y != 1440f) {
					if (panelRect.sizeDelta.y > 853f) {
						height -= 40f;
					}
				} else {
					height = 1440f;
				}
				panelRect.sizeDelta = new Vector2 (i, height);
				yield return new WaitForSeconds (0.010f);
			}
			panelRect.sizeDelta = previousPanelScale;
			scalingDone = true;
		} else {
			yield return null;
		}
	}


	public IEnumerator FadeInPanel (Image panel, int panelIndex, int pageIndex)
	{
		panelOutline.enabled = true;
		foreach (Image component in components) {
			yield return new WaitForSeconds (.4f);
			if (component.tag == "Cutscene_SpeechBubble") {
				ProcessText = _ProcessText (component);

				StartCoroutine (ProcessText);
			}
				
			if (component.tag == "Cutscene_SFX") {
				PlaySFX (panelIndex, pageIndex);
			}
			if (switchingOverlay && component.tag == "Cutscene_Overlay" && previousComponent.tag == "Cutscene_Overlay") {
					StartCoroutine(FadeOut (previousComponent));
					switchingOverlay = false;
			}

		
				for (float a = 0.0f; a <= 1.0f; a += 0.02f) {
				if (monochromeIn) component.color = new Color (1, 1, 1, a * 0.44f);
				else component.color = new Color (1, 1, 1, a);
					yield return new WaitForSeconds (0.01f);
			}
		
			yield return new WaitForSeconds (1f);
			previousComponent = component;
		}
		//ForceFadeAll (true);
	}

	IEnumerator FadeOut(Image component){
		for (float a = 1.0f; a > 0.0f; a -= 0.02f) {
			component.color = new Color (1, 1, 1, a);
			yield return new WaitForSeconds (0.01f);
		}
		component.color = new Color (1, 1, 1, 0);
	}

	//use for single components
	public void ForceFade(Image component, bool fadein)
	{
		if (fadein) {
			component.color = new Color (1, 1, 1, 1);
		}
		else {
			component.color = new Color (1, 1, 1, 0);
		}
	}

	//use for all components in panel
	public void ForceFadeAll (bool fadein)
	{
		textIndex = 0;
		foreach (Image component in components) {
			if (fadein) {
				component.color = new Color (1, 1, 1, 1);
				if (component.tag == "Cutscene_SpeechBubble") {
					component.color = new Color (1, 1, 1, 1 * 0.44f);
					StopAllCoroutines ();
					Text textComponent = component.GetComponentInChildren<Text> ();
					textComponent.color = new Color (1, 1, 1, 1);
					textComponent.text = speechLines[textIndex] as string;
					textIndex++;
				}
			}
		}


	}
		
	void PlaySFX (int panel, int page){
		
		SFXClip = Resources.Load ("Cutscenes/SFX/" + SFXPaths[sfxIndex]) as AudioClip;
		audioSource.PlayOneShot (SFXClip);
		sfxIndex++;
	}


	////////////////////////////////////////////////////
	// BACKGROUND PROCESSES
	// DO NOT TOUCH
	///////////////////////////////////////////////////

	float GetProperties(int panel, int page){
		float fadeTime = 1f;
		return fadeTime;
	}

	IEnumerator _ProcessText (Image speechBubble)
	{
		textProcessed = false;
		Text textBox = speechBubble.GetComponentInChildren<Text> ();
		textBox.color = new Color (1, 1, 1, 1);

		string text = speechLines [textIndex] as string;
		textIndex++;
		char[] characters = text.ToCharArray ();
		textBox.text = "";

		for (int i = 0; i < characters.Length; i++) {
			textBox.text = textBox.text + characters [i];
			float dynamicWrite = Random.Range (0.03f, 0.06f);
			yield return new WaitForSeconds (dynamicWrite);
		}
		if (textIndex >= speechLines.Length) {
			textProcessed = true;
		}
	}

}
