using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private TextMeshProUGUI battleTurnOwnerStateText;
    [SerializeField] private TextMeshProUGUI battleActionStateText;

    [SerializeField] private Button mainActionAttackButton;
    [SerializeField] private Button skipActionButton;

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
}
