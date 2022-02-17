using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {InGame, Paused , Dead}
public class GameUI : MonoBehaviour
{
    public GameState CurrentGameState;

    public GameObject InGamePanel;
    public GameObject PausedPanel;
    public GameObject DeadPanel;
    // Start is called before the first frame update
    void Start()
    {
        SetState(CurrentGameState.ToString());
    }
    public void SetState(string State)
    {
        if(State == GameState.InGame.ToString())
        {
            Time.timeScale = 1;
            //Set Ingamepanel
            InGamePanel.SetActive(true);
            PausedPanel.SetActive(false);
            DeadPanel.SetActive(false);
        }
        else if(State == GameState.Paused.ToString())
        {
            Time.timeScale = 0; 
            //Set Paused Panel
            InGamePanel.SetActive(false);
            PausedPanel.SetActive(true);
            DeadPanel.SetActive(false);
        }
        else if (State == GameState.Dead.ToString())
        {
            Time.timeScale = 1;
            //Set DeadPanel
            InGamePanel.SetActive(false);
            PausedPanel.SetActive(false);
            DeadPanel.SetActive(true);
        }
    }
}
