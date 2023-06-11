using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAgent : MonoBehaviour
{
    public enum AgentType { PLAYER, ENEMY, NONE }
    public enum BattleAction { ON_HOLD, MAIN, MOVEMENT }

    public class OnBattleActionChangedEventArgs : EventArgs { public BattleAction action; }
    public event EventHandler<OnBattleActionChangedEventArgs> OnBattleActionChanged;

    public event EventHandler OnBattleTurnEnded;

    [Header("NAVIGATION INFO")]
    [SerializeField] protected float movementSpeed = 10f;
    [SerializeField] protected float minThresholdToReachPosition = 0.005f;

    [Header("COMBAT INFO")]
    [SerializeField] protected float maxHitPoints = 5f;
    [SerializeField] protected float maxMovementAmount = 10f;
    [SerializeField] protected float initiative = 1f;
    [SerializeField] protected float damage = 1f;
    [SerializeField] protected float visionRange = 5f;

    protected List<Vector3> pathToFollow;
    protected Vector3 nextPositionToReach;
    protected bool hasPathToFollow;

    protected bool isInBattle;
    protected AgentType agentType;
    protected BattleAction battleAction;
    protected float hitPoints;

    protected List<Vector2Int> FindPathToFollow(Vector3 endPosition)
    {
        Vector2Int startCellPosition = NavigationManager.Instance.ConvertToCellPosition(transform.position);
        Vector2Int endCellPosition = NavigationManager.Instance.ConvertToCellPosition(endPosition);

        return NavigationManager.Instance.FindPath(startCellPosition, endCellPosition);
    }

    protected void SetPathToFollow(List<Vector3> newPathToFollow)
    {
        hasPathToFollow = true;
        pathToFollow = newPathToFollow;
    }

    protected void FollowPathFound(float t)
    {
        if (Vector3.Distance(transform.position, nextPositionToReach) > minThresholdToReachPosition)
        {
            Vector3 nextLerpPosition = Vector3.Lerp(transform.position, nextPositionToReach, t);

            transform.position = nextLerpPosition;
        }
        else
        {
            if (pathToFollow.Count > 0)
            {
                nextPositionToReach = pathToFollow[0];
                pathToFollow.RemoveAt(0);

                LookAt(nextPositionToReach.x);
            }
            else
            {
                hasPathToFollow = false;
            }
        }
    }

    protected void StopNavigation()
    {
        pathToFollow = new List<Vector3>();
        nextPositionToReach = transform.position;
        hasPathToFollow = false;
    }

    protected void EndTurn()
    {
        battleAction = BattleAction.ON_HOLD;

        OnBattleTurnEnded?.Invoke(this, EventArgs.Empty);
    }

    protected void SetNextAction(BattleAction action)
    {
        battleAction = action;

        OnBattleActionChanged?.Invoke(this, new OnBattleActionChangedEventArgs { action = action });
    }

    public void LookAt(float x)
    {
        if (transform.position.x != x)
        {
            transform.localScale = new Vector3(transform.position.x <= x ? 1f : -1f, 1f, 1f);
        }
    }

    public void PutInBattle()
    {
        isInBattle = true;

        StopNavigation();
        SetNextAction(BattleAction.ON_HOLD);
    }

    public void RemoveFromBattle()
    {
        isInBattle = false;

        SetNextAction(BattleAction.ON_HOLD);
    }

    public void StartTurn()
    {
        SetNextAction(BattleAction.MOVEMENT);
    }

    public AgentType GetAgentType()
    {
        return agentType;
    }

    public bool IsDead()
    {
        return hitPoints == 0f;
    }
}
