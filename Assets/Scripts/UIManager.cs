using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("TEXT FIELDS")]
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private TextMeshProUGUI battleTurnOwnerStateText;
    [SerializeField] private TextMeshProUGUI battleActionStateText;

    [Header("BUTTONS")]
    [SerializeField] private Button mainActionAttackButton;
    [SerializeField] private Button skipActionButton;

    [Header("HIT POINTS")]
    [SerializeField] private GameObject hitPointsGroup;
    [SerializeField] private GameObject[] heartImagePrefabs;

    private void Awake()
    {
        gameStateText.text = "EXPLORING";
        battleTurnOwnerStateText.text = "-";
        battleActionStateText.text = "-";

        mainActionAttackButton.interactable = false;
        skipActionButton.interactable = false;
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

        UpdateHitPointsUI();
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
}
