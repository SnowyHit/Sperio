using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    //GetCurrent Level and load the prefab.
    GameObject player;
    public int CurrentLevel;
    public List<GameObject> Levels;
    public List<GameObject> CreatedLevels;
    Vector3 startingPoint;
    public Vector3 NextLevelOffset;
    PlayerManager tempPlayerManager;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        tempPlayerManager = player.GetComponent<PlayerManager>();
        CurrentLevel = PlayerPrefs.HasKey("CurrentLevel") ? PlayerPrefs.GetInt("CurrentLevel") : 0;
        Debug.Log(CurrentLevel);
        startingPoint = Vector3.zero;
        //Create current and next 2 levels.
        if (Levels.Count-1 >= CurrentLevel + 1)
        {
            //Middle Level
            CreatedLevels.Add(Instantiate(Levels[CurrentLevel], startingPoint, Quaternion.Euler(0,-90,0)));
            CreatedLevels.Add(Instantiate(Levels[CurrentLevel + 1], startingPoint + NextLevelOffset, Quaternion.Euler(0, -90, 0)));
        }
        else if(Levels.Count-1 >= CurrentLevel)
        {
            //Last Level
            CreatedLevels.Add(Instantiate(Levels[CurrentLevel], startingPoint, Quaternion.Euler(0, -90, 0)));
        }
    }

    public void GoToNextLevel()
    {
        if(CurrentLevel + 1 == Levels.Count - 1)
        {
            StartCoroutine(TransportPlayer());
            PlayerPrefs.SetInt("CurrentLevel", CurrentLevel);
            return;
        } 
        else if(CurrentLevel == Levels.Count -1)
        {
            return;
        }

        startingPoint = startingPoint + NextLevelOffset;
        CurrentLevel += 1;
        PlayerPrefs.SetInt("CurrentLevel" , CurrentLevel);
        CreatedLevels.Add(Instantiate(Levels[CurrentLevel + 1], startingPoint + NextLevelOffset, Quaternion.Euler(0, -90, 0)));
        StartCoroutine(TransportPlayer());
        Destroy(CreatedLevels[0], 1f);
        CreatedLevels.RemoveAt(0);
        
    }

    public IEnumerator TransportPlayer()
    {
        tempPlayerManager.PlayerCollider.enabled = false;
        tempPlayerManager.MovementLock = true;
        tempPlayerManager.ShootLock = true;
        player.transform.DOMove(CreatedLevels[CreatedLevels.Count - 1].transform.Find("PlayerDropZone").transform.position , 1f);
        yield return new WaitForSeconds(1f);
        tempPlayerManager.MovementLock = false;
        tempPlayerManager.ShootLock = false;
        tempPlayerManager.PlayerCollider.enabled = true;
    }
}
