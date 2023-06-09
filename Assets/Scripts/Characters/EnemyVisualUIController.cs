using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyVisualUIController : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private Image hitPointsBar;
    [SerializeField] private Image armorPointsBar;

    private void Start()
    {
        float maxHitPoints = enemyController.GetMaxHitPoints();
        float hitPoints = enemyController.GetHitPoints();
        float maxArmorPoints = enemyController.GetMaxArmorPoints();
        float armorPoints = enemyController.GetArmorPoints();

        enemyController.OnHitPointsChanged += EnemyController_OnHitPointsChanged;
        enemyController.OnArmorPointsChanged += EnemyController_OnArmorPointsChanged;

        hitPointsBar.fillAmount = maxHitPoints > 0f ? hitPoints / maxHitPoints : 0f;
        armorPointsBar.fillAmount = maxArmorPoints > 0f ? armorPoints / maxArmorPoints : 0f;
    }

    private void Update()
    {
        UpdateHitPointsBarDisposition(); // FIXME: temporary/bad solution.
        UpdateArmorPointsBarDisposition(); // FIXME: temporary/bad solution.
    }

    private void EnemyController_OnHitPointsChanged(object sender, System.EventArgs e)
    {
        float maxHitPoints = enemyController.GetMaxHitPoints();
        float hitPoints = enemyController.GetHitPoints();

        hitPointsBar.fillAmount = maxHitPoints > 0f ? hitPoints / maxHitPoints : 0f;
    }

    private void EnemyController_OnArmorPointsChanged(object sender, System.EventArgs e)
    {
        float maxArmorPoints = enemyController.GetMaxArmorPoints();
        float armorPoints = enemyController.GetArmorPoints();

        armorPointsBar.fillAmount = maxArmorPoints > 0f ? armorPoints / maxArmorPoints : 0f;
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

    private void UpdateArmorPointsBarDisposition()
    {
        switch (enemyController.transform.localScale.x)
        {
            case +1f:
                armorPointsBar.fillOrigin = (int)Image.OriginHorizontal.Left;
                break;
            case -1f:
                armorPointsBar.fillOrigin = (int)Image.OriginHorizontal.Right;
                break;
        }
    }
}
