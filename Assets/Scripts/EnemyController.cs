using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BattleAgent
{
    public enum AnimationState { IDLE, RUNNING }

    public class OnAnimationStateChangedEventArgs : EventArgs { public AnimationState state; }
    public event EventHandler<OnAnimationStateChangedEventArgs> OnAnimationStateChanged;

    private AnimationState animationState;

    private void Awake()
    {
        animationState = AnimationState.IDLE;

        pathToFollow = new List<(Vector3, int)>();
        nextPositionToReach = transform.position;
        hasPathToFollow = false;
        navigatedDistance = 0;

        isInBattle = false;
        agentType = AgentType.ENEMY;
        battleAction = BattleAction.ON_HOLD;

        hitPoints = maxHitPoints;
    }

    private void Update()
    {
        if (isInBattle)
        {
            if (battleAction == BattleAction.MAIN)
            {
                // TODO: do attack action.

                EndTurn();
            }

            if (battleAction == BattleAction.MOVEMENT)
            {
                if (hasPathToFollow)
                {
                    FollowPathFound(movementSpeed * Time.deltaTime);

                    if (!hasPathToFollow)
                    {
                        animationState = AnimationState.IDLE;

                        OnAnimationStateChanged?.Invoke(this, new OnAnimationStateChangedEventArgs { state = animationState });

                        SetNextAction(BattleAction.MAIN);
                    }
                }
                else
                {
                    List<(Vector2Int, int)> pathFound = FindPathToFollow(MainCharacterController.Instance.transform.position);

                    if (pathFound.Count > 0)
                    {
                        pathFound.RemoveAt(pathFound.Count - 1); // Removing the last element prevents the enemy from reaching the same position as the target.

                        animationState = AnimationState.RUNNING;

                        OnAnimationStateChanged?.Invoke(this, new OnAnimationStateChangedEventArgs { state = animationState });

                        SetPathToFollow(NavigationManager.Instance.ConvertToWorldPosition(pathFound));
                    }
                    else
                    {
                        SetNextAction(BattleAction.MAIN);
                    }
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, MainCharacterController.Instance.transform.position) <= visionRange)
            {
                LookAt(MainCharacterController.Instance.transform.position.x);

                BattleManager.Instance.RequestBattleToStart();
            }
        }
    }
}
