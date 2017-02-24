using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelHandler : MonoBehaviour {

	public int layers;
	public ArrayList components;
	public ArrayList overlays;
	public ArrayList SFX;
	public ArrayList speechBubbles;
	ArrayList textArray;

	IEnumerator ProcessText;

	// Use this for initialization
	void Awake () {
		components = new ArrayList ();
		overlays = new ArrayList ();
		SFX = new ArrayList ();
		speechBubbles = new ArrayList ();



		components.AddRange (GetComponentsInChildren<Image> ());
		foreach (Image component in components) {
			switch (component.tag) {
			case "Cutscene_Overlay":
				overlays.Add (component);
				break;
			case "Cutscene_SpeechBubble":
				speechBubbles.Add (component);
				break;
			case "Cutscene_SFX":
				SFX.Add (component);
				break;
			}
		}
	}

	public IEnumerator FadeInPanel (Image panel, int panelIndex, int pageIndex)
	{
		GetText (panelIndex, pageIndex);

		foreach (Image component in components) {
			if (component.tag == "Cutscene_SpeechBubble") {
				ProcessText = _ProcessText (component);
				StartCoroutine (ProcessText);
			}
			for (float a = 0.0f; a <= 1.0f; a += 0.01f) {
				component.color = new Color (1, 1, 1, a);
				yield return new WaitForSeconds (0.01f);
			}

			component.color = new Color (1, 1, 1, 1);
		}
	}

	void GetText(int panel, int page){
		string path = "Cutscenes/Cutscene1/Cutscene_Text";
		TextAsset propertyFile = Resources.Load (path) as TextAsset;
		string toProcess = propertyFile.text;
		string[] toSplit = toProcess.Split('\n');

		textArray = new ArrayList ();

		for(int i = 1; i < toSplit.Length; i++){
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

	IEnumerator _ProcessText(Image speechBubble){
		Text textBox = speechBubble.GetComponentInChildren<Text>();
		textBox.color = new Color (0, 0, 0, 1);

		foreach (string text in textArray) {

			char[] characters = text.ToCharArray ();
			textBox.text = "";

			for (int i = 0; i < characters.Length; i++) {
				textBox.text = textBox.text + characters [i];
				float dynamicWrite = Random.Range (0.03f, 0.06f);
				yield return new WaitForSeconds (dynamicWrite);
			}
		}

		yield return new WaitForSeconds (1f);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
