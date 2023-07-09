using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                DoButtonAction();
            }
        }
    }

    private void DoButtonAction()
    {
        isPressed = true;

        spriteRenderer.sprite = buttonDownSprite;

        for (int i = 0; i < connectionGridPositions.Length; i++)
        {
            Vector2Int gridPosition = connectionGridPositions[i];
            int level = i == 0 || i == 1 ? 2 : 1; // According the order of the positions on the list...

            GameManager.Instance.InsertEnviromentTile(gridPosition, doorOpenTileGroupSO.tileGroupList[i], level);
            NavigationManager.Instance.InsertNavigationTile(gridPosition);
        }
    }
}
