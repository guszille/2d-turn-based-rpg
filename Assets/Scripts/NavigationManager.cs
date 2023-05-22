using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private Tilemap navigationTilemap;
    [SerializeField] private Tilemap debugTilemap;
    [SerializeField] private TileBaseSO tileBaseSO;

    private void Start()
    {
        navigationTilemap.CompressBounds();
        
        navigationTilemap.GetComponent<TilemapRenderer>().enabled = false;
    }

    private void Update()
    {
        Vector3 mainCharacterPosition = MainCharacterController.Instance.transform.position;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3Int mainCharacterCellPosition = navigationTilemap.WorldToCell(mainCharacterPosition);
        Vector3Int mouseCellPosition = debugTilemap.WorldToCell(mouseWorldPosition);

        Vector2Int startPosition = new Vector2Int(mainCharacterCellPosition.x, mainCharacterCellPosition.y);
        Vector2Int endPosition = new Vector2Int(mouseCellPosition.x, mouseCellPosition.y);

        List<Vector2Int> pathFound = FindPath(startPosition, endPosition);
        List<Vector3> pathToMainCharacterFollows = new List<Vector3>();

        if (pathFound != null)
        {
            debugTilemap.ClearAllTiles();

            foreach (Vector2Int position in pathFound)
            {
                Vector3Int tilemapCellPosition = new Vector3Int(position.x, position.y, 0);
                Vector3 tilemapCellCenterPosition = navigationTilemap.GetCellCenterWorld(tilemapCellPosition);

                debugTilemap.SetTile(tilemapCellPosition, tileBaseSO.tileBase);

                pathToMainCharacterFollows.Add(tilemapCellCenterPosition);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (pathToMainCharacterFollows.Count > 0)
            {
                MainCharacterController.Instance.SetPathToFollow(pathToMainCharacterFollows);
            }
        }
    }

    private List<Vector2Int> FindPath(Vector2Int startPosition, Vector2Int endPosition)
    {
        Dictionary<Vector2Int, AStar2D.Node> nodes = new Dictionary<Vector2Int, AStar2D.Node>();

        for (int x = navigationTilemap.cellBounds.xMin; x < navigationTilemap.cellBounds.xMax; x++)
        {
            for (int y = navigationTilemap.cellBounds.yMin; y < navigationTilemap.cellBounds.yMax; y++)
            {
                for (int z = navigationTilemap.cellBounds.zMin; z < navigationTilemap.cellBounds.zMax; z++)
                {
                    // Debug.Log("TILE AT (" + x + ", " + y + ", " + z + "): " + navigationTilemap.GetTile(new Vector3Int(x, y, z)));

                    if (navigationTilemap.HasTile(new Vector3Int(x, y, z)))
                    {
                        Vector2Int nodePosition = new Vector2Int(x, y);

                        nodes.Add(nodePosition, new AStar2D.Node(nodePosition));
                    }
                }
            }
        }

        return AStar2D.FindPath(startPosition, endPosition, nodes);
    }
}
