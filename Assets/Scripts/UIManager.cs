using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("GAME STATE DISPLAY")]
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private TextMeshProUGUI battleTurnOwnerStateText;
    [SerializeField] private TextMeshProUGUI battleActionStateText;

    [Header("BATTLE ACTION BUTTONS")]
    [SerializeField] private Button mainActionAttackButton;
    [SerializeField] private Button mainActionUseItemButton;
    [SerializeField] private Button skipActionButton;

    [Header("PLAYER ATTRIBUTES")]
    [SerializeField] private GameObject hitPointsGroup;
    [SerializeField] private GameObject[] heartImagePrefabs;
    [SerializeField] private TextMeshProUGUI armorPointsText;
    [SerializeField] private TextMeshProUGUI initiativeText;

    [Header("ENEMY INFO VIEW")]
    [SerializeField] private GameObject enemyViewPanel;
    [SerializeField] private Image enemyHitPointsBar;
    [SerializeField] private Image enemyArmorPointsBar;
    [SerializeField] private TextMeshProUGUI enemyHitPointsText;
    [SerializeField] private TextMeshProUGUI enemyArmorPointsText;

    private BattleAgent trackedEnemy;

    private void Awake()
    {
        gameStateText.text = "EXPLORING";
        battleTurnOwnerStateText.text = "-";
        battleActionStateText.text = "-";

        mainActionAttackButton.interactable = false;
        mainActionUseItemButton.interactable = false;
        skipActionButton.interactable = false;

        enemyViewPanel.SetActive(false);

        trackedEnemy = null;
    }

    private void Start()
    {
        mainActionAttackButton.onClick.AddListener(() => {
            MainCharacterController.Instance.StartAttackAction();
        });

        skipActionButton.onClick.AddListener(() => {
            MainCharacterController.Instance.SkipCurrentAction();
        });

        BattleManager.Instance.OnBattleModeChanged += BattleManager_OnBattleModeChanged;
        BattleManager.Instance.OnBattleTurnChanged += BattleManager_OnBattleTurnChanged;
        BattleManager.Instance.OnBattleActionChanged += BattleManager_OnBattleActionChanged;

        MainCharacterController.Instance.OnHitPointsChanged += MainCharacter_OnHitPointsChanged;
        MainCharacterController.Instance.OnArmorPointsChanged += MainCharacter_OnArmorPointsChanged;

        UpdateHitPointsUI();
        UpdateArmorPointsUI();
        UpdateInitiativeUI();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // RIGHT MOUSE CLICK.
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int mouseCellPosition = NavigationManager.Instance.ConvertToCellPosition(mouseWorldPosition);

            UpdateTrackedEnemy(BattleManager.Instance.GetEnemyOnCellPosition(mouseCellPosition, false));

            UpdateEnemyViewPanel();
        }
    }

    private void BattleManager_OnBattleModeChanged(object sender, BattleManager.OnBattleModeChangedEventArgs e)
    {
        switch (e.battleMode)
        {
            case BattleManager.BattleMode.ON:
                gameStateText.text = "IN BATTLE";
                break;
            case BattleManager.BattleMode.OFF:
                gameStateText.text = "EXPLORING";
                break;
        }
    }

    private void BattleManager_OnBattleTurnChanged(object sender, BattleManager.OnBattleTurnChangedEventArgs e)
    {
        switch (e.agentType)
        {
            case BattleAgent.AgentType.NONE:
                battleTurnOwnerStateText.text = "-";
                break;
            case BattleAgent.AgentType.PLAYER:
                battleTurnOwnerStateText.text = "PLAYER";
                break;
            case BattleAgent.AgentType.ENEMY:
                battleTurnOwnerStateText.text = "ENEMY";
                break;
        }
    }

    private void BattleManager_OnBattleActionChanged(object sender, BattleManager.OnBattleActionChangedEventArgs e)
    {
        mainActionAttackButton.interactable = false;
        skipActionButton.interactable = false;

        switch (e.battleAction)
        {
            case BattleAgent.BattleAction.ON_HOLD:
                battleActionStateText.text = "-";
                break;
            case BattleAgent.BattleAction.MAIN:
                if (BattleManager.Instance.GetTurnOwner().GetAgentType() == BattleAgent.AgentType.PLAYER)
                {
                    mainActionAttackButton.interactable = true;
                    skipActionButton.interactable = true;
                }

                battleActionStateText.text = "MAIN ACTION";
                break;
            case BattleAgent.BattleAction.MOVEMENT:
                if (BattleManager.Instance.GetTurnOwner().GetAgentType() == BattleAgent.AgentType.PLAYER)
                {
                    skipActionButton.interactable = true;
                }

                battleActionStateText.text = "MOVEMENT ACTION";
                break;
        }
    }

    private void MainCharacter_OnHitPointsChanged(object sender, System.EventArgs e)
    {
        UpdateHitPointsUI();
    }

    private void MainCharacter_OnArmorPointsChanged(object sender, System.EventArgs e)
    {
        UpdateArmorPointsUI();
    }

    private void UpdateHitPointsUI()
    {
        float maxHitPoints = MainCharacterController.Instance.GetMaxHitPoints();
        float hitPoints = MainCharacterController.Instance.GetHitPoints();

        foreach (Transform child in hitPointsGroup.transform)
        {
            Destroy(child.gameObject);
        }

        for (float i = 0f; i < maxHitPoints; i++)
        {
            if (i + 1f <= hitPoints)
            {
                Instantiate(heartImagePrefabs[0], hitPointsGroup.transform); // Full.
            }
            else if (i + 0.5f <= hitPoints)
            {
                Instantiate(heartImagePrefabs[1], hitPointsGroup.transform); // Half.
            }
            else
            {
                Instantiate(heartImagePrefabs[2], hitPointsGroup.transform); // Empty.
            }
        }
    }

    private void UpdateArmorPointsUI()
    {
        float maxArmorPoints = MainCharacterController.Instance.GetMaxArmorPoints();
        float armorPoints = MainCharacterController.Instance.GetArmorPoints();

        armorPointsText.text = armorPoints.ToString("F1") + "/" + maxArmorPoints.ToString("F1");
    }

    private void UpdateInitiativeUI()
    {
        float initiative = MainCharacterController.Instance.GetInitiative();

        initiativeText.text = initiative.ToString("F1");
    }

    private void UpdateTrackedEnemy(BattleAgent newTrackedEnemy)
    {
        if (trackedEnemy != null)
        {
            trackedEnemy.OnHitPointsChanged -= TrackedEnemy_OnHitPointsChanged;
            trackedEnemy.OnArmorPointsChanged -= TrackedEnemy_OnArmorPointsChanged;
        }

        trackedEnemy = newTrackedEnemy;

        if (trackedEnemy != null)
        {
            trackedEnemy.OnHitPointsChanged += TrackedEnemy_OnHitPointsChanged;
            trackedEnemy.OnArmorPointsChanged += TrackedEnemy_OnArmorPointsChanged;
        }
    }

    private void TrackedEnemy_OnHitPointsChanged(object sender, System.EventArgs e)
    {
        UpdateEnemyViewPanel();
    }

    private void TrackedEnemy_OnArmorPointsChanged(object sender, System.EventArgs e)
    {
        UpdateEnemyViewPanel();
    }

    private void UpdateEnemyViewPanel()
    {
        if (trackedEnemy != null)
        {
            float maxArmorPoints = trackedEnemy.GetMaxArmorPoints();
            float armorPoints = trackedEnemy.GetArmorPoints();
            float maxHitPoints = trackedEnemy.GetMaxHitPoints();
            float hitPoints = trackedEnemy.GetHitPoints();

            enemyArmorPointsBar.fillAmount = maxArmorPoints > 0f ? armorPoints / maxArmorPoints : 0f;
            enemyHitPointsBar.fillAmount = maxHitPoints > 0f ? hitPoints / maxHitPoints : 0f;

            enemyArmorPointsText.text = armorPoints.ToString("F1") + "/" + maxArmorPoints.ToString("F1");
            enemyHitPointsText.text = hitPoints.ToString("F1") + "/" + maxHitPoints.ToString("F1");

            enemyViewPanel.SetActive(true);
        }
        else
        {
            enemyViewPanel.SetActive(false);
        }
    }
}
