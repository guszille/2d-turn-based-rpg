using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyVisualUIController : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private Image hitPointsBar;

    private void Awake()
    {
        hitPointsBar.fillAmount = 1f;
    }

    private void Start()
    {
        enemyController.OnHitPointsChanged += EnemyController_OnHitPointsChanged;
    }

    private void Update()
    {
        UpdateHitPointsBarDisposition(); // FIXME: temporary/bad solution.
    }

    private void EnemyController_OnHitPointsChanged(object sender, System.EventArgs e)
    {
        float maxHitPoints = enemyController.GetMaxHitPoints();
        float hitPoints = enemyController.GetHitPoints();

        hitPointsBar.fillAmount = hitPoints / maxHitPoints;
    }

    private void UpdateHitPointsBarDisposition()
    {
        switch (enemyController.transform.localScale.x)
        {
            case +1f:
                hitPointsBar.fillOrigin = (int)Image.OriginHorizontal.Left;
                break;
            case -1f:
                hitPointsBar.fillOrigin = (int)Image.OriginHorizontal.Right;
                break;
        }
    }
}
