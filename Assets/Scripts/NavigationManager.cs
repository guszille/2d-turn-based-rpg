using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance { get; private set; }

    [SerializeField] private Tilemap navigationTilemap;
    [SerializeField] private Tilemap projectionTilemap;
    [SerializeField] private TileBaseSO tileBaseSO;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        navigationTilemap.CompressBounds();
        
        navigationTilemap.GetComponent<TilemapRenderer>().enabled = false;
    }

    private void Update()
    {
        ClearMarkedTilemap();
    }

    public Vector2Int ConvertToCellPosition(Vector3 position)
    {
        return (Vector2Int)projectionTilemap.WorldToCell(position); // Removing "z" coordinate.
    }

    public List<(Vector2Int, int)> FindPath(Vector2Int startPosition, Vector2Int endPosition)
    {
        Dictionary<Vector2Int, AStar2D.Node> nodes = new Dictionary<Vector2Int, AStar2D.Node>();

        for (int x = navigationTilemap.cellBounds.xMin; x < navigationTilemap.cellBounds.xMax; x++)
        {
            for (int y = navigationTilemap.cellBounds.yMin; y < navigationTilemap.cellBounds.yMax; y++)
            {
                for (int z = navigationTilemap.cellBounds.zMin; z < navigationTilemap.cellBounds.zMax; z++)
                {
                    // Debug.Log("NAV TILE AT (" + x + ", " + y + ", " + z + "): " + navigationTilemap.GetTile(new Vector3Int(x, y, z)));

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

    public List<(Vector2Int, int)> FindPath(Vector2Int startPosition, Vector2Int endPosition, List<Vector2Int> blackListOfPositions)
    {
        Dictionary<Vector2Int, AStar2D.Node> nodes = new Dictionary<Vector2Int, AStar2D.Node>();

        for (int x = navigationTilemap.cellBounds.xMin; x < navigationTilemap.cellBounds.xMax; x++)
        {
            for (int y = navigationTilemap.cellBounds.yMin; y < navigationTilemap.cellBounds.yMax; y++)
            {
                for (int z = navigationTilemap.cellBounds.zMin; z < navigationTilemap.cellBounds.zMax; z++)
                {
                    // Debug.Log("NAV TILE AT (" + x + ", " + y + ", " + z + "): " + navigationTilemap.GetTile(new Vector3Int(x, y, z)));

                    if (navigationTilemap.HasTile(new Vector3Int(x, y, z)))
                    {
                        Vector2Int nodePosition = new Vector2Int(x, y);

                        if (!blackListOfPositions.Contains(nodePosition))
                        {
                            nodes.Add(nodePosition, new AStar2D.Node(nodePosition));
                        }
                    }
                }
            }
        }

        return AStar2D.FindPath(startPosition, endPosition, nodes);
    }

    public (List<Vector2Int>, int) GetCostFromPath(List<(Vector2Int, int)> path)
    {
        List<Vector2Int> newPath = new List<Vector2Int>();
        int totalCostOfPath = 0;

        foreach ((Vector2Int position, int cost) in path)
        {
            newPath.Add(position);

            totalCostOfPath += cost;
        }

        return (newPath, totalCostOfPath);
    }

    public List<(Vector3, int)> ConvertToWorldPosition(List<(Vector2Int, int)> path)
    {
        List<(Vector3, int)> newPath = new List<(Vector3, int)>();

        foreach ((Vector2Int position, int cost) in path)
        {
            newPath.Add((navigationTilemap.GetCellCenterWorld(new Vector3Int(position.x, position.y)), cost));
        }

        return newPath;
    }

    public void MarkPosition(Vector2Int position, Color color)
    {
        projectionTilemap.color = color;
        projectionTilemap.SetTile(new Vector3Int(position.x, position.y), tileBaseSO.tileBase);
    }

    public void MarkPath(List<Vector2Int> path, Color color)
    {
        foreach (Vector2Int position in path)
        {
            MarkPosition(position, color);
        }
    }

    public void ClearMarkedTilemap()
    {
        projectionTilemap.ClearAllTiles();
    }

    public void InsertNavigationTile(Vector2Int position)
    {
        navigationTilemap.SetTile(new Vector3Int(position.x, position.y), tileBaseSO.tileBase);
    }
}
