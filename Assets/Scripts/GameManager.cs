using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Tilemap enviromentWallsAndDoorsTilemap;

    private void Awake()
    {
        Instance = this;
    }

    public void InsertEnviromentTile(Vector2Int position, TileBase tileBase)
    {
        enviromentWallsAndDoorsTilemap.SetTile(new Vector3Int(position.x, position.y), tileBase);
    }
}
