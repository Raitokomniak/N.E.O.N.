using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPartLoader : MonoBehaviour
{
    public int nextScene;
    GameControllerScript gScript;
    void Awake()
    {
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gScript.loadNextScene(nextScene);
        }
    }
}