using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPartLoader : MonoBehaviour
{
    public GameObject currentLevelPart;
    public GameObject nextLevelPart;
    GameObject player;
    GameObject camera;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //player.SetActive(false);
            currentLevelPart.SetActive(false);
            player.transform.position = new Vector3(0f, 0f, 0f);
            camera.transform.position = new Vector3(0f, 0f, 0f);
            nextLevelPart.SetActive(true);
            //player.SetActive(true);
        }
    }
}