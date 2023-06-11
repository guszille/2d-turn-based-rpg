using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private TextMeshProUGUI battleTurnOwnerStateText;
    [SerializeField] private TextMeshProUGUI battleActionStateText;

    private void Awake()
    {
        gameStateText.text = "EXPLORING";
        battleTurnOwnerStateText.text = "-";
        battleActionStateText.text = "-";
    }

    private void Start()
    {
        BattleManager.Instance.OnBattleModeChanged += BattleManager_OnBattleModeChanged;
        BattleManager.Instance.OnBattleTurnChanged += BattleManager_OnBattleTurnChanged;
        BattleManager.Instance.OnBattleActionChanged += BattleManager_OnBattleActionChanged;
    }

    private void BattleManager_OnBattleModeChanged(object sender, BattleManager.OnBattleModeChangedEventArgs e)
    {
        gameStateText.text = e.battleMode == BattleManager.BattleMode.ON ? "IN BATTLE" : "EXPLORING";
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
        switch (e.battleAction)
        {
            case BattleAgent.BattleAction.ON_HOLD:
                battleActionStateText.text = "-";
                break;
            case BattleAgent.BattleAction.MAIN:
                battleActionStateText.text = "MAIN ACTION";
                break;
            case BattleAgent.BattleAction.MOVEMENT:
                battleActionStateText.text = "MOVEMENT ACTION";
                break;
        }
    }
}
