using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    public enum BattleMode { ON, OFF }

    public class OnBattleModeChangedEventArgs : EventArgs { public BattleMode battleMode; }
    public event EventHandler<OnBattleModeChangedEventArgs> OnBattleModeChanged;

    public class OnBattleTurnChangedEventArgs : EventArgs { public BattleAgent.AgentType agentType; }
    public event EventHandler<OnBattleTurnChangedEventArgs> OnBattleTurnChanged;
    
    public class OnBattleActionChangedEventArgs : EventArgs { public BattleAgent.BattleAction battleAction; }
    public event EventHandler<OnBattleActionChangedEventArgs> OnBattleActionChanged;

    [SerializeField] private List<EnemyController> enemiesInTheRoom;  // FIXME: temporary solution to get the enemies.

    private BattleMode battleMode;
    private List<BattleAgent> agentsInBattle;
    private int currentTurnOwner;

    private void Awake()
    {
        Instance = this;

        battleMode = BattleMode.OFF;
        agentsInBattle = new List<BattleAgent>();
        currentTurnOwner = 0;
    }

    private void ResetBattleManager()
    {
        agentsInBattle = new List<BattleAgent>();
        currentTurnOwner = 0;
    }

    private void SetupAgentsInBattle()
    {
        MainCharacterController.Instance.PutInBattle();

        agentsInBattle.Add(MainCharacterController.Instance);

        foreach (EnemyController enemy in enemiesInTheRoom)
        {
            enemy.PutInBattle();

            agentsInBattle.Add(enemy);
        }
    }

    private void GiveAgentTurnOwnersip(int index)
    {
        agentsInBattle[index].OnBattleActionChanged += BattleAgent_OnBattleActionChanged;
        agentsInBattle[index].OnBattleTurnEnded += BattleAgent_OnBattleTurnEnded;
    }

    private void RevokeAgentTurnOwnership(int index)
    {
        agentsInBattle[index].OnBattleActionChanged -= BattleAgent_OnBattleActionChanged;
        agentsInBattle[index].OnBattleTurnEnded -= BattleAgent_OnBattleTurnEnded;
    }

    private void BattleAgent_OnBattleActionChanged(object sender, BattleAgent.OnBattleActionChangedEventArgs e)
    {
        OnBattleActionChanged?.Invoke(this, new OnBattleActionChangedEventArgs { battleAction = e.action });
    }

    private void BattleAgent_OnBattleTurnEnded(object sender, EventArgs e)
    {
        ChangeTurnOwnership();
    }

    private void StartCurrentTurn()
    {
        GiveAgentTurnOwnersip(currentTurnOwner);
        agentsInBattle[currentTurnOwner].StartTurn();

        OnBattleTurnChanged?.Invoke(this, new OnBattleTurnChangedEventArgs {
            agentType = agentsInBattle[currentTurnOwner].GetAgentType()
        });
    }

    private void CheckIfBattleIsOver()
    {
        bool battleIsOver = true;

        foreach (BattleAgent agent in agentsInBattle)
        {
            if (agent.GetAgentType() == BattleAgent.AgentType.PLAYER && agent.IsDead())
            {
                break;
            }
            else if (!agent.IsDead())
            {
                battleIsOver = false;
                break;
            }
        }

        if (battleIsOver)
        {
            battleMode = BattleMode.OFF;
            OnBattleModeChanged?.Invoke(this, new OnBattleModeChangedEventArgs { battleMode = battleMode });

            foreach (BattleAgent agent in agentsInBattle)
            {
                agent.RemoveFromBattle();
            }

            ResetBattleManager();
        }
    }

    private void ChangeTurnOwnership()
    {
        RevokeAgentTurnOwnership(currentTurnOwner);
        CheckIfBattleIsOver();

        if (battleMode == BattleMode.ON)
        {
            currentTurnOwner = (currentTurnOwner + 1) % agentsInBattle.Count; // Move to next agent.

            StartCurrentTurn();
        }
    }

    public void RequestBattleToStart()
    {
        if (battleMode == BattleMode.OFF)
        {
            ResetBattleManager();
            SetupAgentsInBattle();

            battleMode = BattleMode.ON;
            OnBattleModeChanged?.Invoke(this, new OnBattleModeChangedEventArgs { battleMode = battleMode });

            StartCurrentTurn();
        }
    }

    public bool HasEnemyOnPosition(Vector3 position)
    {
        Vector2Int positionOnGrid = NavigationManager.Instance.ConvertToCellPosition(position);

        foreach (EnemyController enemy in enemiesInTheRoom)
        {
            if (positionOnGrid == NavigationManager.Instance.ConvertToCellPosition(enemy.transform.position))
            {
                return true;
            }
        }

        return false;
    }
}
