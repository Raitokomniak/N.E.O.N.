using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneCamera : MonoBehaviour {


	//CutsceneHandler cutsceneHandler;

	Vector3 startPosition;
	Vector3 targetPosition;

	Camera cam;

	bool moving;

	bool forceMovement;

	public float FoV;

	// Use this for initialization
	void Awake () {
		moving = false;
		startPosition = transform.position;
		cam = GetComponent<Camera> ();
		if (this.isActiveAndEnabled) {
			cam.fieldOfView = FoV;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (moving) {
			if (!forceMovement) {
				if (!Mathf.Approximately (transform.position.magnitude, targetPosition.magnitude)) {
					transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime);
				} else {
					moving = false;
					Debug.Log ("destination reached");
				}
			} else {
				transform.position = targetPosition;
			}
		}
	}

	public void MoveToPanel(Image panel, bool firstPanelOfPage)
	{
		forceMovement = firstPanelOfPage;
		Vector3 pPos = panel.transform.position;
		targetPosition = new Vector3 (pPos.x, pPos.y, transform.position.z);
		moving = true;
	}

}
