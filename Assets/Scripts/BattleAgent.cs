using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAgent : MonoBehaviour
{
    public enum AgentType { PLAYER, ENEMY, NONE }
    public enum BattleAction { ON_HOLD, MAIN, MOVEMENT }
    public enum MainActionType { ATTACK, ITEM, NONE }
    public enum MainActionState { ON_HOLD, PROJECTING, PERFORMING }
    public enum AnimationState { IDLE, RUNNING }

    public class OnBattleActionChangedEventArgs : EventArgs { public BattleAction action; }
    public event EventHandler<OnBattleActionChangedEventArgs> OnBattleActionChanged;

    public class OnAnimationStateChangedEventArgs : EventArgs { public AnimationState state; }
    public event EventHandler<OnAnimationStateChangedEventArgs> OnAnimationStateChanged;

    public event EventHandler OnBattleTurnEnded;
    public event EventHandler OnHitPointsChanged;

    [Header("NAVIGATION INFO")]
    [SerializeField] protected float movementSpeed = 10f;
    [SerializeField] protected float minThresholdToReachPosition = 0.005f;

    [Header("COMBAT INFO")]
    [SerializeField] protected float maxHitPoints = 5f;
    [SerializeField] protected float maxMovementAmount = 10f;
    [SerializeField] protected float initiative = 1f;
    [SerializeField] protected float damage = 1f;
    [SerializeField] protected float visionRange = 5f;
    [SerializeField] protected float rangeToAttack = 1.5f;

    protected List<(Vector3, int)> pathToFollow;
    protected Vector3 nextPositionToReach;
    protected bool hasPathToFollow;
    protected int navigatedDistance;

    protected bool isInBattle;
    protected AgentType agentType;
    protected BattleAction battleAction;
    protected MainActionType mainActionType;
    protected MainActionState mainActionState;
    protected float hitPoints;

    protected AnimationState animationState;

    protected List<(Vector2Int, int)> FindPathToFollow(Vector3 endPosition)
    {
        Vector2Int startCellPosition = NavigationManager.Instance.ConvertToCellPosition(transform.position);
        Vector2Int endCellPosition = NavigationManager.Instance.ConvertToCellPosition(endPosition);

        return NavigationManager.Instance.FindPath(startCellPosition, endCellPosition);
    }

    protected List<(Vector2Int, int)> FindPathToFollow(Vector3 endPosition, List<Vector2Int> blackListOfPositions)
    {
        Vector2Int startCellPosition = NavigationManager.Instance.ConvertToCellPosition(transform.position);
        Vector2Int endCellPosition = NavigationManager.Instance.ConvertToCellPosition(endPosition);

        return NavigationManager.Instance.FindPath(startCellPosition, endCellPosition, blackListOfPositions);
    }

    protected void SetPathToFollow(List<(Vector3, int)> newPathToFollow)
    {
        hasPathToFollow = true;
        pathToFollow = newPathToFollow;
    }

    protected void FollowPathFound(float translationSpeed, bool ignoresMovementLimit = false)
    {
        if (Vector3.Distance(transform.position, nextPositionToReach) > minThresholdToReachPosition)
        {
            Vector3 nextLerpPosition = Vector3.Lerp(transform.position, nextPositionToReach, translationSpeed);

            transform.position = nextLerpPosition;
        }
        else
        {
            if (pathToFollow.Count > 0)
            {
                (Vector3 positionToReach, int costToReachPosition) = pathToFollow[0];

                if (navigatedDistance + costToReachPosition <= maxMovementAmount || ignoresMovementLimit)
                {
                    nextPositionToReach = positionToReach;
                    navigatedDistance = navigatedDistance + costToReachPosition;

                    pathToFollow.RemoveAt(0);

                    LookAt(nextPositionToReach.x);
                }
                else
                {
                    StopNavigation();
                }
            }
            else
            {
                StopNavigation();
            }
        }
    }

    protected void StopNavigation()
    {
        pathToFollow = new List<(Vector3, int)>();
        hasPathToFollow = false;
        navigatedDistance = 0;

        transform.position = nextPositionToReach; // FIXME: should let the navigation stop by itself.

        SetNextAnimation(AnimationState.IDLE); // FIXME: should let the navigation stop by itself.
    }

    protected void SetNextAction(BattleAction action)
    {
        battleAction = action;

        OnBattleActionChanged?.Invoke(this, new OnBattleActionChangedEventArgs { action = action });
    }

    protected void EndTurn()
    {
        battleAction = BattleAction.ON_HOLD;

        OnBattleTurnEnded?.Invoke(this, EventArgs.Empty);
    }

    protected void LookAt(float x)
    {
        if (transform.position.x != x)
        {
            transform.localScale = new Vector3(transform.position.x <= x ? 1f : -1f, 1f, 1f);
        }
    }

    protected void SetNextAnimation(AnimationState nextAnimationState)
    {
        if (nextAnimationState != animationState)
        {
            animationState = nextAnimationState;

            OnAnimationStateChanged?.Invoke(this, new OnAnimationStateChangedEventArgs { state = animationState });
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

        mainActionType = MainActionType.NONE;
        mainActionState = MainActionState.ON_HOLD;
    }

    public void StartTurn()
    {
        SetNextAction(BattleAction.MOVEMENT);
    }

    public void SkipCurrentAction()
    {
        if (isInBattle && battleAction != BattleAction.ON_HOLD)
        {
            if (battleAction == BattleAction.MOVEMENT)
            {
                SetNextAction(BattleAction.MAIN);
            }
            else
            {
                EndTurn();
            }
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void TakeDamage(float damageTaken)
    {
        hitPoints = Mathf.Max(hitPoints - damageTaken, 0f);

        OnHitPointsChanged?.Invoke(this, EventArgs.Empty);

        if (hitPoints == 0f)
        {
            Hide();
        }
    }

    public AgentType GetAgentType()
    {
        return agentType;
    }

    public float GetMaxHitPoints()
    {
        return maxHitPoints;
    }

    public float GetHitPoints()
    {
        return hitPoints;
    }

    public float GetInitiative()
    {
        return initiative;
    }
}
