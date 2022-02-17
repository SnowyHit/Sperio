using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalHandler : MonoBehaviour
{
    LevelManager LevelManager;
    private void Start()
    {
        LevelManager = GameObject.Find("Managers").GetComponent<LevelManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            LevelManager.GoToNextLevel();
        }
    }

}
