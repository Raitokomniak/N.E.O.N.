﻿using System.Collections;
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


	void Awake (){
		cutsceneHandler = GameObject.Find ("CutsceneHandler").GetComponent<CutsceneHandler> ();
		audioSource = cutsceneHandler.GetComponent<AudioSource> ();

		FetchComponents ();
	}

	void FetchComponents ()
	{
		components = new ArrayList ();
		components.AddRange (GetComponentsInChildren<Image> ());
	}
		

	public IEnumerator FadeInPanel (Image panel, int panelIndex, int pageIndex)
	{
		GetText (panelIndex, pageIndex);

		foreach (Image component in components) {
			if (component.tag == "Cutscene_SpeechBubble") {
				ProcessText = _ProcessText (component);
				StartCoroutine (ProcessText);
			}
			if (component.tag == "Cutscene_SFX") {
				PlaySFX (panelIndex, pageIndex);
			}
			for (float a = 0.0f; a <= 1.0f; a += 0.02f) {
				component.color = new Color (1, 1, 1, a);
				yield return new WaitForSeconds (0.01f);
			}
			component.color = new Color (1, 1, 1, 1);
		}
	}

	public void ForceFade (bool fadein)
	{
		textIndex = 0;
		foreach (Image component in components) {
			if (fadein) {
				component.color = new Color (1, 1, 1, 1);
				if (component.tag == "Cutscene_SpeechBubble") {
					StopAllCoroutines ();
					Text textComponent = component.GetComponentInChildren<Text> ();
					textComponent.color = new Color (0, 0, 0, 1);
					textComponent.text = textArray[textIndex] as string;
					textIndex++;
				}
			}
		}
	}
		
	void PlaySFX (int panel, int page){
		SFXClip = Resources.Load ("Cutscenes/SFX/" + GetSFX (panel, page)) as AudioClip;
		audioSource.PlayOneShot (SFXClip);
	}


	////////////////////////////////////////////////////
	// BACKGROUND PROCESSES
	// DO NOT TOUCH
	///////////////////////////////////////////////////

	string GetSFX (int panel, int page)
	{
		string path = "Cutscenes/Cutscene1/Panel_Properties";
		TextAsset propertyFile = Resources.Load (path) as TextAsset;
		string toProcess = propertyFile.text;
		string[] toSplit = toProcess.Split ('\n');

		string clipName = "";

		for (int i = 1; i < toSplit.Length; i++) {
			string[] prop = toSplit [i].Split ('\t');
			int checkedPage = int.Parse (prop [1]);
			if (checkedPage == page) {
				int checkedPanel = int.Parse (prop [2]);
				if (checkedPanel == panel) {
					clipName = prop [5];
				}
			}
		}
		return clipName;
	}

	void GetText (int panel, int page)
	{
		string path = "Cutscenes/Cutscene1/Cutscene_Text";
		TextAsset propertyFile = Resources.Load (path) as TextAsset;
		string toProcess = propertyFile.text;
		string[] toSplit = toProcess.Split ('\n');

		textArray = new ArrayList ();

		for (int i = 1; i < toSplit.Length; i++) {
			string[] prop = toSplit [i].Split ('\t');
			int checkedPage = int.Parse (prop [0]);
			if (checkedPage == page) {
				int checkedPanel = int.Parse (prop [1]);
				if (checkedPanel == panel) {
					textArray.Add (prop [2]);
					Debug.Log (prop [2]);
				}
			}
		}
	}

	IEnumerator _ProcessText (Image speechBubble)
	{
		Text textBox = speechBubble.GetComponentInChildren<Text> ();
		textBox.color = new Color (0, 0, 0, 1);

		string text = textArray [textIndex] as string;
		textIndex++;
		char[] characters = text.ToCharArray ();
		textBox.text = "";

		for (int i = 0; i < characters.Length; i++) {
			textBox.text = textBox.text + characters [i];
			float dynamicWrite = Random.Range (0.03f, 0.06f);
			yield return new WaitForSeconds (dynamicWrite);
		}
		yield return new WaitForSeconds (1f);
	}

}
