using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Transform MapGO;
    public GameObject Portal;
    public GameObject VoidTile;
    public List<GameObject> Tiles;
    public int LevelLength;
    public int LevelWidth;
    public int TileSize;
    public int VoidTileLine;
    void Awake()
    {
        GenerateLevel(LevelWidth, LevelLength, TileSize, VoidTileLine);
    }

    void Update()
    {
        
    }

    void GenerateLevel(int width , int length , int tileSize , int voidTileLine ,Vector3 startingTilePoint = default)
    {
        if(startingTilePoint == default)
        {
            startingTilePoint = Vector3.zero;
        }
        bool portalTile = true;
        //Generating levels by 3 tile's wide , and level lenght tile's long. Also , there can be void tile's every even tile line.
        for (int i = 0; i < length; i++)
        {
            if(i % voidTileLine == 1) //Find if we are in the even
            {
                bool voidTile = true;
                for (int j = 0; j < width; j++)
                {
                    if (voidTile && Random.Range(0, 20) > 10)
                    {
                        voidTile = false; 
                        Instantiate(VoidTile, new Vector3(j * tileSize, 0, tileSize * i), Quaternion.identity, MapGO);
                    }
                    else
                    {
                        if (i == length - 1 && portalTile)
                        {
                            Instantiate(Portal, new Vector3(j * tileSize, 1f, tileSize * i), Quaternion.identity, MapGO);
                            portalTile = false;
                        }
                        Instantiate(Tiles[Random.Range(0, Tiles.Count)], new Vector3(j * tileSize, 0, tileSize * i), Quaternion.identity, MapGO);
                    }

                }
            }
            else // Odd
            {
                for (int j = 0; j < width; j++)
                {
                    if (i == length - 1 && portalTile)
                    {
                        Instantiate(Portal, new Vector3(j * tileSize, 1f, tileSize * i), Quaternion.identity, MapGO);
                        portalTile = false;
                    }
                    Instantiate(Tiles[Random.Range(0, Tiles.Count)], new Vector3(j * tileSize, 0, tileSize * i), Quaternion.identity, MapGO);
                }
            }
        }
    }
}
