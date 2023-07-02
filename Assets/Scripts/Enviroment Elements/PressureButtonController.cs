using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PressureButtonController : MonoBehaviour
{
    [Header("BUTTON")]
    [SerializeField] private Sprite buttonUpSprite;
    [SerializeField] private Sprite buttonDownSprite;

    [Header("CONNECTION INFO")]
    [SerializeField] private Vector2Int[] connectionGridPositions;

    [Header("TILES")]
    [SerializeField] private TileGroupSO doorClosedTileGroupSO;
    [SerializeField] private TileGroupSO doorOpenTileGroupSO;

    private bool isPressed = false;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!BattleManager.Instance.IsBattleOn() && !isPressed)
        {
            Vector2Int mainCharacterCellPosition = NavigationManager.Instance.ConvertToCellPosition(MainCharacterController.Instance.transform.position);
            Vector2Int buttonCellPosition = NavigationManager.Instance.ConvertToCellPosition(transform.position);

            if (mainCharacterCellPosition == buttonCellPosition)
            {
                isPressed = true;

                spriteRenderer.sprite = buttonDownSprite;

                SwapDoorTiles();
            }
        }
    }

    private void SwapDoorTiles()
    {
        for (int i = 0; i < connectionGridPositions.Length; i++)
        {
            Vector2Int gridPosition = connectionGridPositions[i];
            TileBase tileBase = doorOpenTileGroupSO.tileGroupList[i];

            GameManager.Instance.InsertEnviromentTile(gridPosition, tileBase);

            NavigationManager.Instance.InsertNavigationTile(gridPosition);
        }
    }
}
