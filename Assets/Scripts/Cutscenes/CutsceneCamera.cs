﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneCamera : MonoBehaviour {


	//CutsceneHandler cutsceneHandler;

	Vector3 startPosition;
	Vector3 originalPosition;
	Vector3 targetPosition;

	Camera cam;

	public float FoV;

	// Use this for initialization
	void Awake () {
		startPosition = transform.position;
		cam = GetComponent<Camera> ();
		if (this.isActiveAndEnabled) {
			cam.fieldOfView = FoV;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MoveToPanel(Image panel)
	{
		Vector3 pPos = panel.transform.position;
		transform.position = new Vector3 (pPos.x, pPos.y, transform.position.z);

	}
}
