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
    public float MoveTime;

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
        if(CurrentLevel + 1 == Levels.Count)
        {
            tempPlayerManager.Death();
            return;
        } 
        else if(CurrentLevel + 1 == Levels.Count -1)
        {
            StartCoroutine(TransportPlayer(CreatedLevels[CreatedLevels.Count - 1].transform.Find("PlayerDropZone").transform.position));
            CurrentLevel += 1;
            return;
        }

        startingPoint = startingPoint + NextLevelOffset;
        CurrentLevel += 1;
        PlayerPrefs.SetInt("CurrentLevel" , CurrentLevel);
        CreatedLevels.Add(Instantiate(Levels[CurrentLevel + 1], startingPoint + NextLevelOffset, Quaternion.Euler(0, -90, 0)));
        StartCoroutine(TransportPlayer(CreatedLevels[CreatedLevels.Count - 1].transform.Find("PlayerDropZone").transform.position));
        Destroy(CreatedLevels[0], 1f);
        CreatedLevels.RemoveAt(0);
        
    }

    public IEnumerator TransportPlayer(Vector3 position)
    {
        float TempMoveTime = Mathf.Abs(Vector3.Distance(player.transform.position, position)) * MoveTime;
        tempPlayerManager.PlayerRigidbody.useGravity = false;
        tempPlayerManager.PlayerCollider.enabled = false;
        tempPlayerManager.MovementLock = true;
        tempPlayerManager.ShootLock = true;
        tempPlayerManager.PlayerSkin.SetActive(false);
        GameObject soul = Instantiate(tempPlayerManager.PlayerSoulGameObject, player.transform.position, Quaternion.identity);
        player.transform.DOMove(position , TempMoveTime).SetEase(Ease.OutQuad);
        player.transform.DOLookAt(position, 0.1f);
        soul.transform.DOMove(position , TempMoveTime).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(TempMoveTime);
        Destroy(soul);
        Destroy(Instantiate(tempPlayerManager.PlayerSoulExplosionGameObject, player.transform.position, Quaternion.identity , player.transform), 0.2f);
        yield return new WaitForSeconds(0.4f);
        tempPlayerManager.MovementLock = false;
        tempPlayerManager.ShootLock = false;
        tempPlayerManager.PlayerCollider.enabled = true;
        tempPlayerManager.PlayerSkin.SetActive(true);
        tempPlayerManager.PlayerRigidbody.useGravity = true;
    }

    public void ResetGameNow()
    {
        tempPlayerManager.PlayerCollider.enabled = false;
        tempPlayerManager.Revive();
        foreach (GameObject level in CreatedLevels)
        {
            Destroy(level);
        }
        CreatedLevels.Clear();
        CurrentLevel = 0;
        PlayerPrefs.SetInt("CurrentLevel", CurrentLevel);
        startingPoint = Vector3.zero;
        if (Levels.Count - 1 >= CurrentLevel + 1)
        {
            //Middle Level
            CreatedLevels.Add(Instantiate(Levels[CurrentLevel], startingPoint, Quaternion.Euler(0, -90, 0)));
            CreatedLevels.Add(Instantiate(Levels[CurrentLevel + 1], startingPoint + NextLevelOffset, Quaternion.Euler(0, -90, 0)));
        }
        else if (Levels.Count - 1 >= CurrentLevel)
        {
            //Last Level
            CreatedLevels.Add(Instantiate(Levels[CurrentLevel], startingPoint, Quaternion.Euler(0, -90, 0)));
        }
        Debug.Log(player.transform.position);
        StartCoroutine(TransportPlayer(new Vector3(5.3f , 5f, 2f)));

        GameObject.FindObjectOfType<GameUI>().SetState(GameState.InGame.ToString());
    }
}
