using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[SerializeField]
public class CutSceneInfo {
	ArrayList cutscenes;

	public CutSceneInfo (){
		cutscenes = new ArrayList ();
	}
}


public class CutsceneLibrary : MonoBehaviour {
	public CutScene[] cutscenes;
	public int cutsceneCount;
	public int[] cutscenePageCount;

	void Start(){
		
	}
	public void CreateCutscenes(){
		cutscenes = new CutScene[cutsceneCount];
		cutscenePageCount = new int[cutsceneCount];

		cutscenePageCount [0] = 1;

		GameObject[] tempPages;

		for (int j = 1; j <= cutsceneCount; j++) {
			int pageCount = cutscenePageCount [j - 1];
			tempPages = new GameObject[pageCount];
			for (int i = 0; i < pageCount; i++) {
				string path = "Cutscenes/Cutscene" + j + "/Cutscene" + j + "_Page" + i;
				tempPages[i] = Resources.Load (path) as GameObject;
			}
			cutscenes[j - 1] = new CutScene (1);
			Debug.Log (cutscenes [j -1 ]);
		}


	}
}
