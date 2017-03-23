﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveCamera : MonoBehaviour {

	// Use this for initialization
	public float smoothing = 4;
	GameObject player;
	Camera cam;
	GameControllerScript gScript;
	void Awake () {
		cam = GetComponent<Camera>();
		player = GameObject.FindGameObjectWithTag("Player");
		gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
	}

	void Start()
	{
		//this.transform.position = new Vector2(player.transform.position.x, player.transform.position.y);
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (!gScript.isDead())
		{
			Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
			this.transform.position = Vector3.Lerp(this.transform.position, targetPos, smoothing * Time.deltaTime);
		}
	}
}