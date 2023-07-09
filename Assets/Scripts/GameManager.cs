using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Tilemap enviromentLevelOneTilemap;
    [SerializeField] private Tilemap enviromentLevelTwoTilemap;

    private void Awake()
    {
        Instance = this;
    }

    public void InsertEnviromentTile(Vector2Int position, TileBase tileBase, int level = 1)
    {
        switch (level)
        {
            case 1:
                enviromentLevelOneTilemap.SetTile(new Vector3Int(position.x, position.y), tileBase);
                break;
            case 2:
                enviromentLevelTwoTilemap.SetTile(new Vector3Int(position.x, position.y), tileBase);
                break;
            default:
                // Do nothing...
                break;
        }
    }
}
