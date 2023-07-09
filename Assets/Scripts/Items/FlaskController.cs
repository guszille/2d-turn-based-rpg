using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlaskController : MonoBehaviour
{
    public enum FlaskType { HIT_POINTS, ARMOR_POINTS, POWER, POINSON }

    private const float FLASK_HEALING_HP_AMOUNT = 2.0f;
    private const float FLASK_HEALING_AP_AMOUNT = 2.5f;

    [SerializeField] private FlaskType flaskType;

    private void Update()
    {
        Vector2Int mainCharacterCellPosition = NavigationManager.Instance.ConvertToCellPosition(MainCharacterController.Instance.transform.position);
        Vector2Int flaskCellPosition = NavigationManager.Instance.ConvertToCellPosition(transform.position);

        if (mainCharacterCellPosition == flaskCellPosition)
        {
            switch (flaskType)
            {
                case FlaskType.HIT_POINTS:
                    MainCharacterController.Instance.HealHP(FLASK_HEALING_HP_AMOUNT);
                    break;

                case FlaskType.ARMOR_POINTS:
                    MainCharacterController.Instance.HealAP(FLASK_HEALING_AP_AMOUNT);
                    break;
            }
        
            Destroy(gameObject);
        }
    }
}
